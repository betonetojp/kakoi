using kakoi.Properties;
using NNostr.Client;
using NNostr.Client.Protocols;
using nokakoiCrypt;
using NTextCat;
using NTextCat.Commons;
using SSTPLib;
using System.Diagnostics;

namespace kakoi
{
    public partial class FormMain : Form
    {
        #region フィールド
        private readonly NostrAccess _nostrAccess = new();

        private readonly string _configPath = Path.Combine(Application.StartupPath, "kakoi.config");

        private readonly FormSetting _formSetting = new();
        private readonly FormPostBar _formPostBar = new();
        private FormManiacs _formManiacs = new();
        private FormRelayList _formRelayList = new();
        private FormWeb _formWeb = new();

        private string _nsec = string.Empty;
        //private string _npub = string.Empty;
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

        private int _cutLength;
        private int _cutNameLength;
        private bool _addClient;
        private bool _showOnlyJapanese;
        private bool _showOnlyFollowees;
        private string _nokakoiKey = string.Empty;
        private bool _sendDSSTP = true;
        private string _password = string.Empty;

        private double _tempOpacity = 1.00;

        private readonly DSSTPSender _ds = new("SakuraUnicode");
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
                buttonPost.Image = new Bitmap(Resources.icons8_create_16, size, size);
                buttonSetting.Image = new Bitmap(Resources.icons8_setting_16, size, size);
            }
            else
            {
                buttonRelayList.Image = new Bitmap(Resources.icons8_list_32, size, size);
                buttonStart.Image = new Bitmap(Resources.icons8_start_32, size, size);
                buttonStop.Image = new Bitmap(Resources.icons8_stop_32, size, size);
                buttonPost.Image = new Bitmap(Resources.icons8_create_32, size, size);
                buttonSetting.Image = new Bitmap(Resources.icons8_setting_32, size, size);
            }

            Setting.Load(_configPath);
            Users = Tools.LoadUsers();
            _emojis = Tools.LoadEmojis();
            comboBoxEmoji.DataSource = _emojis;

            Location = Setting.Location;
            if (new Point(0, 0) == Location || Location.X < 0 || Location.Y < 0)
            {
                StartPosition = FormStartPosition.CenterScreen;
            }
            Size = Setting.Size;
            TopMost = Setting.TopMost;
            _cutLength = Setting.CutLength;
            _cutNameLength = Setting.CutNameLength;
            Opacity = Setting.Opacity;
            if (0 == Opacity)
            {
                Opacity = 1;
            }
            _tempOpacity = Opacity;
            _formPostBar.Opacity = Opacity;
            _formWeb.Opacity = Opacity;
            _addClient = Setting.AddClient;
            _showOnlyJapanese = Setting.ShowOnlyJapanese;
            _showOnlyFollowees = Setting.ShowOnlyFollowees;
            _nokakoiKey = Setting.NokakoiKey;
            _sendDSSTP = Setting.SendDSSTP;
            _formPostBar.Location = Setting.PostBarLocation;
            if (new Point(0, 0) == _formPostBar.Location || _formPostBar.Location.X < 0 || _formPostBar.Location.Y < 0)
            {
                _formPostBar.StartPosition = FormStartPosition.CenterScreen;
            }
            _formWeb.Location = Setting.WebLocation;
            if (new Point(0, 0) == _formWeb.Location || _formWeb.Location.X < 0 || _formWeb.Location.Y < 0)
            {
                _formWeb.StartPosition = FormStartPosition.CenterScreen;
            }
            _formPostBar.Size = Setting.PostBarSize;
            _formWeb.Size = Setting.WebSize;
            dataGridViewNotes.Columns["name"].Width = Setting.NameColumnWidth;
            dataGridViewNotes.GridColor = Tools.HexToColor(Setting.GridColor);
            dataGridViewNotes.DefaultCellStyle.SelectionBackColor = Tools.HexToColor(Setting.GridColor);

