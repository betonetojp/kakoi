using kakoi.Properties;
using NNostr.Client;
using NNostr.Client.Protocols;
using nokakoiCrypt;
using NTextCat;
using NTextCat.Commons;
using SkiaSharp;
using SSTPLib;
using Svg.Skia;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace kakoi
{
    public partial class FormMain : Form
    {
        #region フィールド
        private const int HOTKEY_ID = 1;
        private const int MOD_CONTROL = 0x0002;
        private const int MOD_SHIFT = 0x0004;
        private const int WM_HOTKEY = 0x0312;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        //private readonly NostrAccess _nostrAccess = new();

        private readonly string _configPath = Path.Combine(Application.StartupPath, "kakoi.config");

        private readonly FormSetting _formSetting = new();
        private readonly FormPostBar _formPostBar = new();
        private FormManiacs _formManiacs = new();
        private FormRelayList _formRelayList = new();
        private FormWeb _formWeb = new();
        internal Point _formWebLocation = new();
        internal Size _formWebSize = new();

        private string _nsec = string.Empty;
        private string _npubHex = string.Empty;

        /// <summary>
        /// フォロイー公開鍵のハッシュセット
        /// </summary>
        private readonly HashSet<string> _followeesHexs = [];
        /// <summary>
        /// ユーザー辞書
        /// </summary>
        internal Dictionary<string, User?> Users = [];
        /// <summary>
        /// キーワード通知
        /// </summary>
        internal KeywordNotifier Notifier = new();

        private bool _showAvatar;
        private bool _showOnlyFollowees;
        private bool _showOnlyJapanese;
        private string _nokakoiKey = string.Empty;
        private string _password = string.Empty;
        private bool _sendDSSTP = true;
        private bool _addClient;
        private static int _avatarSize = 32;

        private double _tempOpacity = 1.00;

        private static readonly DSSTPSender _ds = new("SakuraUnicode");
        private readonly string _SSTPMethod = "NOTIFY SSTP/1.1";
        private readonly Dictionary<string, string> _baseSSTPHeader = new(){
            {"Charset","UTF-8"},
            {"SecurityLevel","local"},
            {"Sender","kakoi"},
            {"Option","nobreak,notranslate"},
            {"Event","OnNostr"},
            {"Reference0","Nostr/0.4"}
        };

        private string _ghostName = string.Empty;
        // 重複イベントIDを保存するリスト
        private readonly LinkedList<string> _displayedEventIds = new();

        //private readonly LinkedList<NostrEvent> _noteEvents = new();

        private List<Emoji> _emojis = [];
        private List<Client> _clients = [];
        private readonly string _avatarPath = Path.Combine(Application.StartupPath, "avatar");

        private ImeStatus _imeStatus = new();
        #endregion

        #region コンストラクタ
        // コンストラクタ
        public FormMain()
        {
            InitializeComponent();

            // ボタンの画像をDPIに合わせて表示
            float scale = CreateGraphics().DpiX / 96f;
            int size = (int)(16 * scale);
            if (scale < 2.0f)
            {
                buttonRelayList.Image = new Bitmap(Resources.icons8_list_16, size, size);
                buttonStart.Image = new Bitmap(Resources.icons8_start_16, size, size);
                buttonStop.Image = new Bitmap(Resources.icons8_stop_16, size, size);
                buttonSetting.Image = new Bitmap(Resources.icons8_setting_16, size, size);
            }
            else
            {
                buttonRelayList.Image = new Bitmap(Resources.icons8_list_32, size, size);
                buttonStart.Image = new Bitmap(Resources.icons8_start_32, size, size);
                buttonStop.Image = new Bitmap(Resources.icons8_stop_32, size, size);
                buttonSetting.Image = new Bitmap(Resources.icons8_setting_32, size, size);
            }

            Setting.Load(_configPath);
            Users = Tools.LoadUsers();
            _emojis = Tools.LoadEmojis();
            comboBoxEmoji.DataSource = _emojis;
            _clients = Tools.LoadClients();

            Location = Setting.Location;
            if (new Point(0, 0) == Location || Location.X < 0 || Location.Y < 0)
            {
                StartPosition = FormStartPosition.CenterScreen;
            }
            Size = Setting.Size;
            TopMost = Setting.TopMost;
            Opacity = Setting.Opacity;
            if (0 == Opacity)
            {
                Opacity = 1;
            }
            _tempOpacity = Opacity;
            _formPostBar.Opacity = Opacity;
            _showAvatar = Setting.ShowAvatar;
            //dataGridViewNotes.Columns["avatar"].Visible = _showAvatar;
            _showOnlyFollowees = Setting.ShowOnlyFollowees;
            _showOnlyJapanese = Setting.ShowOnlyJapanese;
            _nokakoiKey = Setting.NokakoiKey;
            _sendDSSTP = Setting.SendDSSTP;
            _addClient = Setting.AddClient;
            _avatarSize = Setting.AvatarSize;

            _formPostBar.Location = Setting.PostBarLocation;
            if (new Point(0, 0) == _formPostBar.Location || _formPostBar.Location.X < 0 || _formPostBar.Location.Y < 0)
            {
                _formPostBar.StartPosition = FormStartPosition.CenterScreen;
            }
            _formPostBar.Size = Setting.PostBarSize;

            _formWebLocation = Setting.WebViewLocation;
            if (new Point(0, 0) == _formWebLocation || _formWebLocation.X < 0 || _formWebLocation.Y < 0)
            {
                _formWeb.StartPosition = FormStartPosition.CenterScreen;
            }
            _formWebSize = Setting.WebViewSize;

            dataGridViewNotes.Columns["name"].Width = Setting.NameColumnWidth;
            dataGridViewNotes.GridColor = Tools.HexToColor(Setting.GridColor);
            dataGridViewNotes.DefaultCellStyle.SelectionBackColor = Tools.HexToColor(Setting.GridColor);

            _formSetting.PostBarForm = _formPostBar;
            _formPostBar.MainForm = this;
            _formManiacs.MainForm = this;

            // avatarフォルダを作成
            if (!Directory.Exists(_avatarPath))
            {
                Directory.CreateDirectory(_avatarPath);
            }
        }
        #endregion

        #region Startボタン
        // Startボタン
        private async void ButtonStart_Click(object sender, EventArgs e)
        {
            try
            {
                int connectCount;
                if (null != NostrAccess.Clients)
                {
                    connectCount = await NostrAccess.ConnectAsync();
                }
                else
                {
                    connectCount = await NostrAccess.ConnectAsync();
                    switch (connectCount)
                    {
                        case 0:
                            labelRelays.Text = "No relay enabled.";
                            toolTipRelays.SetToolTip(labelRelays, string.Empty);
                            break;
                        case 1:
                            labelRelays.Text = NostrAccess.Relays[0].ToString();
                            toolTipRelays.SetToolTip(labelRelays, string.Join("\n", NostrAccess.Relays.Select(r => r.ToString())));
                            break;
                        default:
                            labelRelays.Text = $"{NostrAccess.Relays.Length} relays";
                            toolTipRelays.SetToolTip(labelRelays, string.Join("\n", NostrAccess.Relays.Select(r => r.ToString())));
                            break;
                    }
                    if (null != NostrAccess.Clients)
                    {
                        NostrAccess.Clients.EventsReceived += OnClientOnEventsReceived;
                    }
                }

                NostrAccess.Subscribe();

                buttonStart.Enabled = false;
                buttonStop.Enabled = true;
                dataGridViewNotes.Focus();
                _formPostBar.textBoxPost.Enabled = true;
                _formPostBar.buttonPost.Enabled = true;

                // ログイン済みの時
                if (!string.IsNullOrEmpty(_npubHex))
                {
                    // フォロイーを購読をする
                    NostrAccess.SubscribeFollows(_npubHex);
                }

                dataGridViewNotes.Rows.Clear();
            }
            catch (Exception ex)
            {
                Debug.Print(ex.ToString());
                labelRelays.Text = "Could not start.";
            }
        }
        #endregion

        #region イベント受信時処理
        /// <summary>
        /// イベント受信時処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnClientOnEventsReceived(object? sender, (string subscriptionId, NostrEvent[] events) args)
        {
            if (args.subscriptionId == NostrAccess.SubscriptionId)
            {
                #region タイムライン購読
                foreach (var nostrEvent in args.events)
                {
                    if (RemoveCompletedEventIds(nostrEvent.Id))
                    {
                        continue;
                    }

                    var content = nostrEvent.Content;
                    if (content != null)
                    {
                        // 時間表示
                        DateTimeOffset time;
                        int hour;
                        int minute;
                        string timeString = "- ";
                        if (nostrEvent.CreatedAt != null)
                        {
                            time = (DateTimeOffset)nostrEvent.CreatedAt;
                            time = time.LocalDateTime;
                            hour = time.Hour;
                            minute = time.Minute;
                            timeString = string.Format("{0:D2}", hour) + ":" + string.Format("{0:D2}", minute);
                        }

                        // Public key Color
                        var pubkeyColor = Tools.HexToColor(nostrEvent.PublicKey.Substring(0, 6));

                        // フォロイーチェック
                        string headMark = "-";
                        string speaker = "\\1"; //"\\u\\p[1]\\s[10]";
                        if (_followeesHexs.Contains(nostrEvent.PublicKey))
                        {
                            headMark = "*";
                            // 本体側がしゃべる
                            speaker = "\\0"; //"\\h\\p[0]\\s[0]";
                        }

                        #region リアクション
                        if (7 == nostrEvent.Kind)
                        {
                            // ログイン済みで自分がしたリアクション
                            if (!_npubHex.IsNullOrEmpty() && nostrEvent.PublicKey == _npubHex)
                            {
                                Users.TryGetValue(nostrEvent.PublicKey, out User? user);

                                // ユーザー表示名取得
                                string userName = GetUserName(nostrEvent.PublicKey);
                                string likedName = GetUserName(nostrEvent.GetTaggedPublicKeys()[0]);

                                // ユーザーが見つからない時は表示しない
                                if (null == user)
                                {
                                    continue;
                                }

                                headMark = "+";

                                // グリッドに表示
                                //_noteEvents.AddFirst(nostrEvent);
                                DateTimeOffset dto = nostrEvent.CreatedAt ?? DateTimeOffset.Now;
                                dataGridViewNotes.Rows.Insert(
                                0,
                                dto.ToLocalTime(),
                                new Bitmap(_avatarSize, _avatarSize), // Placeholder for Image
                                $"{headMark} {userName}",
                                $"Sent {content} to {likedName}.",
                                nostrEvent.Id,
                                nostrEvent.PublicKey
                                );

                                // avatar列のToolTipに表示名を設定
                                dataGridViewNotes.Rows[0].Cells["avatar"].ToolTipText = userName;

                                // avatar列にアバターを表示
                                if (_showAvatar && user.Picture != null && user.Picture.Length > 0)
                                {
                                    string avatarFile = Path.Combine(_avatarPath, $"{nostrEvent.PublicKey}.png");
                                    if (!_imeStatus.Compositing && !File.Exists(avatarFile))
                                    {
                                        var postBarFcuced = _formPostBar.ContainsFocus;
                                        var formSettingFocusd = _formSetting.ContainsFocus;

                                        _ = GetAvatarAsync(nostrEvent.PublicKey, user.Picture);

                                        if (postBarFcuced)
                                        {
                                            _formPostBar.Focus();
                                        }
                                        else if (formSettingFocusd)
                                        {
                                            _formSetting.Focus();
                                        }
                                        else
                                        {
                                            Focus();
                                        }
                                    }

                                    if (File.Exists(avatarFile))
                                    {
                                        using var fileStream = new FileStream(avatarFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                                        using var avatar = new Bitmap(fileStream);
                                        dataGridViewNotes.Rows[0].Cells["avatar"].Value = new Bitmap(avatar);
                                    }
                                }

                                // 背景色をリアクションカラーに変更
                                dataGridViewNotes.Rows[0].DefaultCellStyle.BackColor = Tools.HexToColor(Setting.ReactionColor);

                                // avastar列の背景色をpubkeyColorに変更
                                dataGridViewNotes.Rows[0].Cells["avatar"].Style.BackColor = pubkeyColor;

                                // クライアントタグによる背景色変更
                                var userClient = nostrEvent.GetTaggedData("client");
                                if (userClient != null && 0 < userClient.Length)
                                {
                                    Color clientColor = Color.WhiteSmoke;

                                    // userClient[0]を_clientsから検索して色を取得
                                    var client = _clients.FirstOrDefault(c => c.Name == userClient[0]);
                                    if (client != null && client.ColorCode != null)
                                    {
                                        clientColor = Tools.HexToColor(client.ColorCode);
                                    }
                                    // time列の背景色をclientColorに変更
                                    dataGridViewNotes.Rows[0].Cells["time"].Style.BackColor = clientColor;
                                }
                            }

                            // ログイン済みで自分へのリアクション
                            if (!_npubHex.IsNullOrEmpty() && nostrEvent.GetTaggedPublicKeys().Contains(_npubHex))
                            {
                                Users.TryGetValue(nostrEvent.PublicKey, out User? user);

                                // ユーザー表示名取得
                                string userName = GetUserName(nostrEvent.PublicKey);

                                // ユーザーが見つからない時は表示しない
                                if (null == user)
                                {
                                    continue;
                                }

                                headMark = "+";

                                // グリッドに表示
                                //_noteEvents.AddFirst(nostrEvent);
                                DateTimeOffset dto = nostrEvent.CreatedAt ?? DateTimeOffset.Now;
                                dataGridViewNotes.Rows.Insert(
                                0,
                                dto.ToLocalTime(),
                                new Bitmap(_avatarSize, _avatarSize), // Placeholder for Image
                                $"{headMark} {userName}",
                                nostrEvent.Content,
                                nostrEvent.Id,
                                nostrEvent.PublicKey
                                );

                                // avatar列のToolTipに表示名を設定
                                dataGridViewNotes.Rows[0].Cells["avatar"].ToolTipText = userName;

                                // avatar列にアバターを表示
                                if (_showAvatar && user.Picture != null && user.Picture.Length > 0)
                                {
                                    string avatarFile = Path.Combine(_avatarPath, $"{nostrEvent.PublicKey}.png");
                                    if (!_imeStatus.Compositing && !File.Exists(avatarFile))
                                    {
                                        var postBarFcuced = _formPostBar.ContainsFocus;
                                        var formSettingFocusd = _formSetting.ContainsFocus;

                                        _ = GetAvatarAsync(nostrEvent.PublicKey, user.Picture);

                                        if (postBarFcuced)
                                        {
                                            _formPostBar.Focus();
                                        }
                                        else if (formSettingFocusd)
                                        {
                                            _formSetting.Focus();
                                        }
                                        else
                                        {
                                            Focus();
                                        }
                                    }

                                    if (File.Exists(avatarFile))
                                    {
                                        using var fileStream = new FileStream(avatarFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                                        using var avatar = new Bitmap(fileStream);
                                        dataGridViewNotes.Rows[0].Cells["avatar"].Value = new Bitmap(avatar);
                                    }
                                }

                                // 背景色をリアクションカラーに変更
                                dataGridViewNotes.Rows[0].DefaultCellStyle.BackColor = Tools.HexToColor(Setting.ReactionColor);

                                // avastar列の背景色をpubkeyColorに変更
                                dataGridViewNotes.Rows[0].Cells["avatar"].Style.BackColor = pubkeyColor;

                                // クライアントタグによる背景色変更
                                var userClient = nostrEvent.GetTaggedData("client");
                                if (userClient != null && 0 < userClient.Length)
                                {
                                    Color clientColor = Color.WhiteSmoke;

                                    // userClient[0]を_clientsから検索して色を取得
                                    var client = _clients.FirstOrDefault(c => c.Name == userClient[0]);
                                    if (client != null && client.ColorCode != null)
                                    {
                                        clientColor = Tools.HexToColor(client.ColorCode);
                                    }
                                    // time列の背景色をclientColorに変更
                                    dataGridViewNotes.Rows[0].Cells["time"].Style.BackColor = clientColor;
                                }

                                // SSPに送る
                                if (_sendDSSTP && null != _ds)
                                {
                                    NIP19.NostrEventNote nostrEventNote = new()
                                    {
                                        EventId = nostrEvent.Id,
                                        Relays = [string.Empty],
                                    };
                                    var nevent = nostrEventNote.ToNIP19();
                                    SearchGhost();
                                    Dictionary<string, string> SSTPHeader = new(_baseSSTPHeader)
                                    {
                                        { "Reference1", "7" }, // kind
                                        { "Reference2", content }, // content
                                        { "Reference3", user?.Name ?? "???" }, // name
                                        { "Reference4", user?.DisplayName ?? string.Empty }, // display_name
                                        { "Reference5", user?.Picture ?? string.Empty }, // picture
                                        { "Reference6", nevent }, // nevent1...
                                        { "Reference7", nostrEvent.PublicKey.ConvertToNpub() }, // npub1...
                                        { "Script", $"{speaker}リアクション {userName}\\n{content}\\e" }
                                    };
                                    string sstpmsg = _SSTPMethod + "\r\n" + String.Join("\r\n", SSTPHeader.Select(kvp => kvp.Key + ": " + kvp.Value.Replace("\n", "\\n"))) + "\r\n\r\n";
                                    string r = _ds.GetSSTPResponse(_ghostName, sstpmsg);
                                    //Debug.WriteLine(r);
                                }
                            }
                        }
                        #endregion

                        #region テキストノート
                        if (1 == nostrEvent.Kind)
                        {
                            //var userClient = nostrEvent.GetTaggedData("client");
                            //var isKakoi = -1 < Array.IndexOf(userClient, "kakoi");
                            var lang = DetermineLanguage(content);
                            if (Users.TryGetValue(nostrEvent.PublicKey, out User? user) && null != user)
                            {
                                //// 言語判定結果を更新（既存ユーザー）
                                //user.Language = lang;
                            }

                            // 日本語限定表示オンで日本語じゃない時は表示しない
                            if (_showOnlyJapanese && "jpn" != lang)
                            {
                                continue;
                            }

                            // フォロイー限定表示オンでフォロイーじゃない時は表示しない
                            if (_showOnlyFollowees && !_followeesHexs.Contains(nostrEvent.PublicKey))
                            {
                                continue;
                            }

                            // ミュートしている時は表示しない
                            if (IsMuted(nostrEvent.PublicKey))
                            {
                                continue;
                            }

                            // ユーザー表示名取得（ユーザー辞書メモリ節約のため↑のフラグ処理後に）
                            string userName = GetUserName(nostrEvent.PublicKey);

                            // ユーザーが見つからない時は表示しない
                            if (null == user)
                            {
                                continue;
                            }

                            bool isReply = false;
                            var e = nostrEvent.GetTaggedData("e");
                            var p = nostrEvent.GetTaggedData("p");
                            var q = nostrEvent.GetTaggedData("q");
                            if (e != null && 0 < e.Length ||
                                p != null && 0 < p.Length ||
                                q != null && 0 < q.Length)
                            {
                                isReply = true;
                                //headMark = "<";
                            }

                            // グリッドに表示
                            //_noteEvents.AddFirst(nostrEvent);
                            DateTimeOffset dto = nostrEvent.CreatedAt ?? DateTimeOffset.Now;
                            dataGridViewNotes.Rows.Insert(
                                0,
                                dto.ToLocalTime(),
                                new Bitmap(_avatarSize, _avatarSize), // Placeholder for Image
                                $"{headMark} {userName}",
                                nostrEvent.Content,
                                nostrEvent.Id,
                                nostrEvent.PublicKey
                                );
                            //dataGridViewNotes.Sort(dataGridViewNotes.Columns["time"], ListSortDirection.Descending);

                            // avatar列のToolTipに表示名を設定
                            dataGridViewNotes.Rows[0].Cells["avatar"].ToolTipText = userName;

                            // avatar列にアバターを表示
                            if (_showAvatar && user.Picture != null && user.Picture.Length > 0)
                            {
                                string avatarFile = Path.Combine(_avatarPath, $"{nostrEvent.PublicKey}.png");
                                if (!_imeStatus.Compositing && !File.Exists(avatarFile))
                                {
                                    var postBarFcuced = _formPostBar.ContainsFocus;
                                    var formSettingFocusd = _formSetting.ContainsFocus;

                                    _ = GetAvatarAsync(nostrEvent.PublicKey, user.Picture);

                                    if (postBarFcuced)
                                    {
                                        _formPostBar.Focus();
                                    }
                                    else if (formSettingFocusd)
                                    {
                                        _formSetting.Focus();
                                    }
                                    else
                                    {
                                        Focus();
                                    }
                                }

                                if (File.Exists(avatarFile))
                                {
                                    using var fileStream = new FileStream(avatarFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                                    using var avatar = new Bitmap(fileStream);
                                    dataGridViewNotes.Rows[0].Cells["avatar"].Value = new Bitmap(avatar);
                                }
                            }

                            // リプライの時は背景色変更
                            if (isReply)
                            {
                                dataGridViewNotes.Rows[0].DefaultCellStyle.BackColor = Tools.HexToColor(Setting.ReplyColor);
                            }

                            // avastar列の背景色をpubkeyColorに変更
                            dataGridViewNotes.Rows[0].Cells["avatar"].Style.BackColor = pubkeyColor;

                            // クライアントタグによる背景色変更
                            var userClient = nostrEvent.GetTaggedData("client");
                            if (userClient != null && 0 < userClient.Length)
                            {
                                Color clientColor = Color.WhiteSmoke;

                                // userClient[0]を_clientsから検索して色を取得
                                var client = _clients.FirstOrDefault(c => c.Name == userClient[0]);
                                if (client != null && client.ColorCode != null)
                                {
                                    clientColor = Tools.HexToColor(client.ColorCode);
                                }
                                // time列の背景色をclientColorに変更
                                dataGridViewNotes.Rows[0].Cells["time"].Style.BackColor = clientColor;
                            }

                            // SSPに送る
                            if (_sendDSSTP && null != _ds)
                            {
                                NIP19.NostrEventNote nostrEventNote = new()
                                {
                                    EventId = nostrEvent.Id,
                                    Relays = [string.Empty],
                                };
                                var nevent = nostrEventNote.ToNIP19();
                                SearchGhost();

                                string msg = content;
                                Dictionary<string, string> SSTPHeader = new(_baseSSTPHeader)
                                {
                                    { "Reference1", "1" }, // kind
                                    { "Reference2", content }, // content
                                    { "Reference3", user?.Name ?? "???" }, // name
                                    { "Reference4", user?.DisplayName ?? string.Empty }, // display_name
                                    { "Reference5", user?.Picture ?? string.Empty }, // picture
                                    { "Reference6", nevent }, // nevent1...
                                    { "Reference7", nostrEvent.PublicKey.ConvertToNpub() }, // npub1...
                                    { "Script", $"{speaker}{userName}\\n{msg}\\e" }
                                };
                                string sstpmsg = _SSTPMethod + "\r\n" + String.Join("\r\n", SSTPHeader.Select(kvp => kvp.Key + ": " + kvp.Value.Replace("\n", "\\n"))) + "\r\n\r\n";
                                string r = _ds.GetSSTPResponse(_ghostName, sstpmsg);
                                //Debug.WriteLine(r);
                            }

                            // キーワード通知
                            var settings = Notifier.Settings;
                            if (Notifier.CheckPost(content))
                            {
                                if (settings.Reaction)
                                {
                                    _ = ReactionAsync(nostrEvent.Id, nostrEvent.PublicKey, "+");
                                }

                                if (settings.Open)
                                {
                                    NIP19.NostrEventNote nostrEventNote = new()
                                    {
                                        EventId = nostrEvent.Id,
                                        Relays = [string.Empty],
                                    };
                                    var nevent = nostrEventNote.ToNIP19();
                                    var app = new ProcessStartInfo
                                    {
                                        FileName = settings.FileName + nevent,
                                        UseShellExecute = true
                                    };
                                    try
                                    {
                                        Process.Start(app);
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.WriteLine(ex.Message);
                                    }
                                }
                            }

                            // 改行をスペースに置き換え
                            content = content.Replace('\n', ' ');
                            Debug.WriteLine($"{timeString} {userName} {content}");
                        }
                        #endregion

                        #region リポスト
                        if (6 == nostrEvent.Kind)
                        {
                            Users.TryGetValue(nostrEvent.PublicKey, out User? user);

                            // フォロイー限定表示オンでフォロイーじゃない時は表示しない
                            if (_showOnlyFollowees && !_followeesHexs.Contains(nostrEvent.PublicKey))
                            {
                                continue;
                            }

                            // ミュートしている時は表示しない
                            if (IsMuted(nostrEvent.PublicKey))
                            {
                                continue;
                            }

                            // ユーザー表示名取得
                            string userName = GetUserName(nostrEvent.PublicKey);

                            // ユーザーが見つからない時は表示しない
                            if (null == user)
                            {
                                continue;
                            }

                            //headMark = ">";

                            // グリッドに表示
                            DateTimeOffset dto = nostrEvent.CreatedAt ?? DateTimeOffset.Now;
                            dataGridViewNotes.Rows.Insert(
                            0,
                            dto.ToLocalTime(),
                            new Bitmap(_avatarSize, _avatarSize), // Placeholder for Image
                            $"{headMark} {userName}",
                            $"reposted {GetUserName(nostrEvent.GetTaggedPublicKeys().Last())}'s post.",
                            nostrEvent.Id,
                            nostrEvent.PublicKey
                            );

                            // avatar列のToolTipに表示名を設定
                            dataGridViewNotes.Rows[0].Cells["avatar"].ToolTipText = userName;

                            // avatar列にアバターを表示
                            if (_showAvatar && user.Picture != null && user.Picture.Length > 0)
                            {
                                string avatarFile = Path.Combine(_avatarPath, $"{nostrEvent.PublicKey}.png");
                                if (!_imeStatus.Compositing && !File.Exists(avatarFile))
                                {
                                    var postBarFcuced = _formPostBar.ContainsFocus;
                                    var formSettingFocusd = _formSetting.ContainsFocus;

                                    _ = GetAvatarAsync(nostrEvent.PublicKey, user.Picture);

                                    if (postBarFcuced)
                                    {
                                        _formPostBar.Focus();
                                    }
                                    else if (formSettingFocusd)
                                    {
                                        _formSetting.Focus();
                                    }
                                    else
                                    {
                                        Focus();
                                    }
                                }

                                if (File.Exists(avatarFile))
                                {
                                    using var fileStream = new FileStream(avatarFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                                    using var avatar = new Bitmap(fileStream);
                                    dataGridViewNotes.Rows[0].Cells["avatar"].Value = new Bitmap(avatar);
                                }
                            }

                            // 背景色をリポストカラーに変更
                            dataGridViewNotes.Rows[0].DefaultCellStyle.BackColor = Tools.HexToColor(Setting.RepostColor);

                            // avastar列の背景色をpubkeyColorに変更
                            dataGridViewNotes.Rows[0].Cells["avatar"].Style.BackColor = pubkeyColor;

                            // クライアントタグによる背景色変更
                            var userClient = nostrEvent.GetTaggedData("client");
                            if (userClient != null && 0 < userClient.Length)
                            {
                                Color clientColor = Color.WhiteSmoke;

                                // userClient[0]を_clientsから検索して色を取得
                                var client = _clients.FirstOrDefault(c => c.Name == userClient[0]);
                                if (client != null && client.ColorCode != null)
                                {
                                    clientColor = Tools.HexToColor(client.ColorCode);
                                }
                                // time列の背景色をclientColorに変更
                                dataGridViewNotes.Rows[0].Cells["time"].Style.BackColor = clientColor;
                            }
                        }
                        #endregion
                    }
                }
                #endregion
            }
            else if (args.subscriptionId == NostrAccess.GetFolloweesSubscriptionId)
            {
                #region フォロイー購読
                foreach (var nostrEvent in args.events)
                {
                    // フォローリスト
                    if (3 == nostrEvent.Kind)
                    {
                        var tags = nostrEvent.Tags;
                        foreach (var tag in tags)
                        {
                            if ("p" == tag.TagIdentifier)
                            {
                                // 公開鍵をハッシュに保存
                                _followeesHexs.Add(tag.Data[0]);

                                // petnameをユーザー辞書に保存
                                if (2 < tag.Data.Count)
                                {
                                    Users.TryGetValue(tag.Data[0], out User? user);
                                    if (null != user)
                                    {
                                        user.PetName = tag.Data[2];
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion
            }
            else if (args.subscriptionId == NostrAccess.GetProfilesSubscriptionId)
            {
                #region プロフィール購読
                foreach (var nostrEvent in args.events)
                {
                    if (RemoveCompletedEventIds(nostrEvent.Id))
                    {
                        continue;
                    }

                    // プロフィール
                    if (0 == nostrEvent.Kind && null != nostrEvent.Content && null != nostrEvent.PublicKey)
                    {
                        var newUserData = Tools.JsonToUser(nostrEvent.Content, nostrEvent.CreatedAt, Notifier.Settings.MuteMostr);
                        if (null != newUserData)
                        {
                            DateTimeOffset? cratedAt = DateTimeOffset.MinValue;
                            if (Users.TryGetValue(nostrEvent.PublicKey, out User? existingUserData))
                            {
                                cratedAt = existingUserData?.CreatedAt;
                            }
                            if (false == existingUserData?.Mute)
                            {
                                // 既にミュートオフのMostrアカウントのミュートを解除
                                newUserData.Mute = false;
                            }
                            if (null == cratedAt || (cratedAt < newUserData.CreatedAt))
                            {
                                newUserData.LastActivity = DateTime.Now;
                                newUserData.PetName = existingUserData?.PetName;
                                Tools.SaveUsers(Users);
                                // 辞書に追加（上書き）
                                Users[nostrEvent.PublicKey] = newUserData;
                                Debug.WriteLine($"cratedAt updated {cratedAt} -> {newUserData.CreatedAt}");
                                Debug.WriteLine($"プロフィール更新 {newUserData.LastActivity} {newUserData.DisplayName} {newUserData.Name}");

                                if (!_imeStatus.Compositing && _showAvatar && null != newUserData.Picture)
                                {
                                    // アバター取得
                                    var postBarFcuced = _formPostBar.ContainsFocus;
                                    var formSettingFocusd = _formSetting.ContainsFocus;

                                    _ = GetAvatarAsync(nostrEvent.PublicKey, newUserData.Picture);

                                    if (postBarFcuced)
                                    {
                                        _formPostBar.Focus();
                                    }
                                    else if (formSettingFocusd)
                                    {
                                        _formSetting.Focus();
                                    }
                                    else
                                    {
                                        Focus();
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion
            }
        }
        #endregion

        #region Stopボタン
        // Stopボタン
        private void ButtonStop_Click(object sender, EventArgs e)
        {
            if (null == NostrAccess.Clients)
            {
                return;
            }

            try
            {
                NostrAccess.CloseSubscriptions();
                labelRelays.Text = "Close subscription.";

                _ = NostrAccess.Clients.Disconnect();
                labelRelays.Text = "Disconnect.";
                NostrAccess.Clients.Dispose();
                NostrAccess.Clients = null;

                buttonStart.Enabled = true;
                buttonStart.Focus();
                buttonStop.Enabled = false;
                _formPostBar.textBoxPost.Enabled = false;
                _formPostBar.buttonPost.Enabled = false;
            }
            catch (Exception ex)
            {
                Debug.Print(ex.ToString());
                labelRelays.Text = "Could not stop.";
            }
        }
        #endregion

        #region Postボタン
        // Postボタン
        internal void ButtonPost_Click(object sender, EventArgs e, NostrEvent? rootEvent)
        {
            if (0 == _formSetting.textBoxNokakoiKey.TextLength || 0 == _formSetting.textBoxPassword.TextLength)
            {
                _formPostBar.textBoxPost.PlaceholderText = "Please set nokakoi key and password.";
                return;
            }
            if (0 == _formPostBar.textBoxPost.TextLength)
            {
                _formPostBar.textBoxPost.PlaceholderText = "Cannot post empty.";
                return;
            }

            try
            {
                _ = PostAsync(rootEvent);

                _formPostBar.textBoxPost.Text = string.Empty;
                _formPostBar.textBoxPost.PlaceholderText = string.Empty;
                _formPostBar.RootEvent = null;
                // デフォルトの色に戻す
                _formPostBar.textBoxPost.BackColor = SystemColors.Window;
                //BackColor = SystemColors.Control;
                _formPostBar.buttonPost.BackColor = SystemColors.Control;
                // 送信後にチェックを外す
                checkBoxPostBar.Checked = false;
            }
            catch (Exception ex)
            {
                Debug.Print(ex.ToString());
                _formPostBar.textBoxPost.PlaceholderText = "Could not post.";
            }

            if (checkBoxPostBar.Checked)
            {
                _formPostBar.textBoxPost.Focus();
            }
        }
        #endregion

        #region 投稿処理
        /// <summary>
        /// 投稿処理
        /// </summary>
        /// <returns></returns>
        private async Task PostAsync(NostrEvent? rootEvent = null)
        {
            if (null == NostrAccess.Clients)
            {
                return;
            }
            // create tags
            List<NostrEventTag> tags = [];
            if (null != rootEvent)
            {
                tags.Add(new NostrEventTag() { TagIdentifier = "e", Data = [rootEvent.Id, string.Empty] });
                tags.Add(new NostrEventTag() { TagIdentifier = "p", Data = [rootEvent.PublicKey] });
            }
            if (_addClient)
            {
                tags.Add(new NostrEventTag()
                {
                    TagIdentifier = "client",
                    Data = ["kakoi", "31990:21ac29561b5de90cdc21995fc0707525cd78c8a52d87721ab681d3d609d1e2df:1727621066968", "wss://relay.nostr.band"]
                });
            }
            // create a new event
            var newEvent = new NostrEvent()
            {
                Kind = 1,
                Content = _formPostBar.textBoxPost.Text
                            //.Replace("\\n", "\r\n") // 本体の改行をポストバーのマルチラインに合わせる→廃止
                            .Replace("\r\n", "\n"),
                Tags = tags
            };

            try
            {
                // load from an nsec string
                var key = _nsec.FromNIP19Nsec();
                // sign the event
                await newEvent.ComputeIdAndSignAsync(key);
                // send the event
                await NostrAccess.Clients.SendEventsAndWaitUntilReceived([newEvent], CancellationToken.None);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                _formPostBar.textBoxPost.PlaceholderText = "Decryption failed.";
            }
        }
        #endregion

        #region リアクション処理
        private async Task ReactionAsync(string e, string p, string? content, string? url = null)
        {
            if (null == NostrAccess.Clients)
            {
                return;
            }
            // create tags
            List<NostrEventTag> tags = [];
            tags.Add(new NostrEventTag() { TagIdentifier = "e", Data = [e] });
            tags.Add(new NostrEventTag() { TagIdentifier = "p", Data = [p] });
            //tags.Add(new NostrEventTag() { TagIdentifier = "k", Data = ["1"] });
            if (!url.IsNullOrEmpty())
            {
                tags.Add(new NostrEventTag() { TagIdentifier = "emoji", Data = [$"{content}", $"{url}"] });
                content = $":{content}:";
            }
            if (_addClient)
            {
                tags.Add(new NostrEventTag() { TagIdentifier = "client", Data = ["kakoi"] });
            }
            // create a new event
            var newEvent = new NostrEvent()
            {
                Kind = 7,
                Content = content,
                Tags = tags
            };

            try
            {
                // load from an nsec string
                var key = _nsec.FromNIP19Nsec();
                // sign the event
                await newEvent.ComputeIdAndSignAsync(key);
                // send the event
                await NostrAccess.Clients.SendEventsAndWaitUntilReceived([newEvent], CancellationToken.None);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                labelRelays.Text = "Decryption failed.";
            }
        }
        #endregion

        #region Settingボタン
        // Settingボタン
        private async void ButtonSetting_Click(object sender, EventArgs e)
        {
            // 開く前
            Opacity = _tempOpacity;
            _formSetting.checkBoxTopMost.Checked = TopMost;
            _formSetting.trackBarOpacity.Value = (int)(Opacity * 100);
            _formSetting.checkBoxShowAvatar.Checked = _showAvatar;
            _formSetting.checkBoxShowOnlyJapanese.Checked = _showOnlyJapanese;
            _formSetting.checkBoxShowOnlyFollowees.Checked = _showOnlyFollowees;
            _formSetting.textBoxNokakoiKey.Text = _nokakoiKey;
            _formSetting.textBoxPassword.Text = _password;
            _formSetting.checkBoxSendDSSTP.Checked = _sendDSSTP;
            _formSetting.checkBoxAddClient.Checked = _addClient;

            // 開く
            _formSetting.ShowDialog(this);

            // 閉じた後
            TopMost = _formSetting.checkBoxTopMost.Checked;
            Opacity = _formSetting.trackBarOpacity.Value / 100.0;
            _tempOpacity = Opacity;
            _formPostBar.Opacity = Opacity;
            _showAvatar = _formSetting.checkBoxShowAvatar.Checked;
            //dataGridViewNotes.Columns["avatar"].Visible = _showAvatar;
            _showOnlyFollowees = _formSetting.checkBoxShowOnlyFollowees.Checked;
            _showOnlyJapanese = _formSetting.checkBoxShowOnlyJapanese.Checked;
            _nokakoiKey = _formSetting.textBoxNokakoiKey.Text;
            _password = _formSetting.textBoxPassword.Text;
            _sendDSSTP = _formSetting.checkBoxSendDSSTP.Checked;
            _addClient = _formSetting.checkBoxAddClient.Checked;
            try
            {
                // 別アカウントログイン失敗に備えてクリアしておく
                _nsec = string.Empty;
                _npubHex = string.Empty;
                _followeesHexs.Clear();
                _formPostBar.textBoxPost.PlaceholderText = "kakoi";

                // 秘密鍵と公開鍵取得
                _nsec = NokakoiCrypt.DecryptNokakoiKey(_nokakoiKey, _password);
                _npubHex = _nsec.GetNpubHex();

                // ログイン済みの時
                if (!_npubHex.IsNullOrEmpty())
                {
                    int connectCount = await NostrAccess.ConnectAsync();
                    switch (connectCount)
                    {
                        case 0:
                            labelRelays.Text = "No relay enabled.";
                            toolTipRelays.SetToolTip(labelRelays, string.Empty);
                            break;
                        case 1:
                            labelRelays.Text = NostrAccess.Relays[0].ToString();
                            toolTipRelays.SetToolTip(labelRelays, string.Join("\n", NostrAccess.Relays.Select(r => r.ToString())));
                            break;
                        default:
                            labelRelays.Text = $"{NostrAccess.Relays.Length} relays";
                            toolTipRelays.SetToolTip(labelRelays, string.Join("\n", NostrAccess.Relays.Select(r => r.ToString())));
                            break;
                    }
                    if (0 == connectCount)
                    {
                        return;
                    }

                    // フォロイーを購読をする
                    NostrAccess.SubscribeFollows(_npubHex);

                    // ログインユーザー表示名取得
                    var name = GetUserName(_npubHex);
                    //_formPostBar.textBoxPost.PlaceholderText = $"Post as {name}";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                labelRelays.Text = "Decryption failed.";
            }

            Setting.TopMost = TopMost;
            Setting.Opacity = Opacity;
            Setting.ShowAvatar = _showAvatar;
            Setting.ShowOnlyFollowees = _showOnlyFollowees;
            Setting.ShowOnlyJapanese = _showOnlyJapanese;
            Setting.NokakoiKey = _nokakoiKey;
            Setting.SendDSSTP = _sendDSSTP;
            Setting.AddClient = _addClient;

            Setting.Save(_configPath);
            _emojis = Tools.LoadEmojis();
            comboBoxEmoji.DataSource = _emojis;
            _clients = Tools.LoadClients();

            dataGridViewNotes.Focus();
        }
        #endregion

        #region 複数リレーからの処理済みイベントを除外
        /// <summary>
        /// 複数リレーからの処理済みイベントを除外
        /// </summary>
        /// <param name="eventId"></param>
        private bool RemoveCompletedEventIds(string eventId)
        {
            if (_displayedEventIds.Contains(eventId))
            {
                return true;
            }
            if (_displayedEventIds.Count >= 128)
            {
                _displayedEventIds.RemoveFirst();
            }
            _displayedEventIds.AddLast(eventId);
            return false;
        }
        #endregion

        #region 透明解除処理
        // マウス入った時
        private void Control_MouseEnter(object sender, EventArgs e)
        {
            _tempOpacity = Opacity;
            Opacity = 1.00;
        }

        // マウス出た時
        private void Control_MouseLeave(object sender, EventArgs e)
        {
            Opacity = _tempOpacity;
        }
        #endregion

        #region SSPゴースト名を取得する
        /// <summary>
        /// SSPゴースト名を取得する
        /// </summary>
        private void SearchGhost()
        {
            _ds.Update();
            SakuraFMO fmo = (SakuraFMO)_ds.FMO;
            var names = fmo.GetGhostNames();
            if (names.Length > 0)
            {
                _ghostName = names.First(); // とりあえず先頭で
                //Debug.Print(_ghostName);
            }
            else
            {
                _ghostName = string.Empty;
                //Debug.Print("ゴーストがいません");
            }
        }
        #endregion

        #region 言語判定
        /// <summary>
        /// 言語判定
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static string DetermineLanguage(string text)
        {
            var factory = new RankedLanguageIdentifierFactory();
            RankedLanguageIdentifier identifier;
            try
            {
                identifier = factory.Load("Core14.profile.xml");
            }
            catch (Exception)
            {
                return string.Empty;
            }
            var languages = identifier.Identify(text);
            var mostCertainLanguage = languages.FirstOrDefault();
            if (mostCertainLanguage != null)
            {
                return mostCertainLanguage.Item1.Iso639_3;
            }
            else
            {
                return string.Empty;
            }
        }
        #endregion

        #region ユーザー表示名を取得する
        /// <summary>
        /// ユーザー表示名を取得する
        /// </summary>
        /// <param name="publicKeyHex">公開鍵HEX</param>
        /// <returns>ユーザー表示名</returns>
        private string GetUserName(string publicKeyHex)
        {
            //// 辞書にない場合プロフィールを購読する
            //if (!_users.TryGetValue(publicKeyHex, out User? user))
            //{
            //    SubscribeProfiles([publicKeyHex]);
            //}
            // kind 0 を毎回購読するように変更（頻繁にdisplay_name等を変更するユーザーがいるため）
            NostrAccess.SubscribeProfiles([publicKeyHex]);

            // 情報があれば表示名を取得
            Users.TryGetValue(publicKeyHex, out User? user);
            string? userName = "???";
            if (null != user)
            {
                userName = user.DisplayName;
                // display_nameが無い場合は@nameとする
                if (null == userName || string.Empty == userName)
                {
                    //userName = $"@{user.Name}";
                    userName = $"{user.Name}";
                }
                // petnameがある場合は📛petnameとする
                if (!user.PetName.IsNullOrEmpty())
                {
                    //userName = $"📛{user.PetName}";
                    userName = $"{user.PetName}";
                }
                // 取得日更新
                user.LastActivity = DateTime.Now;
                Tools.SaveUsers(Users);
                Debug.WriteLine($"ユーザー名取得 {user.LastActivity} {user.DisplayName} {user.Name}");
            }
            return userName;
        }
        #endregion

        #region ミュートされているか確認する
        /// <summary>
        /// ミュートされているか確認する
        /// </summary>
        /// <param name="publicKeyHex">公開鍵HEX</param>
        /// <returns>ミュートフラグ</returns>
        private bool IsMuted(string publicKeyHex)
        {
            if (Users.TryGetValue(publicKeyHex, out User? user))
            {
                if (null != user)
                {
                    return user.Mute;
                }
            }
            return false;
        }
        #endregion

        #region 閉じる
        // 閉じる
        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // ホットキーの登録を解除
            UnregisterHotKey(this.Handle, HOTKEY_ID);

            NostrAccess.CloseSubscriptions();
            NostrAccess.DisconnectAndDispose();

            if (FormWindowState.Normal != WindowState)
            {
                // 最小化最大化状態の時、元の位置と大きさを保存
                Setting.Location = RestoreBounds.Location;
                Setting.Size = RestoreBounds.Size;
            }
            else
            {
                Setting.Location = Location;
                Setting.Size = Size;
            }
            Setting.PostBarLocation = _formPostBar.Location;
            Setting.PostBarSize = _formPostBar.Size;
            if (FormWindowState.Normal != _formWeb.WindowState)
            {
                // 最小化最大化状態の時、元の位置と大きさを保存
                Setting.WebViewLocation = _formWeb.RestoreBounds.Location;
                Setting.WebViewSize = _formWeb.RestoreBounds.Size;
            }
            else
            {
                Setting.WebViewLocation = _formWeb.Location;
                Setting.WebViewSize = _formWeb.Size;
            }
            Setting.NameColumnWidth = dataGridViewNotes.Columns["name"].Width;
            Setting.Save(_configPath);
            Tools.SaveUsers(Users);

            _ds?.Dispose();      // FrmMsgReceiverのThread停止せず1000ms待たされるうえにプロセス残るので…
            Application.Exit(); // ←これで殺す。SSTLibに手を入れた方がいいが、とりあえず。
        }
        #endregion

        #region ロード時
        // ロード時
        private void FormMain_Load(object sender, EventArgs e)
        {
            // Ctrl + Shift + A をホットキーとして登録
            RegisterHotKey(this.Handle, HOTKEY_ID, MOD_CONTROL | MOD_SHIFT, (int)Keys.A);

            _formPostBar.ShowDialog();
            ButtonStart_Click(sender, e);
        }
        #endregion

        #region ポストバー表示切り替え
        // ポストバー表示切り替え
        private void CheckBoxPostBar_CheckedChanged(object sender, EventArgs e)
        {
            _formPostBar.Visible = checkBoxPostBar.Checked;
            _formPostBar.textBoxPost.Focus();
        }
        #endregion

        #region 画面表示切替
        // 画面表示切替
        private void FormMain_KeyDown(object sender, KeyEventArgs e)
        {
            // ポストバー表示切替
            if (e.KeyCode == Keys.F11 || e.KeyCode == Keys.F12 || e.KeyCode == Keys.F1)
            {
                checkBoxPostBar.Checked = !checkBoxPostBar.Checked;
            }
            // F2キーでtime列の表示切替
            if (e.KeyCode == Keys.F2)
            {
                dataGridViewNotes.Columns["time"].Visible = !dataGridViewNotes.Columns["time"].Visible;
            }
            // F3キーでavatar列の表示切替
            if (e.KeyCode == Keys.F3)
            {
                dataGridViewNotes.Columns["avatar"].Visible = !dataGridViewNotes.Columns["avatar"].Visible;
            }
            // F4キーでname列の表示切替
            if (e.KeyCode == Keys.F4)
            {
                dataGridViewNotes.Columns["name"].Visible = !dataGridViewNotes.Columns["name"].Visible;
            }

            if (e.KeyCode == Keys.Escape)
            {
                ButtonSetting_Click(sender, e);
            }

            if (e.KeyCode == Keys.F10)
            {
                var ev = new MouseEventArgs(MouseButtons.Right, 1, 0, 0, 0);
                FormMain_MouseClick(sender, ev);
            }

            if (e.KeyCode == Keys.F9)
            {
                var ev = new MouseEventArgs(MouseButtons.Left, 2, 0, 0, 0);
                FormMain_MouseDoubleClick(sender, ev);
            }
        }
        #endregion

        #region マニアクス表示
        private void FormMain_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (null == _formManiacs || _formManiacs.IsDisposed)
                {
                    _formManiacs = new FormManiacs
                    {
                        MainForm = this
                    };
                }
                if (!_formManiacs.Visible)
                {
                    _formManiacs.Show(this);
                }
            }
        }
        #endregion

        #region リレーリスト表示
        private void ButtonRelayList_Click(object sender, EventArgs e)
        {
            _formRelayList = new FormRelayList();
            if (_formRelayList.ShowDialog(this) == DialogResult.OK)
            {
                ButtonStop_Click(sender, e);
                ButtonStart_Click(sender, e);
            }
            _formRelayList.Dispose();
        }
        #endregion

        #region セルダブルクリック
        private void DataGridViewNotes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // マウスカーソルがデフォルトじゃない時は無視
            if (Cursor.Current != Cursors.Default)
            {
                return;
            }
            // ヘッダー行がダブルクリックされた場合は無視
            if (e.RowIndex < 0)
            {
                return;
            }
            DataGridViewRow selectedRow = dataGridViewNotes.Rows[e.RowIndex];
            string id = (string)selectedRow.Cells["id"].Value;
            string pubkey = (string)selectedRow.Cells["pubkey"].Value;

            if (comboBoxEmoji.SelectedItem is Emoji emoji)
            {
                var content = emoji.Content;
                var url = emoji.Url;

                _ = ReactionAsync(id, pubkey, content, url);
            }
        }
        #endregion

        #region グリッドキー入力
        private void DataGridViewNotes_KeyDown(object sender, KeyEventArgs e)
        {
            // Cキーで_formWebを閉じる
            if (e.KeyCode == Keys.C)
            {
                if (null != _formWeb && !_formWeb.IsDisposed)
                {
                    _formWeb.Close();
                }
            }
            // Wキーで選択行を上に
            if (e.KeyCode == Keys.W)
            {
                if (dataGridViewNotes.SelectedRows[0].Index > 0)
                {
                    dataGridViewNotes.Rows[dataGridViewNotes.SelectedRows[0].Index - 1].Selected = true;
                    dataGridViewNotes.CurrentCell = dataGridViewNotes["note", dataGridViewNotes.SelectedRows[0].Index];
                }
            }
            // Sキーで選択行を下に
            if (e.KeyCode == Keys.S)
            {
                if (dataGridViewNotes.SelectedRows[0].Index < dataGridViewNotes.Rows.Count - 1)
                {
                    dataGridViewNotes.Rows[dataGridViewNotes.SelectedRows[0].Index + 1].Selected = true;
                    dataGridViewNotes.CurrentCell = dataGridViewNotes["note", dataGridViewNotes.SelectedRows[0].Index];
                }
            }
            // Shift + Wキーで選択行を最上部に
            if (e.KeyCode == Keys.W && e.Shift)
            {
                dataGridViewNotes.Rows[0].Selected = true;
                dataGridViewNotes.CurrentCell = dataGridViewNotes["note", 0];
            }
            // Shift + Sキーで選択行を最下部に
            if (e.KeyCode == Keys.S && e.Shift)
            {
                dataGridViewNotes.Rows[dataGridViewNotes.Rows.Count - 1].Selected = true;
                dataGridViewNotes.CurrentCell = dataGridViewNotes["note", dataGridViewNotes.Rows.Count - 1];
            }
            // リアクション
            if (e.KeyCode == Keys.Right || e.KeyCode == Keys.F)
            {
                // 画面外に出た時サイズ変更用カーソルを記憶しているのでデフォルトに戻す
                Cursor.Current = Cursors.Default;
                var ev = new DataGridViewCellEventArgs(3, dataGridViewNotes.SelectedRows[0].Index);
                DataGridViewNotes_CellDoubleClick(sender, ev);
            }
            // Webビュー表示
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.A)
            {
                var mev = new MouseEventArgs(MouseButtons.Right, 1, 0, 0, 0);
                var ev = new DataGridViewCellMouseEventArgs(0, dataGridViewNotes.SelectedRows[0].Index, 0, 0, mev);
                DataGridViewNotes_CellMouseClick(sender, ev);
            }
            // 数字キーで絵文字選択
            if (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
            {
                int index = e.KeyCode - Keys.D0 - 1;
                if (e.KeyCode == Keys.D0)
                {
                    index = 10 - 1;
                }
                if (index < comboBoxEmoji.Items.Count)
                {
                    comboBoxEmoji.SelectedIndex = index;
                    //var ev = new DataGridViewCellEventArgs(3, dataGridViewNotes.SelectedRows[0].Index);
                    //DataGridViewNotes_CellDoubleClick(sender, ev);
                }
            }
            // Rキーで_postBar.RootEventに選択行のイベントを設定
            if (e.KeyCode == Keys.R)
            {
                if (dataGridViewNotes.SelectedRows[0].Index >= 0)
                {
                    var rootEvent = new NostrEvent()
                    {
                        Id = (string)dataGridViewNotes.Rows[dataGridViewNotes.SelectedRows[0].Index].Cells["id"].Value,
                        PublicKey = (string)dataGridViewNotes.Rows[dataGridViewNotes.SelectedRows[0].Index].Cells["pubkey"].Value
                    };
                    _formPostBar.RootEvent = rootEvent;
                    _formPostBar.textBoxPost.PlaceholderText = $"Reply to {GetUserName(rootEvent.PublicKey)}";
                    if (!checkBoxPostBar.Checked)
                    {
                        checkBoxPostBar.Checked = true;
                    }
                    _formPostBar.Focus();
                }
            }

        }
        #endregion

        #region フォームマウスダブルクリック
        private void FormMain_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (dataGridViewNotes.Columns["note"].DefaultCellStyle.WrapMode != DataGridViewTriState.True)
            {
                dataGridViewNotes.Columns["note"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            }
            else
            {
                dataGridViewNotes.Columns["note"].DefaultCellStyle.WrapMode = DataGridViewTriState.NotSet;
            }
        }
        #endregion

        #region セル右クリック
        private void DataGridViewNotes_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                dataGridViewNotes.Rows[e.RowIndex].Selected = true;
                dataGridViewNotes.Rows[e.RowIndex].Cells["note"].Selected = true;

                if (null == _formWeb || _formWeb.IsDisposed)
                {
                    _formWeb = new FormWeb();
                }
                if (!_formWeb.Visible)
                {
                    _formWeb.Location = _formWebLocation;
                    _formWeb.Size = _formWebSize;
                    _formWeb.Show(this);
                }
                if (_formWeb.WindowState == FormWindowState.Minimized)
                {
                    _formWeb.WindowState = FormWindowState.Normal;
                }
                Setting.WebViewLocation = _formWeb.Location;
                Setting.WebViewSize = _formWeb.Size;

                var id = dataGridViewNotes.Rows[e.RowIndex].Cells["id"].Value.ToString() ?? string.Empty;
                NIP19.NostrEventNote nostrEventNote = new()
                {
                    EventId = id,
                    Relays = [string.Empty],
                };
                var nevent = nostrEventNote.ToNIP19();
                try
                {
                    _formWeb.webView2.Source = new Uri(Setting.WebViewUrl + nevent);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    _formWeb.Close();
                }
                Focus();
            }
        }
        #endregion

        #region avatar取得
        private static async Task GetAvatarAsync(string publicKeyHex, string avatarUrl)
        {
            string picturePath = Path.Combine(new FormMain()._avatarPath, $"{publicKeyHex}.png");
            using HttpClient httpClient = new();
            httpClient.Timeout = TimeSpan.FromSeconds(5);   // タイムアウト5秒
            SKBitmap bitmap = new();
            try
            {
                if (Path.GetExtension(avatarUrl).Equals(".svg", StringComparison.OrdinalIgnoreCase))
                {
                    // SVGファイルの場合
                    Debug.WriteLine("svg画像処理開始");
                    // URLからSVGデータをダウンロード
                    using var svgData = await httpClient.GetStreamAsync(avatarUrl);
                    // SVGデータを読み込む
                    using var svg = new SKSvg();
                    svg.Load(svgData);
                    if (null != svg.Picture)
                    {
                        bitmap = new SKBitmap((int)svg.Picture.CullRect.Width, (int)svg.Picture.CullRect.Height);
                        using var canvas = new SKCanvas(bitmap);
                        canvas.DrawPicture(svg.Picture);
                        canvas.Flush();
                    }
                }
                else
                {
                    // URLから画像データを取得
                    var avatarBytes = await httpClient.GetByteArrayAsync(avatarUrl);
                    // バイト配列をMemoryStreamに変換
                    using MemoryStream ms = new(avatarBytes);
                    // MemoryStreamから画像を読み込む
                    bitmap = SKBitmap.Decode(ms);
                }
                // リサイズ
                using (var resizedBitmap = bitmap?.Resize(new SKImageInfo(_avatarSize, _avatarSize), SKFilterQuality.High))
                {
                    // 画像をPNG形式で保存
                    using SKImage image = SKImage.FromBitmap(resizedBitmap);
                    using SKData data = image.Encode(SKEncodedImageFormat.Png, 100);
                    using FileStream fs = File.OpenWrite(picturePath);
                    data.SaveTo(fs);
                }

                Debug.WriteLine("画像が正常に保存されました。");
            }
            catch (TaskCanceledException ex)
            {
                Debug.WriteLine($"タイムアウトしました: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"エラーが発生しました: {ex.Message}");
            }
            finally
            {
                if (null != bitmap)
                {
                    bitmap.Dispose();
                }
            }
        }
        #endregion

        private void FormMain_Shown(object sender, EventArgs e)
        {
            dataGridViewNotes.Focus();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == HOTKEY_ID)
            {
                this.Activate(); // FormMainをアクティブにする
            }
            base.WndProc(ref m);
        }

        private void comboBoxEmoji_SelectionChangeCommitted(object sender, EventArgs e)
        {
            dataGridViewNotes.Focus();
        }
    }
}
