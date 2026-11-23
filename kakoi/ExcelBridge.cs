using NNostr.Client;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

namespace kakoi
{
    internal sealed record ExcelNoteItem(
        DateTime Time,
        string Name,
        string Note,
        string EventId,
        string Pubkey,
        int Kind);

    /// <summary>
    /// Microsoft Excel との双方向連携ブリッジ。
    /// - 1行目: 投稿/返信入力バー（C1 に本文を入力して Enter で送信、B1 を削除で返信解除）
    /// - 2行目: ヘッダー行（1〜2行目をウィンドウ枠固定）
    /// - 3行目〜: タイムライン（新着を3行目に挿入、D列入力でリアクション/リポスト/返信、行ダブルクリックで返信モード）
    /// - セル編集中（RPC_E_CALL_REJECTED）は切断せずキューに保持し、確定後に自動反映する
    /// </summary>
    internal sealed class ExcelBridge : IDisposable
    {
        private const int FirstTimelineRow = 3;
        private const int MaxTimelineRows = 500;
        private const int MaxQueueSize = 300;
        private const int FlushBatchSize = 20;

        private const int RPC_E_CALL_REJECTED = unchecked((int)0x80010001);
        private const int RPC_E_SERVERCALL_RETRYLATER = unchecked((int)0x8001010A);
        private const int RPC_E_DISCONNECTED = unchecked((int)0x80010108);
        private const int RPC_S_SERVER_UNAVAILABLE = unchecked((int)0x800706BA);

        private readonly Control _uiInvoker;
        private readonly System.Windows.Forms.Timer _flushTimer;
        private readonly ConcurrentQueue<ExcelNoteItem> _pendingNotes = new();

        private Excel.Application? _excelApp;
        private Excel.Workbook? _workbook;
        private Excel.Worksheet? _worksheet;

        private bool _isActive;
        private bool _isUpdatingExcel;
        private int _timelineRowCount;
        private string? _pendingStatus;
        private NostrEvent? _replyTarget;
        private string? _replyTargetName;

        public bool IsActive => _isActive;

        /// <summary>
        /// Excel（C1セル）からの投稿/返信要求。(content, replyTarget) => 成功したか
        /// </summary>
        public Func<string, NostrEvent?, Task<bool>>? OnPostRequested { get; set; }

        /// <summary>
        /// Excel（D列）からのリアクション要求。(eventId, pubkey, kind, emoji) => 成功したか
        /// </summary>
        public Func<string, string, int, string, Task<bool>>? OnReactionRequested { get; set; }

        /// <summary>
        /// Excel（D列の b/rp）からのリポスト要求。(eventId, pubkey, kind) => 成功したか
        /// </summary>
        public Func<string, string, int, Task<bool>>? OnRepostRequested { get; set; }

        public ExcelBridge(Control uiInvoker)
        {
            _uiInvoker = uiInvoker;
            _flushTimer = new System.Windows.Forms.Timer { Interval = 400 };
            _flushTimer.Tick += FlushTimer_Tick;
        }

        public bool Toggle(IEnumerable<ExcelNoteItem>? initialNotes = null)
        {
            if (_isActive)
            {
                Stop();
                return false;
            }

            return Start(initialNotes);
        }

        public bool Start(IEnumerable<ExcelNoteItem>? initialNotes = null)
        {
            if (_isActive)
            {
                return true;
            }

            ClearQueue();
            _timelineRowCount = 0;
            _replyTarget = null;
            _replyTargetName = null;
            _pendingStatus = null;

            try
            {
                _isUpdatingExcel = true;

                _excelApp = new Excel.Application
                {
                    DisplayAlerts = false
                };
                _workbook = _excelApp.Workbooks.Add();
                _worksheet = (Excel.Worksheet)_workbook.Sheets[1];
                _worksheet.Name = "Timeline";

                ConfigureSheetLayout(_worksheet);

                _worksheet.Change += Worksheet_Change;
                _worksheet.BeforeDoubleClick += Worksheet_BeforeDoubleClick;

                _excelApp.Visible = true;
                _isActive = true;

                if (initialNotes != null)
                {
                    // 新しい投稿が上に並ぶよう古い順からキューに積む
                    foreach (var item in initialNotes.Reverse())
                    {
                        EnqueueNote(item);
                    }
                }

                _flushTimer.Start();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Excel の初期化に失敗しました: {ex.Message}");
                Stop();
                return false;
            }
            finally
            {
                _isUpdatingExcel = false;
            }
        }

