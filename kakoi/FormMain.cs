﻿using kakoi.Properties;
using NNostr.Client;
using NNostr.Client.Protocols;
using NTextCat;
using NTextCat.Commons;
using SkiaSharp;
using SSTPLib;
using Svg.Skia;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace kakoi
{
    public partial class FormMain : Form
    {
        #region フィールド
        private const int HOTKEY_ID = 1;
        private const int MOD_CONTROL = 0x0002;
        private const int MOD_SHIFT = 0x0004;
        private const int WM_HOTKEY = 0x0312;

        private const string NostrPattern = @"nostr:(\w+)";
        private const string ImagePattern = @"(https?:\/\/.*\.(jpg|jpeg|png|gif|bmp|webp))";
        private const string UrlPattern = @"(https?:\/\/[^\s]+)";

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private readonly string _configPath = Path.Combine(Application.StartupPath, "kakoi.config");

        private readonly FormSetting _formSetting = new();
        private readonly FormPostBar _formPostBar = new();
        private FormManiacs _formManiacs = new();
        private FormRelayList _formRelayList = new();
        private FormWeb _formWeb = new();
        internal Point _formWebLocation = new();
        internal Size _formWebSize = new();
        private FormAI _formAI = new();

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

        private bool _getAvatar;
        private bool _showOnlyFollowees;
        private bool _minimizeToTray;
        private bool _showOnlySelectedLanguage;
        private bool _showDAN;
        private bool _showDEU;
        private bool _showENG;
        private bool _showFRA;
        private bool _showITA;
        private bool _showJPN;
        private bool _showKOR;
        private bool _showNLD;
        private bool _showNOR;
        private bool _showPOR;
        private bool _showRUS;
        private bool _showSPA;
        private bool _showSWE;
        private bool _showZHO;
        private bool _showRepostsOnlyFromFollowees;
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

        private readonly ImeStatus _imeStatus = new();
        private bool _reallyClose = false;
        private static Mutex? _mutex;

        // 前回の最新created_at
        internal DateTimeOffset LastCreatedAt = DateTimeOffset.MinValue;
        // 最新のcreated_at
        internal DateTimeOffset LatestCreatedAt = DateTimeOffset.MinValue;
        #endregion

        #region コンストラクタ
        // コンストラクタ
        public FormMain()
        {
            InitializeComponent();

            // アプリケーションの実行パスを取得
            string exePath = Application.ExecutablePath;
            string mutexName = $"kakoiMutex_{exePath.Replace("\\", "_")}";

            // 二重起動を防ぐためのミューテックスを作成
            _mutex = new Mutex(true, mutexName, out bool createdNew);

            if (!createdNew)
            {
                // 既に起動している場合はメッセージを表示して終了
                MessageBox.Show("Already running.", "kakoi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Environment.Exit(0);
            }

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
            _getAvatar = Setting.GetAvatar;
            _showOnlyFollowees = Setting.ShowOnlyFollowees;
            _minimizeToTray = Setting.MinimizeToTray;
            notifyIcon.Visible = _minimizeToTray;
            _showOnlySelectedLanguage = Setting.ShowOnlySelectedLanguage;
            _showDAN = Setting.ShowDAN;
            _showDEU = Setting.ShowDEU;
            _showENG = Setting.ShowENG;
            _showFRA = Setting.ShowFRA;
            _showITA = Setting.ShowITA;
            _showJPN = Setting.ShowJPN;
            _showKOR = Setting.ShowKOR;
            _showNLD = Setting.ShowNLD;
            _showNOR = Setting.ShowNOR;
            _showPOR = Setting.ShowPOR;
            _showRUS = Setting.ShowRUS;
            _showSPA = Setting.ShowSPA;
            _showSWE = Setting.ShowSWE;
            _showZHO = Setting.ShowZHO;
            _showRepostsOnlyFromFollowees = Setting.ShowRepostsOnlyFromFollowees;
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
            _formAI.MainForm = this;

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
                if (NostrAccess.Clients != null)
                {
                    connectCount = await NostrAccess.ConnectAsync();
                }
                else
                {
                    connectCount = await NostrAccess.ConnectAsync();

                    if (NostrAccess.Clients != null)
                    {
                        NostrAccess.Clients.EventsReceived += OnClientOnUsersInfoEventsReceived;
                        NostrAccess.Clients.EventsReceived += OnClientOnTimeLineEventsReceived;
                    }
                }

                toolTipRelays.SetToolTip(labelRelays, string.Join("\n", NostrAccess.RelayStatusList));

                switch (connectCount)
                {
                    case 0:
                        labelRelays.Text = "No relay enabled.";
                        buttonStart.Enabled = false;
                        return;
                    case 1:
                        labelRelays.Text = $"{connectCount} relay";
                        break;
                    default:
                        labelRelays.Text = $"{connectCount} relays";
                        break;
                }

                await NostrAccess.SubscribeAsync();

                buttonStart.Enabled = false;
                buttonStop.Enabled = true;
                dataGridViewNotes.Focus();
                _formPostBar.textBoxPost.Enabled = true;
                _formPostBar.buttonPost.Enabled = true;

                // ログイン済みの時
                if (!string.IsNullOrEmpty(_npubHex))
                {
                    // フォロイーを購読をする
                    await NostrAccess.SubscribeFollowsAsync(_npubHex);

                    // ログインユーザー名取得
                    var loginName = GetName(_npubHex);
                    if (!loginName.IsNullOrEmpty())
                    {
                        Text = $"kakoi - @{loginName}";
                        notifyIcon.Text = $"kakoi - @{loginName}";
                    }
                }

                dataGridViewNotes.Rows.Clear();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                labelRelays.Text = "Could not start.";
            }
        }
        #endregion

        #region ユーザー情報イベント受信時処理
        /// <summary>
        /// ユーザー情報イベント受信時処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void OnClientOnUsersInfoEventsReceived(object? sender, (string subscriptionId, NostrEvent[] events) args)
        {
            if (args.subscriptionId == NostrAccess.GetFolloweesSubscriptionId)
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
                                    if (user != null)
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
                    if (0 == nostrEvent.Kind && nostrEvent.Content != null && nostrEvent.PublicKey != null)
                    {
                        var newUserData = Tools.JsonToUser(nostrEvent.Content, nostrEvent.CreatedAt, Notifier.Settings.MuteMostr);
                        if (newUserData != null)
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
                            if (cratedAt == null || (cratedAt < newUserData.CreatedAt))
                            {
                                newUserData.LastActivity = DateTime.Now;
                                newUserData.PetName = existingUserData?.PetName;
                                Tools.SaveUsers(Users);
                                // 辞書に追加（上書き）
                                Users[nostrEvent.PublicKey] = newUserData;
                                Debug.WriteLine($"cratedAt updated {cratedAt} -> {newUserData.CreatedAt}");
                                Debug.WriteLine($"プロフィール更新: {newUserData.DisplayName} @{newUserData.Name}");

                                if (_getAvatar && newUserData.Picture != null && newUserData.Picture.Length > 0)
                                {
                                    await GetAvatarAsync(nostrEvent.PublicKey, newUserData.Picture);
                                    // avatar列にアバターを表示
                                    await PutAvatarAsync(newUserData, nostrEvent.PublicKey);
                                }
                            }
                        }
                    }
                }
                #endregion
            }
        }
        #endregion

        #region タイムラインイベント受信時処理
        /// <summary>
        /// タイムラインイベント受信時処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void OnClientOnTimeLineEventsReceived(object? sender, (string subscriptionId, NostrEvent[] events) args)
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
                        string userName = string.Empty;
                        User? user = null;

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
                                // プロフィール購読
                                await NostrAccess.SubscribeProfilesAsync([nostrEvent.PublicKey]);

                                // ユーザー取得
                                user = await GetUserAsync(nostrEvent.PublicKey);
                                // ユーザーが見つからない時は表示しない
                                if (user == null)
                                {
                                    continue;
                                }
                                // ユーザー表示名取得
                                userName = GetUserName(nostrEvent.PublicKey);
                                string likedName = GetUserName(nostrEvent.GetTaggedPublicKeys()[0]);

                                headMark = "+";

                                // グリッドに表示
                                //_noteEvents.AddFirst(nostrEvent);
                                DateTimeOffset dto = nostrEvent.CreatedAt ?? DateTimeOffset.Now;
                                dataGridViewNotes.Rows.Insert(
                                0,
                                dto.ToLocalTime(),
                                new Bitmap(1, 1),
                                $"{headMark} {userName}",
                                $"Sent {content} to {likedName}.",
                                nostrEvent.Id,
                                nostrEvent.PublicKey,
                                nostrEvent.Kind
                                );

                                // 背景色をリアクションカラーに変更
                                dataGridViewNotes.Rows[0].DefaultCellStyle.BackColor = Tools.HexToColor(Setting.ReactionColor);

                                // 行を装飾
                                await EditRowAsync(nostrEvent, user, userName);
                            }

                            // ログイン済みで自分へのリアクション
                            if (!_npubHex.IsNullOrEmpty() && nostrEvent.GetTaggedPublicKeys().Contains(_npubHex))
                            {
                                // プロフィール購読
                                await NostrAccess.SubscribeProfilesAsync([nostrEvent.PublicKey]);

                                // ユーザー取得
                                user = await GetUserAsync(nostrEvent.PublicKey);
                                // ユーザーが見つからない時は表示しない
                                if (user == null)
                                {
                                    continue;
                                }
                                // ユーザー表示名取得
                                userName = GetUserName(nostrEvent.PublicKey);

                                headMark = "+";

                                // グリッドに表示
                                //_noteEvents.AddFirst(nostrEvent);
                                DateTimeOffset dto = nostrEvent.CreatedAt ?? DateTimeOffset.Now;
                                dataGridViewNotes.Rows.Insert(
                                0,
                                dto.ToLocalTime(),
                                new Bitmap(1, 1),
                                $"{headMark} {userName}",
                                nostrEvent.Content,
                                nostrEvent.Id,
                                nostrEvent.PublicKey,
                                nostrEvent.Kind
                                );

                                // 背景色をリアクションカラーに変更
                                dataGridViewNotes.Rows[0].DefaultCellStyle.BackColor = Tools.HexToColor(Setting.ReactionColor);

                                // 行を装飾
                                await EditRowAsync(nostrEvent, user, userName);

                                // SSPに送る
                                if (_sendDSSTP && _ds != null)
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
                            string editedContent = content;

                            // nostr:npub1またはnostr:nprofile1が含まれている場合、@ユーザー名を取得
                            MatchCollection matches = Regex.Matches(editedContent, @"nostr:(npub1\w+|nprofile1\w+)");
                            foreach (Match match in matches)
                            {
                                if (match.Success)
                                {
                                    string npubOrNprofile = match.Groups[1].Value.ConvertToHex();
                                    // ユーザー名取得
                                    string mentionedUserName = $"［👤{GetUserName(npubOrNprofile)}］";
                                    // nostr:npub1またはnostr:nprofile1を@ユーザー名に置き換え
                                    editedContent = editedContent.Replace(match.Value, mentionedUserName);
                                }
                            }

                            //string nostrPattern = @"nostr:(\w+)";
                            // nostr:を含む場合、(citations omitted)に置き換え
                            editedContent = Regex.Replace(editedContent, NostrPattern, "［🗒️］");

                            //string imagePattern = @"(https?:\/\/.*\.(jpg|jpeg|png|gif|bmp|webp))";
                            // 画像URLを含む場合、(image)に置き換え
                            editedContent = Regex.Replace(editedContent, ImagePattern, "［🖼️］", RegexOptions.IgnoreCase);

                            //string urlPattern = @"(https?:\/\/[^\s]+)";
                            // URLを含む場合、(url)に置き換え
                            editedContent = Regex.Replace(editedContent, UrlPattern, "［🔗］", RegexOptions.IgnoreCase);

                            // 言語判定
                            var lang = DetermineLanguage(editedContent);
                            // 言語限定表示オンでフォロイーじゃない時は表示しない
                            if (_showOnlySelectedLanguage && !_followeesHexs.Contains(nostrEvent.PublicKey))
                            {
                                if (_showDAN && "dan" == lang ||
                                    _showDEU && "deu" == lang ||
                                    _showENG && "eng" == lang ||
                                    _showFRA && "fra" == lang ||
                                    _showITA && "ita" == lang ||
                                    _showJPN && "jpn" == lang ||
                                    _showKOR && "kor" == lang ||
                                    _showNLD && "nld" == lang ||
                                    _showNOR && "nor" == lang ||
                                    _showPOR && "por" == lang ||
                                    _showRUS && "rus" == lang ||
                                    _showSPA && "spa" == lang ||
                                    _showSWE && "swe" == lang ||
                                    _showZHO && "zho" == lang)
                                {
                                    // 何もしない
                                }
                                else
                                {
                                    // 表示しない
                                    continue;
                                }
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
                            // pタグにミュートされている公開鍵が含まれている時は表示しない
                            if (nostrEvent.GetTaggedPublicKeys().Any(pk => IsMuted(pk)))
                            {
                                continue;
                            }

                            // プロフィール購読
                            await NostrAccess.SubscribeProfilesAsync([nostrEvent.PublicKey]);

                            // ユーザー取得
                            user = await GetUserAsync(nostrEvent.PublicKey);
                            // ユーザーが見つからない時は表示しない
                            if (user == null)
                            {
                                continue;
                            }
                            // ユーザー表示名取得
                            userName = GetUserName(nostrEvent.PublicKey);

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

                                if (p != null && 0 < p.Length)
                                {
                                    string mentionedUserNames = string.Empty;
                                    foreach (var u in p)
                                    {
                                        mentionedUserNames = $"{mentionedUserNames} {GetUserName(u)}";
                                    }
                                    editedContent = $"［💬{mentionedUserNames}］\r\n{editedContent}";
                                }
                            }

                            // グリッドに表示
                            //_noteEvents.AddFirst(nostrEvent);
                            DateTimeOffset dto = nostrEvent.CreatedAt ?? DateTimeOffset.Now;
                            dataGridViewNotes.Rows.Insert(
                                0,
                                dto.ToLocalTime(),
                                new Bitmap(1, 1),
                                $"{headMark} {userName}",
                                //nostrEvent.Content,
                                editedContent,
                                nostrEvent.Id,
                                nostrEvent.PublicKey,
                                nostrEvent.Kind
                                );
                            //dataGridViewNotes.Sort(dataGridViewNotes.Columns["time"], ListSortDirection.Descending);

                            // リプライの時は背景色変更
                            if (isReply)
                            {
                                dataGridViewNotes.Rows[0].DefaultCellStyle.BackColor = Tools.HexToColor(Setting.ReplyColor);
                            }

                            // 行を装飾
                            await EditRowAsync(nostrEvent, user, userName);

                            // SSPに送る
                            if (_sendDSSTP && _ds != null)
                            {
                                NIP19.NostrEventNote nostrEventNote = new()
                                {
                                    EventId = nostrEvent.Id,
                                    Relays = [string.Empty],
                                };
                                var nevent = nostrEventNote.ToNIP19();
                                SearchGhost();

                                //string msg = content;
                                Dictionary<string, string> SSTPHeader = new(_baseSSTPHeader)
                                {
                                    { "Reference1", "1" }, // kind
                                    { "Reference2", content }, // content
                                    { "Reference3", user?.Name ?? "???" }, // name
                                    { "Reference4", user?.DisplayName ?? string.Empty }, // display_name
                                    { "Reference5", user?.Picture ?? string.Empty }, // picture
                                    { "Reference6", nevent }, // nevent1...
                                    { "Reference7", nostrEvent.PublicKey.ConvertToNpub() }, // npub1...
                                    { "Script", $"{speaker}{userName}\\n{editedContent}\\e" }
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
                                    await ReactionAsync(nostrEvent.Id, nostrEvent.PublicKey, nostrEvent.Kind, "+");
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

                            // 改行をスペースに置き換えてログ表示
                            Debug.WriteLine($"{userName}: {content.Replace('\n', ' ')}");
                        }
                        #endregion

                        #region リポスト
                        if (6 == nostrEvent.Kind || 16 == nostrEvent.Kind)
                        {
                            // フォロイー限定表示オンでフォロイーじゃない時は表示しない
                            if ((_showOnlyFollowees || _showRepostsOnlyFromFollowees) && !_followeesHexs.Contains(nostrEvent.PublicKey))
                            {
                                continue;
                            }
                            // ミュートしている時は表示しない
                            if (IsMuted(nostrEvent.PublicKey))
                            {
                                continue;
                            }

                            // プロフィール購読
                            await NostrAccess.SubscribeProfilesAsync([nostrEvent.PublicKey]);

                            // リポスト元公開鍵取得
                            string originalPublicKey = string.Empty;
                            if (nostrEvent.GetTaggedPublicKeys().Length != 0)
                            {
                                originalPublicKey = nostrEvent.GetTaggedPublicKeys().Last();
                            }

                            // ユーザー取得
                            user = await GetUserAsync(nostrEvent.PublicKey);
                            // ユーザーが見つからない時は表示しない
                            if (user == null)
                            {
                                continue;
                            }
                            // ユーザー表示名取得
                            userName = GetUserName(nostrEvent.PublicKey);

                            // リポスト元ユーザー表示名取得
                            string originalUserName = GetUserName(originalPublicKey);

                            // グリッドに表示
                            DateTimeOffset dto = nostrEvent.CreatedAt ?? DateTimeOffset.Now;
                            dataGridViewNotes.Rows.Insert(
                            0,
                            dto.ToLocalTime(),
                            new Bitmap(1, 1),
                            $"{headMark} {userName}",
                            $"reposted {originalUserName}'s post.［k:{nostrEvent.Kind}］",
                            nostrEvent.Id,
                            nostrEvent.PublicKey,
                            nostrEvent.Kind
                            );

                            // 背景色をリポストカラーに変更
                            dataGridViewNotes.Rows[0].DefaultCellStyle.BackColor = Tools.HexToColor(Setting.RepostColor);

                            // 行を装飾
                            await EditRowAsync(nostrEvent, user, userName);

                            Debug.WriteLine($"リポスト: {userName} が {originalUserName} をリポスト");
                        }
                        #endregion
                    }
                }
                #endregion
            }
        }
        #endregion

        #region グリッド行装飾
        private async Task EditRowAsync(NostrEvent nostrEvent, User user, string userName)
        {
            // avatar列のToolTipに表示名を設定
            dataGridViewNotes.Rows[0].Cells["avatar"].ToolTipText = userName;
            // note列のToolTipにcontentを設定
            dataGridViewNotes.Rows[0].Cells["note"].ToolTipText = nostrEvent.Content;

            // avastar列の背景色をpubkeyColorに変更
            var pubkeyColor = Tools.HexToColor(nostrEvent.PublicKey[..6]); // [i..j] で「i番目からj番目の範囲」
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

            // content-warning
            string[]? reason = null;
            try
            {
                reason = nostrEvent.GetTaggedData("content-warning"); // reasonが無いと例外吐く
            }
            catch
            {
                reason = [""];
            }
            if (reason != null && 0 < reason.Length)
            {
                dataGridViewNotes.Rows[0].Cells["note"].Value = "CW: " + reason[0];
                //// ツールチップにcontentを設定
                //dataGridViewNotes.Rows[0].Cells["note"].ToolTipText = nostrEvent.Content;
                // note列の背景色をCWColorに変更
                dataGridViewNotes.Rows[0].Cells["note"].Style.BackColor = Tools.HexToColor(Setting.CWColor);
            }

            // avatar列にアバターを表示
            await PutAvatarAsync(user, nostrEvent.PublicKey);
        }
        #endregion

        #region ユーザー取得
        private async Task<User?> GetUserAsync(string pubkey)
        {
            User? user = null;
            int retryCount = 0;
            while (retryCount < 10)
            {
                Debug.WriteLine($"retryCount = {retryCount} {GetUserName(pubkey)}");
                Users.TryGetValue(pubkey, out user);
                // ユーザーが見つかった場合、ループを抜ける
                if (user != null)
                {
                    break;
                }
                // 一定時間待機してから再試行
                await Task.Delay(100);
                retryCount++;
            }
            return user;
        }
        #endregion

        #region avatar取得
        private async Task GetAvatarAsync(string publicKeyHex, string avatarUrl)
        {
            if (!_imeStatus.Compositing)
            {
                var postBarFcuced = _formPostBar.ContainsFocus;
                var formSettingFocusd = _formSetting.ContainsFocus;

                string picturePath = Path.Combine(_avatarPath, $"{publicKeyHex}.png");
                using HttpClient httpClient = new();
                httpClient.Timeout = TimeSpan.FromSeconds(5);   // タイムアウト5秒
                SKBitmap? bitmap = null;
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
                        if (svg.Picture != null)
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

                    if (bitmap == null)
                    {
                        return;
                    }

                    // 中央から正方形に切り取る
                    int size = Math.Min(bitmap.Width, bitmap.Height);
                    int x = (bitmap.Width - size) / 2;
                    int y = (bitmap.Height - size) / 2;
                    using var croppedBitmap = new SKBitmap(size, size);
                    using (var canvas = new SKCanvas(croppedBitmap))
                    {
                        canvas.DrawBitmap(bitmap, new SKRect(x, y, x + size, y + size), new SKRect(0, 0, size, size));
                    }

                    // リサイズ
                    using (var resizedBitmap = croppedBitmap?.Resize(new SKImageInfo(_avatarSize, _avatarSize), new SKSamplingOptions(SKFilterMode.Linear)))
                    {
                        if (resizedBitmap == null)
                        {
                            return;
                        }

                        // 円形に切り抜くためのマスクを作成
                        size = Math.Min(resizedBitmap.Width, resizedBitmap.Height);
                        using var maskBitmap = new SKBitmap(size, size);
                        using var canvas = new SKCanvas(maskBitmap);
                        var paint = new SKPaint
                        {
                            IsAntialias = true,
                            Color = SKColors.Black
                        };
                        canvas.Clear(SKColors.Transparent);
                        canvas.DrawCircle(size / 2, size / 2, size / 2, paint);

                        // マスクを適用して新しいビットマップを作成
                        using var resultBitmap = new SKBitmap(size, size);
                        using var resultCanvas = new SKCanvas(resultBitmap);
                        resultCanvas.Clear(SKColors.Transparent);
                        resultCanvas.DrawBitmap(resizedBitmap, new SKRect(0, 0, size, size));
                        paint = new SKPaint
                        {
                            IsAntialias = true,
                            BlendMode = SKBlendMode.DstIn
                        };
                        resultCanvas.DrawBitmap(maskBitmap, 0, 0, paint);

                        // 画像をPNG形式で保存
                        using SKImage image = SKImage.FromBitmap(resultBitmap);
                        using SKData data = image.Encode(SKEncodedImageFormat.Png, 100);
                        using FileStream fs = File.OpenWrite(picturePath);
                        data.SaveTo(fs);
                    }

                    Debug.WriteLine($"画像保存: {GetUserName(publicKeyHex)}");
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
                    bitmap?.Dispose();

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
        #endregion

        #region avatar列にアバターを表示
        private async Task PutAvatarAsync(User? user, string pubkey)
        {
            string avatarFile = Path.Combine(_avatarPath, $"{pubkey}.png");
            if (_getAvatar && user?.Picture != null && user.Picture.Length > 0)
            {
                if (!File.Exists(avatarFile))
                {
                    await GetAvatarAsync(pubkey, user.Picture);
                }
                if (File.Exists(avatarFile))
                {
                    using var fileStream = new FileStream(avatarFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using var avatar = new Bitmap(fileStream);
                    //dataGridViewNotes.Rows[0].Cells["avatar"].Value = new Bitmap(avatar);
                    foreach (DataGridViewRow row in dataGridViewNotes.Rows)
                    {
                        if (row.Cells["pubkey"].Value != null && row.Cells["pubkey"].Value.ToString() == pubkey)
                        {
                            row.Cells["avatar"].Value = new Bitmap(avatar); ;
                            break;
                        }
                    }
                    Debug.WriteLine($"画像表示: {GetUserName(pubkey)}");
                }
            }
        }
        #endregion

        #region Stopボタン
        // Stopボタン
        private void ButtonStop_Click(object sender, EventArgs e)
        {
            if (NostrAccess.Clients == null)
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
                Debug.WriteLine(ex.Message);
                labelRelays.Text = "Could not stop.";
            }
        }
        #endregion

        #region Postボタン
        // Postボタン
        internal void ButtonPost_Click(NostrEvent? rootEvent, bool isQuote)
        {
            if (0 == _formSetting.textBoxNsec.TextLength)
            {
                _formPostBar.textBoxPost.PlaceholderText = "Please set nsec.";
                return;
            }
            if (0 == _formPostBar.textBoxPost.TextLength)
            {
                _formPostBar.textBoxPost.PlaceholderText = "Cannot post empty.";
                return;
            }

            try
            {
                _ = PostAsync(rootEvent, isQuote);

                _formPostBar.textBoxPost.Text = string.Empty;
                _formPostBar.textBoxPost.PlaceholderText = string.Empty;
                _formPostBar.RootEvent = null;
                // デフォルトの色に戻す
                _formPostBar.textBoxPost.BackColor = SystemColors.Window;
                _formPostBar.buttonPost.BackColor = SystemColors.Control;
                // 送信後にチェックを外す
                checkBoxPostBar.Checked = false;
                dataGridViewNotes.Focus();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
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
        private async Task PostAsync(NostrEvent? rootEvent = null, bool isQuote = false)
        {
            if (NostrAccess.Clients == null)
            {
                return;
            }
            // create tags
            List<NostrEventTag> tags = [];
            if (rootEvent != null)
            {
                if (isQuote)
                {
                    tags.Add(new NostrEventTag() { TagIdentifier = "q", Data = [rootEvent.Id, string.Empty] });
                }
                else
                {
                    tags.Add(new NostrEventTag() { TagIdentifier = "e", Data = [rootEvent.Id, string.Empty] });
                    tags.Add(new NostrEventTag() { TagIdentifier = "p", Data = [rootEvent.PublicKey] });
                }
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
        private async Task ReactionAsync(string e, string p, int k, string? content, string? url = null)
        {
            if (NostrAccess.Clients == null)
            {
                return;
            }
            // create tags
            List<NostrEventTag> tags = [];
            tags.Add(new NostrEventTag() { TagIdentifier = "e", Data = [e] });
            tags.Add(new NostrEventTag() { TagIdentifier = "p", Data = [p] });
            tags.Add(new NostrEventTag() { TagIdentifier = "k", Data = [k.ToString()] });
            if (!url.IsNullOrEmpty())
            {
                tags.Add(new NostrEventTag() { TagIdentifier = "emoji", Data = [$"{content}", $"{url}"] });
                content = $":{content}:";
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

        #region リポスト処理
        private async Task RepostAsync(string e, string p, int k)
        {
            if (NostrAccess.Clients == null)
            {
                return;
            }
            // create tags
            List<NostrEventTag> tags = [];
            tags.Add(new NostrEventTag() { TagIdentifier = "e", Data = [e, string.Empty] });
            tags.Add(new NostrEventTag() { TagIdentifier = "p", Data = [p] });
            if (1 != k)
            {
                tags.Add(new NostrEventTag() { TagIdentifier = "k", Data = [k.ToString()] });
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
                Kind = k == 1 ? 6 : 16,
                Content = string.Empty,
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
            _formSetting.checkBoxGetAvatar.Checked = _getAvatar;
            _formSetting.checkBoxShowOnlyFollowees.Checked = _showOnlyFollowees;
            _formSetting.checkBoxMinimizeToTray.Checked = _minimizeToTray;
            _formSetting.checkBoxShowOnlySelectedLanguage.Checked = _showOnlySelectedLanguage;
            _formSetting.checkBoxDAN.Checked = _showDAN;
            _formSetting.checkBoxDEU.Checked = _showDEU;
            _formSetting.checkBoxENG.Checked = _showENG;
            _formSetting.checkBoxFRA.Checked = _showFRA;
            _formSetting.checkBoxITA.Checked = _showITA;
            _formSetting.checkBoxJPN.Checked = _showJPN;
            _formSetting.checkBoxKOR.Checked = _showKOR;
            _formSetting.checkBoxNLD.Checked = _showNLD;
            _formSetting.checkBoxNOR.Checked = _showNOR;
            _formSetting.checkBoxPOR.Checked = _showPOR;
            _formSetting.checkBoxRUS.Checked = _showRUS;
            _formSetting.checkBoxSPA.Checked = _showSPA;
            _formSetting.checkBoxSWE.Checked = _showSWE;
            _formSetting.checkBoxZHO.Checked = _showZHO;
            _formSetting.checkBoxShowRepostsOnlyFromFollowees.Checked = _showRepostsOnlyFromFollowees;
            _formSetting.checkBoxSendDSSTP.Checked = _sendDSSTP;
            _formSetting.checkBoxAddClient.Checked = _addClient;
            _formSetting.textBoxNsec.Text = _nsec;
            _formSetting.textBoxNpub.Text = _nsec.GetNpub();

            // 開く
            _formSetting.ShowDialog(this);

            // 閉じた後
            TopMost = _formSetting.checkBoxTopMost.Checked;
            Opacity = _formSetting.trackBarOpacity.Value / 100.0;
            _tempOpacity = Opacity;
            _formPostBar.Opacity = Opacity;
            _getAvatar = _formSetting.checkBoxGetAvatar.Checked;
            _showOnlyFollowees = _formSetting.checkBoxShowOnlyFollowees.Checked;
            _minimizeToTray = _formSetting.checkBoxMinimizeToTray.Checked;
            notifyIcon.Visible = _minimizeToTray;
            _showOnlySelectedLanguage = _formSetting.checkBoxShowOnlySelectedLanguage.Checked;
            _showDAN = _formSetting.checkBoxDAN.Checked;
            _showDEU = _formSetting.checkBoxDEU.Checked;
            _showENG = _formSetting.checkBoxENG.Checked;
            _showFRA = _formSetting.checkBoxFRA.Checked;
            _showITA = _formSetting.checkBoxITA.Checked;
            _showJPN = _formSetting.checkBoxJPN.Checked;
            _showKOR = _formSetting.checkBoxKOR.Checked;
            _showNLD = _formSetting.checkBoxNLD.Checked;
            _showNOR = _formSetting.checkBoxNOR.Checked;
            _showPOR = _formSetting.checkBoxPOR.Checked;
            _showRUS = _formSetting.checkBoxRUS.Checked;
            _showSPA = _formSetting.checkBoxSPA.Checked;
            _showSWE = _formSetting.checkBoxSWE.Checked;
            _showZHO = _formSetting.checkBoxZHO.Checked;
            _showRepostsOnlyFromFollowees = _formSetting.checkBoxShowRepostsOnlyFromFollowees.Checked;
            _nsec = _formSetting.textBoxNsec.Text;
            _sendDSSTP = _formSetting.checkBoxSendDSSTP.Checked;
            _addClient = _formSetting.checkBoxAddClient.Checked;
            try
            {
                // 別アカウントログイン失敗に備えてクリアしておく
                _npubHex = string.Empty;
                _followeesHexs.Clear();
                _formPostBar.textBoxPost.PlaceholderText = "kakoi";
                Text = "kakoi";
                notifyIcon.Text = "kakoi";

                // 秘密鍵と公開鍵取得
                _npubHex = _nsec.GetNpubHex();

                // ログイン済みの時
                if (!_npubHex.IsNullOrEmpty())
                {
                    int connectCount = await NostrAccess.ConnectAsync();

                    toolTipRelays.SetToolTip(labelRelays, string.Join("\n", NostrAccess.RelayStatusList));

                    switch (connectCount)
                    {
                        case 0:
                            labelRelays.Text = "No relay enabled.";
                            return;
                        case 1:
                            labelRelays.Text = $"{connectCount} relay";
                            break;
                        default:
                            labelRelays.Text = $"{connectCount} relays";
                            break;
                    }

                    // フォロイーを購読をする
                    await NostrAccess.SubscribeFollowsAsync(_npubHex);

                    // ログインユーザー名取得
                    var loginName = GetName(_npubHex);
                    if (!loginName.IsNullOrEmpty())
                    {
                        Text = $"kakoi - @{loginName}";
                        notifyIcon.Text = $"kakoi - @{loginName}";
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                labelRelays.Text = "Decryption failed.";
            }
            // nsecを保存
            SavePubkey(_npubHex);
            SaveNsec(_npubHex, _nsec);

            Setting.TopMost = TopMost;
            Setting.Opacity = Opacity;
            Setting.GetAvatar = _getAvatar;
            Setting.ShowOnlyFollowees = _showOnlyFollowees;
            Setting.MinimizeToTray = _minimizeToTray;
            Setting.ShowOnlySelectedLanguage = _showOnlySelectedLanguage;
            Setting.ShowDAN = _showDAN;
            Setting.ShowDEU = _showDEU;
            Setting.ShowENG = _showENG;
            Setting.ShowFRA = _showFRA;
            Setting.ShowITA = _showITA;
            Setting.ShowJPN = _showJPN;
            Setting.ShowKOR = _showKOR;
            Setting.ShowNLD = _showNLD;
            Setting.ShowNOR = _showNOR;
            Setting.ShowPOR = _showPOR;
            Setting.ShowRUS = _showRUS;
            Setting.ShowSPA = _showSPA;
            Setting.ShowSWE = _showSWE;
            Setting.ShowZHO = _showZHO;
            Setting.ShowRepostsOnlyFromFollowees = _showRepostsOnlyFromFollowees;
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
            if (_displayedEventIds.Count >= 4096)
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
                                            //Debug.WriteLine(_ghostName);
            }
            else
            {
                _ghostName = string.Empty;
                //Debug.WriteLine("ゴーストがいません");
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

        #region ユーザー名を取得する
        /// <summary>
        /// ユーザー名を取得する
        /// </summary>
        /// <param name="publicKeyHex">公開鍵HEX</param>
        /// <returns>ユーザー名</returns>
        private string? GetName(string publicKeyHex)
        {
            // 情報があればユーザー名を取得
            Users.TryGetValue(publicKeyHex, out User? user);
            string? userName = string.Empty;
            if (user != null)
            {
                userName = user.Name;
                // 取得日更新
                user.LastActivity = DateTime.Now;
                Tools.SaveUsers(Users);
            }
            return userName;
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
            // 情報があれば表示名を取得
            Users.TryGetValue(publicKeyHex, out User? user);
            string? userName = "???";
            if (user != null)
            {
                userName = user.DisplayName;
                // display_nameが無い場合は@nameとする
                if (userName == null || string.Empty == userName)
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
                //Debug.WriteLine($"名前取得: {user.DisplayName} @{user.Name} 📛{user.PetName}");
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
                if (user != null)
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
            if (_minimizeToTray && !_reallyClose && e.CloseReason == CloseReason.UserClosing)
            {
                // 閉じるボタンが押されたときは最小化
                e.Cancel = true;
                WindowState = FormWindowState.Minimized;
                Hide(); // フォームを非表示にします（タスクトレイに格納）
            }
            else
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
        }
        #endregion

        #region ロード時
        // ロード時
        private void FormMain_Load(object sender, EventArgs e)
        {
            // Ctrl + Shift + A をホットキーとして登録
            RegisterHotKey(this.Handle, HOTKEY_ID, MOD_CONTROL | MOD_SHIFT, (int)Keys.A);

            _formPostBar.ShowDialog();

            try
            {
                _npubHex = LoadPubkey();
                _nsec = LoadNsec();
                _formSetting.textBoxNsec.Text = _nsec;
                _formSetting.textBoxNpub.Text = _nsec.GetNpub();
                if (!_formSetting.textBoxNpub.Text.IsNullOrEmpty())
                {
                    _formSetting.textBoxNsec.Enabled = false;
                }

                ButtonStart_Click(sender, e);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                labelRelays.Text = "Decryption failed.";
            }
        }
        #endregion

        #region ポストバー表示切り替え
        // ポストバー表示切り替え
        private void CheckBoxPostBar_CheckedChanged(object sender, EventArgs e)
        {
            _formPostBar.Visible = checkBoxPostBar.Checked;
            if (_formPostBar.Visible)
            {
                _formPostBar.textBoxPost.Focus();
            }
            else
            {
                dataGridViewNotes.Focus();
            }
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
            // F5キーでFormAIを表示
            if (e.KeyCode == Keys.F5)
            {
                if (_formAI == null || _formAI.IsDisposed)
                {
                    _formAI = new FormAI();
                    _formAI.MainForm = this;
                }
                if (!_formAI.Visible)
                {
                    _formAI.Show(this);
                }
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
                if (_formManiacs == null || _formManiacs.IsDisposed)
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
            dataGridViewNotes.Focus();
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
            int kind = (int)selectedRow.Cells["kind"].Value;

            if (comboBoxEmoji.SelectedItem is Emoji emoji)
            {
                var content = emoji.Content;
                var url = emoji.Url;

                _ = ReactionAsync(id, pubkey, kind, content, url);
                // comboBoxEmojiの選択を先頭に戻す
                comboBoxEmoji.SelectedIndex = 0;
            }
        }
        #endregion

        #region グリッドキー入力
        private void DataGridViewNotes_KeyDown(object sender, KeyEventArgs e)
        {
            // Cキーで_formWebを閉じる
            if (e.KeyCode == Keys.C)
            {
                if (_formWeb != null && !_formWeb.IsDisposed)
                {
                    _formWeb.Close();
                }
            }
            // Wキーで選択行を上に
            if (e.KeyCode == Keys.W)
            {
                if (dataGridViewNotes.SelectedRows.Count > 0 && dataGridViewNotes.SelectedRows[0].Index > 0)
                {
                    dataGridViewNotes.Rows[dataGridViewNotes.SelectedRows[0].Index - 1].Selected = true;
                    dataGridViewNotes.CurrentCell = dataGridViewNotes["note", dataGridViewNotes.SelectedRows[0].Index];
                }
            }
            // Sキーで選択行を下に
            if (e.KeyCode == Keys.S)
            {
                if (dataGridViewNotes.SelectedRows.Count > 0 && dataGridViewNotes.SelectedRows[0].Index < dataGridViewNotes.Rows.Count - 1)
                {
                    dataGridViewNotes.Rows[dataGridViewNotes.SelectedRows[0].Index + 1].Selected = true;
                    dataGridViewNotes.CurrentCell = dataGridViewNotes["note", dataGridViewNotes.SelectedRows[0].Index];
                }
            }
            // Shift + Wキーで選択行を最上部に
            if (e.KeyCode == Keys.W && e.Shift)
            {
                if (dataGridViewNotes.SelectedRows.Count > 0 && dataGridViewNotes.SelectedRows[0].Index > 0)
                {
                    dataGridViewNotes.Rows[0].Selected = true;
                    dataGridViewNotes.CurrentCell = dataGridViewNotes["note", 0];
                }
            }
            // Shift + Sキーで選択行を最下部に
            if (e.KeyCode == Keys.S && e.Shift)
            {
                if (dataGridViewNotes.SelectedRows.Count > 0 && dataGridViewNotes.SelectedRows[0].Index < dataGridViewNotes.Rows.Count - 1)
                {
                    dataGridViewNotes.Rows[^1].Selected = true; // インデックス演算子 [^i] で「後ろからi番目の要素」
                    dataGridViewNotes.CurrentCell = dataGridViewNotes["note", dataGridViewNotes.Rows.Count - 1];
                }
            }
            // リアクション
            if (e.KeyCode == Keys.Right || e.KeyCode == Keys.F)
            {
                if (dataGridViewNotes.SelectedRows.Count > 0 && dataGridViewNotes.SelectedRows[0].Index >= 0)
                {
                    // 画面外に出た時サイズ変更用カーソルを記憶しているのでデフォルトに戻す
                    Cursor.Current = Cursors.Default;
                    var ev = new DataGridViewCellEventArgs(3, dataGridViewNotes.SelectedRows[0].Index);
                    DataGridViewNotes_CellDoubleClick(sender, ev);
                }
            }
            // Webビュー表示
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.A)
            {
                if (dataGridViewNotes.SelectedRows.Count > 0 && dataGridViewNotes.SelectedRows[0].Index >= 0)
                {
                    var mev = new MouseEventArgs(MouseButtons.Right, 1, 0, 0, 0);
                    var ev = new DataGridViewCellMouseEventArgs(0, dataGridViewNotes.SelectedRows[0].Index, 0, 0, mev);
                    DataGridViewNotes_CellMouseClick(sender, ev);
                }
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
                    dataGridViewNotes.Focus();
                }
            }
            // Rキーで返信
            if (e.KeyCode == Keys.R)
            {
                if (dataGridViewNotes.SelectedRows.Count > 0 && dataGridViewNotes.SelectedRows[0].Index >= 0)
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
            // Qキーで引用
            if (e.KeyCode == Keys.Q)
            {
                if (dataGridViewNotes.SelectedRows.Count > 0 && dataGridViewNotes.SelectedRows[0].Index >= 0)
                {
                    var rootEvent = new NostrEvent()
                    {
                        Id = (string)dataGridViewNotes.Rows[dataGridViewNotes.SelectedRows[0].Index].Cells["id"].Value,
                        PublicKey = (string)dataGridViewNotes.Rows[dataGridViewNotes.SelectedRows[0].Index].Cells["pubkey"].Value
                    };
                    _formPostBar.RootEvent = rootEvent;
                    var eventNote = new NIP19.NostrEventNote()
                    {
                        EventId = rootEvent.Id,
                        Relays = [string.Empty],
                    };
                    _formPostBar.textBoxPost.Text = $"{Environment.NewLine}nostr:{eventNote.ToNIP19()}";
                    _formPostBar.textBoxPost.SelectionStart = 0;
                    _formPostBar.IsQuote = true;
                    if (!checkBoxPostBar.Checked)
                    {
                        checkBoxPostBar.Checked = true;
                    }
                    _formPostBar.Focus();
                }
            }

            // Zキーでnote列の折り返し切り替え
            if (e.KeyCode == Keys.Z)
            {
                var ev = new MouseEventArgs(MouseButtons.Left, 2, 0, 0, 0);
                FormMain_MouseDoubleClick(sender, ev);
            }
            // Bキーで選択行をリポスト
            if (e.KeyCode == Keys.B)
            {
                if (dataGridViewNotes.SelectedRows.Count > 0 && dataGridViewNotes.SelectedRows[0].Index >= 0)
                {
                    var id = (string)dataGridViewNotes.Rows[dataGridViewNotes.SelectedRows[0].Index].Cells["id"].Value;
                    var pubkey = (string)dataGridViewNotes.Rows[dataGridViewNotes.SelectedRows[0].Index].Cells["pubkey"].Value;
                    var kind = (int)dataGridViewNotes.Rows[dataGridViewNotes.SelectedRows[0].Index].Cells["kind"].Value;
                    _ = RepostAsync(id, pubkey, kind);
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

                if (_formWeb == null || _formWeb.IsDisposed)
                {
                    _formWeb = new FormWeb
                    {
                        Location = _formWebLocation,
                        Size = _formWebSize
                    };
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
                    //Relays = [string.Empty],
                    Relays = [],
                };
                var nevent = nostrEventNote.ToNIP19();
                try
                {
                    _formWeb.webView2.Source = new Uri(Setting.WebViewUrl + nevent);

                    //var app = new ProcessStartInfo
                    //{
                    //    FileName = Setting.WebViewUrl + nevent,
                    //    UseShellExecute = true
                    //};
                    //Process.Start(app);
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

        #region フォーム最初の表示時
        private void FormMain_Shown(object sender, EventArgs e)
        {
            dataGridViewNotes.Focus();
        }
        #endregion

        #region パスワード管理
        private static void SavePubkey(string pubkey)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Remove("pubkey");
            config.AppSettings.Settings.Add("pubkey", pubkey);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private static string LoadPubkey()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            return config.AppSettings.Settings["pubkey"]?.Value ?? string.Empty;
        }

        private static void SaveNsec(string pubkey, string nsec)
        {
            // 前回のトークンを削除
            DeletePreviousTarget();

            // 新しいtargetを生成して保存
            string target = Guid.NewGuid().ToString();
            Tools.SavePassword("kakoi_" + target, pubkey, nsec);
            SaveTarget(target);
        }

        private static void SaveTarget(string target)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Remove("target");
            config.AppSettings.Settings.Add("target", target);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private static void DeletePreviousTarget()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var previousTarget = config.AppSettings.Settings["target"]?.Value;
            if (!previousTarget.IsNullOrEmpty())
            {
                Tools.DeletePassword("kakoi_" + previousTarget);
                config.AppSettings.Settings.Remove("target");
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
        }

        private static string LoadNsec()
        {
            string target = LoadTarget();
            if (!target.IsNullOrEmpty())
            {
                return Tools.LoadPassword("kakoi_" + target);
            }
            return string.Empty;
        }

        private static string LoadTarget()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            return config.AppSettings.Settings["target"]?.Value ?? string.Empty;
        }
        #endregion

        #region グローバルホットキー
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == HOTKEY_ID)
            {
                this.Activate(); // FormMainをアクティブにする
            }
            base.WndProc(ref m);
        }
        #endregion

        #region タスクトレイ最小化
        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            // 右クリック時は抜ける
            if (e is MouseEventArgs me && me.Button == MouseButtons.Right)
            {
                return;
            }

            // 最小化時は通常表示に戻す
            if (WindowState == FormWindowState.Minimized)
            {
                Show();
                WindowState = FormWindowState.Normal;
            }
            else if (WindowState == FormWindowState.Normal)
            {
                WindowState = FormWindowState.Minimized;
            }
        }

        private void SettingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 設定画面がすでに開かれている場合は抜ける
            if (_formSetting.Visible)
            {
                return;
            }
            Show();
            WindowState = FormWindowState.Normal;
            ButtonSetting_Click(sender, e);
        }

        private void QuitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _reallyClose = true;
            Close();
        }

        private void FormMain_SizeChanged(object sender, EventArgs e)
        {
            // 最小化時はタスクトレイに格納
            if (_minimizeToTray && WindowState == FormWindowState.Minimized)
            {
                Hide();
            }
        }
        #endregion

        public string GetNotesContent()
        {
            var notes = new StringBuilder();
            int count = 0;
            try
            {
                Debug.Print($"_lastCreatedAt: {LastCreatedAt}");
                foreach (DataGridViewRow row in dataGridViewNotes.Rows)
                {
                    // timeが_lastCreatedAtの時は抜ける
                    if (DateTimeOffset.TryParse(row.Cells["time"].Value?.ToString(), out DateTimeOffset createdAt) && createdAt == LastCreatedAt)
                    {
                        Debug.Print($"_lastCreatedAt: {LastCreatedAt}");
                        break;
                    }
                    // 一番上の行のtimeをDateTimeOffsetに変換して_latestCreatedAtに保存
                    if (count == 0)
                    {
                        if (DateTimeOffset.TryParse(row.Cells["time"].Value?.ToString(), out DateTimeOffset latestCreatedAt))
                        {
                            LatestCreatedAt = latestCreatedAt;
                        }
                    }
                    // 指定件数で抜ける
                    if (count >= _formAI.numericUpDownNumberOfPosts.Value)
                    {
                        break;
                    }
                    // kindが7の時はスキップ
                    if ((int)row.Cells["kind"].Value == 7)
                    {
                        continue;
                    }
                    notes.Append(row.Cells["time"].Value?.ToString() + "\r\n");
                    notes.Append(row.Cells["name"].Value?.ToString()?.Substring(2) + "\r\n");
                    notes.Append(row.Cells["note"].Value?.ToString() + "\r\n\r\n");
                    notes.AppendLine();
                    count++;
                }
                LastCreatedAt = LatestCreatedAt;
                Debug.Print($"_latestCreatedAt: {LatestCreatedAt} count: {count}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return notes.ToString();
        }
    }
}