            _formSetting.PostBarForm = _formPostBar;
            _formSetting.WebForm = _formWeb;
            _formPostBar.MainForm = this;
            _formManiacs.MainForm = this;
        }
        #endregion

        #region Startボタン
        // Startボタン
        private async void ButtonStart_Click(object sender, EventArgs e)
        {
            try
            {
                int connectCount;
                if (null != _nostrAccess.Clients)
                {
                    connectCount = await _nostrAccess.ConnectAsync();
                }
                else
                {
                    connectCount = await _nostrAccess.ConnectAsync();
                    switch (connectCount)
                    {
                        case 0:
                            labelRelays.Text = "0 relays";
                            toolTipRelays.SetToolTip(labelRelays, string.Empty);
                            break;
                        case 1:
                            labelRelays.Text = _nostrAccess.Relays[0].ToString();
                            toolTipRelays.SetToolTip(labelRelays, string.Join("\n", _nostrAccess.Relays.Select(r => r.ToString())));
                            break;
                        default:
                            labelRelays.Text = $"{_nostrAccess.Relays.Length} relays";
                            toolTipRelays.SetToolTip(labelRelays, string.Join("\n", _nostrAccess.Relays.Select(r => r.ToString())));
                            break;
                    }
                    if (null != _nostrAccess.Clients)
                    {
                        _nostrAccess.Clients.EventsReceived += OnClientOnEventsReceived;
                    }
                }

                if (0 == connectCount)
                {
                    textBoxPost.PlaceholderText = "> No relay enabled.";
                    return;
                }

                textBoxPost.PlaceholderText = "> Connect.";

                _nostrAccess.Subscribe();

                buttonStart.Enabled = false;
                buttonStop.Enabled = true;
                buttonStop.Focus();
                textBoxPost.Enabled = true;
                buttonPost.Enabled = true;
                _formPostBar.textBoxPost.Enabled = true;
                _formPostBar.buttonPost.Enabled = true;
                textBoxPost.PlaceholderText = "> Create subscription.";

                // ログイン済みの時
                if (!string.IsNullOrEmpty(_npubHex))
                {
                    // フォロイーを購読をする
                    _nostrAccess.SubscribeFollows(_npubHex);

                    // ログインユーザー表示名取得
                    var name = GetUserName(_npubHex);
                    textBoxPost.PlaceholderText = $"> Login as {name}.";
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.ToString());
                textBoxPost.PlaceholderText = "> Could not start.";
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
            // タイムライン購読
            if (args.subscriptionId == _nostrAccess.SubscriptionId)
            {
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

                        // フォロイーチェック
                        string headMark = "-";
                        string speaker = "\\1"; //"\\u\\p[1]\\s[10]";
                        if (_followeesHexs.Contains(nostrEvent.PublicKey))
                        {
                            headMark = "*";
                            // 本体側がしゃべる
                            speaker = "\\0"; //"\\h\\p[0]\\s[0]";
                        }

                        // リアクション
                        if (7 == nostrEvent.Kind)
                        {
                            // ログイン済みで自分へのリアクション
                            if (!_npubHex.IsNullOrEmpty() && nostrEvent.GetTaggedPublicKeys().Contains(_npubHex))
                            {
                                Users.TryGetValue(nostrEvent.PublicKey, out User? user);

                                // ユーザー表示名取得
                                string userName = GetUserName(nostrEvent.PublicKey);

                                headMark = "+";

                                // グリッドに表示
                                //_noteEvents.AddFirst(nostrEvent);
                                DateTimeOffset dto = nostrEvent.CreatedAt ?? DateTimeOffset.Now;
                                dataGridViewNotes.Rows.Insert(
                                0,
                                dto.ToLocalTime(),
                                $"{headMark} {userName}",
                                nostrEvent.Content,
                                nostrEvent.Id,
                                nostrEvent.PublicKey
                                );

                                // ユーザー表示名カット
                                if (userName.Length > _cutNameLength)
                                {
                                    userName = $"{userName[.._cutNameLength]}...";
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
                                // 画面に表示
                                textBoxPost.PlaceholderText = $"{timeString} {headMark} {userName} {content}";
                            }
                        }
                        // テキストノート
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
                                headMark = "<";
                            }

                            // グリッドに表示
                            //_noteEvents.AddFirst(nostrEvent);
                            DateTimeOffset dto = nostrEvent.CreatedAt ?? DateTimeOffset.Now;
                            dataGridViewNotes.Rows.Insert(
                                0,
                                dto.ToLocalTime(),
                                $"{headMark} {userName}",
                                nostrEvent.Content,
                                nostrEvent.Id,
                                nostrEvent.PublicKey
                                );
                            //dataGridViewNotes.Sort(dataGridViewNotes.Columns["time"], ListSortDirection.Descending);

                            // クライアントタグによる背景色変更のテスト
                            var userClient = nostrEvent.GetTaggedData("client");
                            if (userClient != null && 0 < userClient.Length)
                            {
                                Color clientColor = Color.Silver;
                                if (-1 < Array.IndexOf(userClient, "kakoi"))
                                {
                                    clientColor = Color.HotPink;
                                }
                                else if (-1 < Array.IndexOf(userClient, "lumilumi"))
                                {
                                    clientColor = Color.Orange;
                                }
                                else if (-1 < Array.IndexOf(userClient, "noStrudel"))
                                {
                                    clientColor = Color.YellowGreen;
                                }
                                dataGridViewNotes.Rows[0].DefaultCellStyle.BackColor = clientColor;
                            }

                            //foreach (var tag in nostrEvent.Tags)
                            //{
                            //    // eタグ、pタグがある時は背景色を変える
                            //    if (tag.TagIdentifier == "e" || tag.TagIdentifier == "p")
                            //    {
                            //        dataGridViewNotes.Rows[0].DefaultCellStyle.BackColor = Color.Lavender;
                            //        continue;
                            //    }
                            //}
                            // e,p,qタグがある時は背景色を変える
                            if (isReply)
                            {
                                dataGridViewNotes.Rows[0].DefaultCellStyle.BackColor = Color.Lavender;
                            }

                            // ユーザー表示名カット
                            if (userName.Length > _cutNameLength)
                            {
                                userName = $"{userName[.._cutNameLength]}...";
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
                                // 本文カット
                                if (msg.Length > _cutLength)
                                {
                                    msg = $"{msg[.._cutLength]}...";//\\u\\p[1]\\s[10]長いよっ！";
                                }
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
                            // 本文カット
                            if (content.Length > _cutLength)
                            {
                                content = $"{content[.._cutLength]}...";
                            }
                            Debug.WriteLine($"{timeString} {userName} {content}");
                        }
                        // リポスト
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

                            headMark = ">";

                            // グリッドに表示
                            DateTimeOffset dto = nostrEvent.CreatedAt ?? DateTimeOffset.Now;
                            dataGridViewNotes.Rows.Insert(
                            0,
                            dto.ToLocalTime(),
                            $"{headMark} {userName}",
                            $"reposted {GetUserName(nostrEvent.GetTaggedPublicKeys()[0])}'s post.",
                            nostrEvent.Id,
                            nostrEvent.PublicKey
                            );
                            dataGridViewNotes.Rows[0].DefaultCellStyle.BackColor = Color.AliceBlue;
                        }
                    }
                }
            }
            // フォロイー購読
            else if (args.subscriptionId == _nostrAccess.GetFolloweesSubscriptionId)
            {
                foreach (var nostrEvent in args.events)
                {
                    // フォローリスト
                    if (3 == nostrEvent.Kind)
                    {
                        var tags = nostrEvent.Tags;
                        foreach (var tag in tags)
                        {
                            // 公開鍵を保存
                            if ("p" == tag.TagIdentifier)
                            {
                                // 先頭を公開鍵と決めつけているが…
                                _followeesHexs.Add(tag.Data[0]);
                            }
                        }
                    }
                }
            }
            // プロフィール購読
            else if (args.subscriptionId == _nostrAccess.GetProfilesSubscriptionId)
            {
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
                                Tools.SaveUsers(Users);
                                // 辞書に追加（上書き）
                                Users[nostrEvent.PublicKey] = newUserData;
                                Debug.WriteLine($"cratedAt updated {cratedAt} -> {newUserData.CreatedAt}");
                                Debug.WriteLine($"プロフィール更新 {newUserData.LastActivity} {newUserData.DisplayName} {newUserData.Name}");
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Stopボタン
        // Stopボタン
        private void ButtonStop_Click(object sender, EventArgs e)
        {
            if (null == _nostrAccess.Clients)
            {
                return;
            }

            try
            {
                _nostrAccess.CloseSubscriptions();
                textBoxPost.PlaceholderText = "> Close subscription.";

                _ = _nostrAccess.Clients.Disconnect();
                textBoxPost.PlaceholderText = "> Disconnect.";
                _nostrAccess.Clients.Dispose();
                _nostrAccess.Clients = null;

                buttonStart.Enabled = true;
                buttonStart.Focus();
                buttonStop.Enabled = false;
                textBoxPost.Enabled = false;
                buttonPost.Enabled = false;
                _formPostBar.textBoxPost.Enabled = false;
                _formPostBar.buttonPost.Enabled = false;
            }
            catch (Exception ex)
            {
                Debug.Print(ex.ToString());
                textBoxPost.PlaceholderText = "> Could not stop.";
            }
        }
        #endregion

        #region Postボタン
        // Postボタン
        internal void ButtonPost_Click(object sender, EventArgs e)
        {
            if (0 == _formSetting.textBoxNokakoiKey.TextLength || 0 == _formSetting.textBoxPassword.TextLength)
            {
                textBoxPost.PlaceholderText = "> Please set nokakoi key and password.";
                return;
            }
            if (0 == textBoxPost.TextLength)
            {
                textBoxPost.PlaceholderText = "> Cannot post empty.";
                return;
            }

            try
            {
                _ = PostAsync();

                textBoxPost.Text = string.Empty;
                _formPostBar.textBoxPost.Text = string.Empty;
            }
            catch (Exception ex)
            {
                Debug.Print(ex.ToString());
                textBoxPost.PlaceholderText = "> Could not post.";
            }

            if (checkBoxPostBar.Checked)
            {
                _formPostBar.textBoxPost.Focus();
            }
            else
            {
                textBoxPost.Focus();
            }
        }
        #endregion

        #region 投稿処理
        /// <summary>
        /// 投稿処理
        /// </summary>
        /// <returns></returns>
        private async Task PostAsync()
        {
            if (null == _nostrAccess.Clients)
            {
                return;
            }
            // create tags
            List<NostrEventTag> tags = [];
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
                Content = textBoxPost.Text
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
                await _nostrAccess.Clients.SendEventsAndWaitUntilReceived([newEvent], CancellationToken.None);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                textBoxPost.PlaceholderText = "> Decryption failed.";
            }
        }
        #endregion

        #region リアクション処理
        private async Task ReactionAsync(string e, string p, string? content, string? url = null)
        {
            if (null == _nostrAccess.Clients)
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
                await _nostrAccess.Clients.SendEventsAndWaitUntilReceived([newEvent], CancellationToken.None);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                textBoxPost.PlaceholderText = "> Decryption failed.";
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
            _formSetting.textBoxCutLength.Text = _cutLength.ToString();
            _formSetting.textBoxCutNameLength.Text = _cutNameLength.ToString();
            _formSetting.trackBarOpacity.Value = (int)(Opacity * 100);
            _formSetting.checkBoxAddClient.Checked = _addClient;
            _formSetting.checkBoxShowOnlyJapanese.Checked = _showOnlyJapanese;
            _formSetting.checkBoxShowOnlyFollowees.Checked = _showOnlyFollowees;
            _formSetting.textBoxNokakoiKey.Text = _nokakoiKey;
            _formSetting.checkBoxSendDSSTP.Checked = _sendDSSTP;
            _formSetting.textBoxPassword.Text = _password;
            _formSetting.WebForm = _formWeb;

            // 開く
            _formSetting.ShowDialog(this);

            // 閉じた後
            TopMost = _formSetting.checkBoxTopMost.Checked;
            if (!int.TryParse(_formSetting.textBoxCutLength.Text, out _cutLength))
            {
                _cutLength = 40;
            }
            else if (_cutLength < 1)
            {
                _cutLength = 1;
            }
            if (!int.TryParse(_formSetting.textBoxCutNameLength.Text, out _cutNameLength))
            {
                _cutNameLength = 8;
            }
            else if (_cutNameLength < 1)
            {
                _cutNameLength = 1;
            }
            Opacity = _formSetting.trackBarOpacity.Value / 100.0;
            _tempOpacity = Opacity;
            _formPostBar.Opacity = Opacity;
            _formWeb.Opacity = Setting.Opacity;
            _addClient = _formSetting.checkBoxAddClient.Checked;
            _showOnlyJapanese = _formSetting.checkBoxShowOnlyJapanese.Checked;
            _showOnlyFollowees = _formSetting.checkBoxShowOnlyFollowees.Checked;
            _nokakoiKey = _formSetting.textBoxNokakoiKey.Text;
            _sendDSSTP = _formSetting.checkBoxSendDSSTP.Checked;
            _password = _formSetting.textBoxPassword.Text;
            try
            {
                // 別アカウントログイン失敗に備えてクリアしておく
                _nsec = string.Empty;
                _npubHex = string.Empty;
                //_npub = string.Empty;
                _followeesHexs.Clear();
                textBoxPost.PlaceholderText = "Hello Nostr!";
                _formPostBar.textBoxPost.PlaceholderText = "kakoi";

                // 秘密鍵と公開鍵取得
                _nsec = NokakoiCrypt.DecryptNokakoiKey(_nokakoiKey, _password);
                _npubHex = _nsec.GetNpubHex();
                //_npub = _npubHex.ConvertToNpub();

                // ログイン済みの時
                if (!_npubHex.IsNullOrEmpty())
                {
                    int connectCount = await _nostrAccess.ConnectAsync();
                    if (0 == connectCount)
                    {
                        textBoxPost.PlaceholderText = "> No relay enabled.";
                        return;
                    }

                    // フォロイーを購読をする
                    _nostrAccess.SubscribeFollows(_npubHex);

                    // ログインユーザー表示名取得
                    var name = GetUserName(_npubHex);
                    textBoxPost.PlaceholderText = $"> Login as {name}.";
                    textBoxPost.PlaceholderText = $"Post as {name}";
                    _formPostBar.textBoxPost.PlaceholderText = $"Post as {name}";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                textBoxPost.PlaceholderText = "> Decryption failed.";
            }

            Setting.TopMost = TopMost;
            Setting.CutLength = _cutLength;
            Setting.CutNameLength = _cutNameLength;
            Setting.Opacity = Opacity;
            Setting.AddClient = _addClient;
            Setting.ShowOnlyJapanese = _showOnlyJapanese;
            Setting.ShowOnlyFollowees = _showOnlyFollowees;
            Setting.NokakoiKey = _nokakoiKey;
            Setting.SendDSSTP = _sendDSSTP;

            Setting.Save(_configPath);
            _emojis = Tools.LoadEmojis();
            comboBoxEmoji.DataSource = _emojis;
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
            /*
            // 辞書にない場合プロフィールを購読する
            if (!_users.TryGetValue(publicKeyHex, out User? user))
            {
                SubscribeProfiles([publicKeyHex]);
            }
            */
            // kind 0 を毎回購読するように変更（頻繁にdisplay_name等を変更するユーザーがいるため）
            _nostrAccess.SubscribeProfiles([publicKeyHex]);

            // 情報があれば表示名を取得
            Users.TryGetValue(publicKeyHex, out User? user);
            string? userName = "???";
            if (null != user)
            {
                userName = user.DisplayName;
                // display_nameが無い場合は@nameとする
                if (null == userName || string.Empty == userName)
                {
                    userName = $"@{user.Name}";
                }
                // petnameがある場合は📛petnameとする
                if (!user.PetName.IsNullOrEmpty())
                {
                    userName = $"📛{user.PetName}";
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
            _nostrAccess.CloseSubscriptions();
            _nostrAccess.DisconnectAndDispose();

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
                Setting.WebLocation = _formWeb.RestoreBounds.Location;
                Setting.WebSize = _formWeb.RestoreBounds.Size;
            }
            else
            {
                Setting.WebLocation = _formWeb.Location;
                Setting.WebSize = _formWeb.Size;
            }
            Setting.NameColumnWidth = dataGridViewNotes.Columns["name"].Width;
            Setting.GridColor = Tools.ColorToHex(dataGridViewNotes.GridColor);
            Setting.Save(_configPath);
            Tools.SaveUsers(Users);
            //Tools.SaveEmojis(_emojis);
            Notifier.SaveSettings(); // 必要ないが更新日時をそろえるため

            _ds.Dispose();      // FrmMsgReceiverのThread停止せず1000ms待たされるうえにプロセス残るので…
            Application.Exit(); // ←これで殺す。SSTLibに手を入れた方がいいが、とりあえず。
        }
        #endregion

        #region ロード時
        // ロード時
        private void FormMain_Load(object sender, EventArgs e)
        {
            _formPostBar.ShowDialog();
            ButtonStart_Click(sender, e);
        }
        #endregion

        #region ポストバー表示切り替え
        // ポストバー表示切り替え
        private void CheckBoxPostBar_CheckedChanged(object sender, EventArgs e)
        {
            _formPostBar.textBoxPost.Focus();
            _formPostBar.Visible = checkBoxPostBar.Checked;
        }
        #endregion

        #region CTRL + ENTERで投稿
        // CTRL + ENTERで投稿
        private void TextBoxPost_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Enter | Keys.Control))
            {
                ButtonPost_Click(sender, e);
            }
        }
        #endregion

        #region 画面表示切替
        // 画面表示切替
        private void FormMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F11 || e.KeyCode == Keys.F12)
            {
                checkBoxPostBar.Checked = !checkBoxPostBar.Checked;
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

        #region カーソルキー
        private void DataGridViewNotes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right)
            {
                var ev = new DataGridViewCellEventArgs(0, dataGridViewNotes.SelectedRows[0].Index);
                DataGridViewNotes_CellDoubleClick(sender, ev);
            }
            if (e.KeyCode == Keys.Left)
            {
                var mev = new MouseEventArgs(MouseButtons.Right, 1, 0, 0, 0);
                var ev = new DataGridViewCellMouseEventArgs(0, dataGridViewNotes.SelectedRows[0].Index, 0, 0, mev);
                DataGridViewNotes_CellMouseClick(sender, ev);
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
                    _formWeb.Location = Setting.WebLocation;
                    if (new Point(0, 0) == _formWeb.Location)
                    {
                        _formWeb.StartPosition = FormStartPosition.CenterScreen;
                    }
                    _formWeb.Size = Setting.WebSize;
                }
                if (!_formWeb.Visible)
                {
                    _formWeb.Show(this);
                }
                if (_formWeb.WindowState == FormWindowState.Minimized)
                {
                    _formWeb.WindowState = FormWindowState.Normal;
                }
                _formWeb.Opacity = Setting.Opacity;
                var id = dataGridViewNotes.Rows[e.RowIndex].Cells["id"].Value.ToString() ?? "";
                NIP19.NostrEventNote nostrEventNote = new()
                {
                    EventId = id,
                    Relays = [string.Empty],
                };
                var settings = Notifier.Settings;
                var nevent = nostrEventNote.ToNIP19();
                try
                {
                    _formWeb.webView21.Source = new Uri(settings.FileName + nostrEventNote.ToNIP19());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    _formWeb.Close();
                }
            }
        }
        #endregion
    }
}