        public void EnqueueNote(DateTime time, string name, string note, string eventId, string pubkey, int kind)
        {
            EnqueueNote(new ExcelNoteItem(time, name, note, eventId, pubkey, kind));
        }

        public void EnqueueNote(ExcelNoteItem item)
        {
            if (!_isActive)
            {
                return;
            }

            while (_pendingNotes.Count >= MaxQueueSize)
            {
                _pendingNotes.TryDequeue(out _);
            }
            _pendingNotes.Enqueue(item);
        }

        public void SetStatus(string status)
        {
            if (!_isActive)
            {
                return;
            }
            _pendingStatus = status;
        }

        public void Stop()
        {
            _flushTimer.Stop();
            _isActive = false;
            _replyTarget = null;
            _replyTargetName = null;
            _pendingStatus = null;
            ClearQueue();

            try
            {
                if (_worksheet != null)
                {
                    try
                    {
                        _worksheet.Change -= Worksheet_Change;
                        _worksheet.BeforeDoubleClick -= Worksheet_BeforeDoubleClick;
                    }
                    catch (COMException)
                    {
                    }

                    if (Marshal.IsComObject(_worksheet))
                    {
                        try
                        {
                            Marshal.ReleaseComObject(_worksheet);
                        }
                        catch (COMException)
                        {
                        }
                    }
                    _worksheet = null;
                }

                if (_workbook != null)
                {
                    if (Marshal.IsComObject(_workbook))
                    {
                        try
                        {
                            _workbook.Close(false);
                        }
                        catch (COMException)
                        {
                        }
                        try
                        {
                            Marshal.ReleaseComObject(_workbook);
                        }
                        catch (COMException)
                        {
                        }
                    }
                    _workbook = null;
                }

                if (_excelApp != null)
                {
                    if (Marshal.IsComObject(_excelApp))
                    {
                        try
                        {
                            _excelApp.Quit();
                        }
                        catch (COMException)
                        {
                        }
                        try
                        {
                            Marshal.ReleaseComObject(_excelApp);
                        }
                        catch (COMException)
                        {
                        }
                    }
                    _excelApp = null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Excel リソース解放中にエラーが発生しました: {ex.Message}");
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        public void Dispose()
        {
            Stop();
            _flushTimer.Dispose();
        }

        private void ConfigureSheetLayout(Excel.Worksheet ws)
        {
            // 列幅・書式設定
            ws.Columns[1].ColumnWidth = 10;
            ws.Columns[1].NumberFormat = "hh:mm:ss";
            ws.Columns[2].ColumnWidth = 14;
            ws.Columns[3].ColumnWidth = 62;
            ws.Columns[3].WrapText = true;
            ws.Columns[4].ColumnWidth = 8;
            ws.Columns[4].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

            for (int col = 1; col <= 4; col++)
            {
                ws.Columns[col].VerticalAlignment = Excel.XlVAlign.xlVAlignTop;
            }

            // メタデータ列（E: id, F: pubkey, G: kind）は非表示
            ws.Range["E:G"].EntireColumn.Hidden = true;

            // 1行目: 入力バー
            ws.Cells[1, 1] = "投稿";
            ws.Cells[1, 2] = string.Empty;
            ws.Cells[1, 3] = string.Empty;
            ws.Cells[1, 4] = "C1に入力";

            var inputBar = ws.Range["A1:D1"];
            inputBar.Interior.Color = ColorTranslator.ToOle(Color.FromArgb(242, 242, 242));
            ws.Range["A1"].Font.Bold = true;
            ws.Range["C1"].Interior.Color = ColorTranslator.ToOle(Color.White);
            ws.Range["C1"].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            ws.Range["C1"].Borders.Color = ColorTranslator.ToOle(Color.FromArgb(180, 180, 180));
            ws.Range["D1"].Font.Color = ColorTranslator.ToOle(Color.DimGray);
            ws.Rows[1].RowHeight = 24;

            // 2行目: ヘッダー行
            ws.Cells[2, 1] = "時刻";
            ws.Cells[2, 2] = "名前";
            ws.Cells[2, 3] = "内容";
            ws.Cells[2, 4] = "反応";
            ws.Cells[2, 5] = "id";
            ws.Cells[2, 6] = "pubkey";
            ws.Cells[2, 7] = "kind";

            var header = ws.Range["A2:D2"];
            header.Font.Bold = true;
            header.Interior.Color = ColorTranslator.ToOle(Color.FromArgb(217, 225, 242));
            header.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

            // 1〜2行目をウィンドウ枠固定
            if (_excelApp?.ActiveWindow != null)
            {
                _excelApp.ActiveWindow.SplitRow = 2;
                _excelApp.ActiveWindow.FreezePanes = true;
            }

            ws.Range["C1"].Select();
        }

        private void FlushTimer_Tick(object? sender, EventArgs e)
        {
            if (!_isActive || _worksheet == null || _excelApp == null || _isUpdatingExcel)
            {
                return;
            }

            if (_pendingNotes.IsEmpty && _pendingStatus == null)
            {
                return;
            }

            try
            {
                _isUpdatingExcel = true;

                if (_pendingStatus != null)
                {
                    _worksheet.Cells[1, 4] = EscapeExcelText(_pendingStatus);
                    _pendingStatus = null;
                }

                int processed = 0;
                while (processed < FlushBatchSize && _pendingNotes.TryPeek(out var item))
                {
                    InsertTimelineRow(_worksheet, item);
                    _pendingNotes.TryDequeue(out _);
                    processed++;
                }

                if (_timelineRowCount > MaxTimelineRows)
                {
                    int trimFrom = FirstTimelineRow + MaxTimelineRows;
                    int trimTo = FirstTimelineRow + _timelineRowCount - 1;
                    if (trimTo >= trimFrom)
                    {
                        _worksheet.Range[$"{trimFrom}:{trimTo}"].Delete();
                        _timelineRowCount = MaxTimelineRows;
                    }
                }
            }
            catch (COMException ex) when (IsBusyError(ex))
            {
                // ユーザーがセル編集中（RPC_E_CALL_REJECTED 等）は切断せず次回リトライ
                Debug.WriteLine("Excel セル編集中のため書き込みを待機します。");
            }
            catch (COMException ex) when (IsDisconnectedError(ex))
            {
                Debug.WriteLine("Excel が閉じられたため連携を終了します。");
                Stop();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Excel への書き込み中にエラーが発生しました: {ex.Message}");
                Stop();
            }
            finally
            {
                _isUpdatingExcel = false;
            }
        }

        private void InsertTimelineRow(Excel.Worksheet ws, ExcelNoteItem item)
        {
            ws.Rows[FirstTimelineRow].Insert(Excel.XlInsertShiftDirection.xlShiftDown);
            ws.Rows[FirstTimelineRow].Interior.ColorIndex = Excel.XlColorIndex.xlColorIndexNone;
            ws.Rows[FirstTimelineRow].Font.Bold = false;
            ws.Rows[FirstTimelineRow].Font.Color = ColorTranslator.ToOle(Color.Black);

            ws.Cells[FirstTimelineRow, 1] = item.Time;
            ws.Cells[FirstTimelineRow, 2] = EscapeExcelText(item.Name);
            ws.Cells[FirstTimelineRow, 3] = EscapeExcelText(item.Note);
            ws.Cells[FirstTimelineRow, 4] = string.Empty;
            ws.Cells[FirstTimelineRow, 5] = EscapeExcelText(item.EventId);
            ws.Cells[FirstTimelineRow, 6] = EscapeExcelText(item.Pubkey);
            ws.Cells[FirstTimelineRow, 7] = item.Kind;

            _timelineRowCount++;
        }

        private void Worksheet_BeforeDoubleClick(Excel.Range target, ref bool cancel)
        {
            if (!_isActive || _isUpdatingExcel || _worksheet == null)
            {
                return;
            }

            try
            {
                int row = target.Row;
                if (row < FirstTimelineRow)
                {
                    return;
                }

                string id = GetCellString(_worksheet, row, 5);
                string pubkey = GetCellString(_worksheet, row, 6);
                string name = GetCellString(_worksheet, row, 2);
                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(pubkey))
                {
                    return;
                }

                cancel = true;
                SetReplyMode(id, pubkey, name);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Excel ダブルクリック処理エラー: {ex.Message}");
            }
        }

        private void Worksheet_Change(Excel.Range target)
        {
            if (!_isActive || _isUpdatingExcel || _worksheet == null)
            {
                return;
            }

            try
            {
                // 単一セル変更のみ反応（行挿入や一括操作は無視）
                if (target.CountLarge is long count && count != 1)
                {
                    return;
                }

                int row = target.Row;
                int col = target.Column;

                // B1 クリアで返信モード解除
                if (row == 1 && col == 2)
                {
                    string val = Convert.ToString(target.Value2) ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(val) && _replyTarget != null)
                    {
                        ClearReplyMode();
                    }
                    return;
                }

                // C1 確定で投稿 / 返信送信
                if (row == 1 && col == 3)
                {
                    string text = Convert.ToString(target.Value2) ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        return;
                    }

                    var replyTarget = _replyTarget;
                    _isUpdatingExcel = true;
                    try
                    {
                        _worksheet.Cells[1, 3] = string.Empty;
                        _worksheet.Cells[1, 1] = "投稿";
                        _worksheet.Cells[1, 2] = string.Empty;
                        _worksheet.Cells[1, 4] = "送信中...";
                        _replyTarget = null;
                        _replyTargetName = null;
                    }
                    finally
                    {
                        _isUpdatingExcel = false;
                    }

                    DispatchToUiAsync(async () =>
                    {
                        if (OnPostRequested != null)
                        {
                            bool ok = await OnPostRequested(text, replyTarget);
                            SetStatus(ok ? "✓ 送信済" : "送信失敗");
                        }
                    });
                    return;
                }

                // 3行目以降の D列（反応列）入力
                if (row >= FirstTimelineRow && col == 4)
                {
                    string actionText = (Convert.ToString(target.Value2) ?? string.Empty).Trim();
                    if (string.IsNullOrEmpty(actionText))
                    {
                        return;
                    }

                    string id = GetCellString(_worksheet, row, 5);
                    string pubkey = GetCellString(_worksheet, row, 6);
                    int kind = GetCellInt(_worksheet, row, 7, 1);
                    string name = GetCellString(_worksheet, row, 2);

                    if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(pubkey))
                    {
                        return;
                    }

                    // r / 返信 -> 返信モードにセット
                    if (actionText.Equals("r", StringComparison.OrdinalIgnoreCase) || actionText == "返信")
                    {
                        _isUpdatingExcel = true;
                        try
                        {
                            _worksheet.Cells[row, 4] = string.Empty;
                        }
                        finally
                        {
                            _isUpdatingExcel = false;
                        }
                        SetReplyMode(id, pubkey, name);
                        return;
                    }

                    // b / rp -> リポスト
                    if (actionText.Equals("b", StringComparison.OrdinalIgnoreCase)
                        || actionText.Equals("rp", StringComparison.OrdinalIgnoreCase))
                    {
                        _isUpdatingExcel = true;
                        try
                        {
                            _worksheet.Cells[row, 4] = "🔁";
                            _worksheet.Cells[1, 4] = "送信中...";
                        }
                        finally
                        {
                            _isUpdatingExcel = false;
                        }

                        DispatchToUiAsync(async () =>
                        {
                            if (OnRepostRequested != null)
                            {
                                bool ok = await OnRepostRequested(id, pubkey, kind);
                                SetStatus(ok ? "✓ リポスト" : "送信失敗");
                            }
                        });
                        return;
                    }

                    // それ以外（+ や絵文字） -> リアクション
                    SetStatus("送信中...");
                    DispatchToUiAsync(async () =>
                    {
                        if (OnReactionRequested != null)
                        {
                            bool ok = await OnReactionRequested(id, pubkey, kind, actionText);
                            SetStatus(ok ? $"✓ 反応({actionText})" : "送信失敗");
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Excel 変更イベント処理エラー: {ex.Message}");
            }
        }

        private void SetReplyMode(string id, string pubkey, string displayName)
        {
            if (_worksheet == null)
            {
                return;
            }

            string cleanName = displayName.Length >= 2 && displayName[1] == ' '
                ? displayName[2..]
                : displayName;

            _replyTarget = new NostrEvent
            {
                Id = id,
                PublicKey = pubkey
            };
            _replyTargetName = cleanName;

            _isUpdatingExcel = true;
            try
            {
                _worksheet.Cells[1, 1] = "返信";
                _worksheet.Cells[1, 2] = EscapeExcelText($"↩ {_replyTargetName}");
                _worksheet.Cells[1, 4] = "C1に入力";
                _worksheet.Range["C1"].Select();
            }
            finally
            {
                _isUpdatingExcel = false;
            }
        }

        private void ClearReplyMode()
        {
            if (_worksheet == null)
            {
                return;
            }

            _replyTarget = null;
            _replyTargetName = null;

            _isUpdatingExcel = true;
            try
            {
                _worksheet.Cells[1, 1] = "投稿";
                _worksheet.Cells[1, 2] = string.Empty;
                _worksheet.Cells[1, 4] = "C1に入力";
            }
            finally
            {
                _isUpdatingExcel = false;
            }
        }

        private void DispatchToUiAsync(Func<Task> action)
        {
            if (_uiInvoker.IsDisposed || !_uiInvoker.IsHandleCreated)
            {
                return;
            }

            try
            {
                _uiInvoker.BeginInvoke(new Action(async () =>
                {
                    try
                    {
                        await action();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"ExcelBridge UI アクションエラー: {ex.Message}");
                    }
                }));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ExcelBridge BeginInvoke エラー: {ex.Message}");
            }
        }

        private void ClearQueue()
        {
            while (_pendingNotes.TryDequeue(out _))
            {
            }
        }

        private static string GetCellString(Excel.Worksheet ws, int row, int col)
        {
            var value = ((Excel.Range)ws.Cells[row, col]).Value2;
            return Convert.ToString(value) ?? string.Empty;
        }

        private static int GetCellInt(Excel.Worksheet ws, int row, int col, int fallback)
        {
            var value = ((Excel.Range)ws.Cells[row, col]).Value2;
            if (value == null)
            {
                return fallback;
            }
            return int.TryParse(Convert.ToString(value), out int result) ? result : fallback;
        }

        private static string EscapeExcelText(string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            string normalized = value.Replace("\r\n", "\n").Replace("\r", "\n");
            char first = normalized[0];
            if (first is '=' or '+' or '-' or '@' or '\'')
            {
                return "'" + normalized;
            }
            return normalized;
        }

        private static bool IsBusyError(COMException ex)
        {
            return ex.ErrorCode == RPC_E_CALL_REJECTED
                || ex.ErrorCode == RPC_E_SERVERCALL_RETRYLATER;
        }

        private static bool IsDisconnectedError(COMException ex)
        {
            return ex.ErrorCode == RPC_E_DISCONNECTED
                || ex.ErrorCode == RPC_S_SERVER_UNAVAILABLE;
        }
    }
}
