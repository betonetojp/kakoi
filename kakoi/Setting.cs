using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace kakoi
{
    public static class Setting
    {
        private static Data _data = new();

        #region データクラス
        /// <summary>
        /// 設定データクラス
        /// </summary>
        public class Data
        {
            public Point Location { get; set; }
            public Size Size { get; set; } = new Size(400, 400);
            public int NameColumnWidth { get; set; } = 70;

            public bool TopMost { get; set; } = false;
            public double Opacity { get; set; } = 1.00;
            public bool GetAvatar { get; set; } = true;
            public bool ShowOnlyFollowees { get; set; } = false;
            public bool ShowRepostsOnlyFromFollowees { get; set; } = false;
            public bool ShowOnlySelectedLanguage { get; set; } = false;
            public bool ShowDAN { get; set; } = false;
            public bool ShowDEU { get; set; } = false;
            public bool ShowENG { get; set; } = false;
            public bool ShowFRA { get; set; } = false;
            public bool ShowITA { get; set; } = false;
            public bool ShowJPN { get; set; } = false;
            public bool ShowKOR { get; set; } = false;
            public bool ShowNLD { get; set; } = false;
            public bool ShowNOR { get; set; } = false;
            public bool ShowPOR { get; set; } = false;
            public bool ShowRUS { get; set; } = false;
            public bool ShowSPA { get; set; } = false;
            public bool ShowSWE { get; set; } = false;
            public bool ShowZHO { get; set; } = false;
            public bool MinimizeToTray { get; set; } = false;
            public bool SendDSSTP { get; set; } = false;
            public bool AddClient { get; set; } = true;

            public int AvatarSize { get; set; } = 32;
            public string GridColor { get; set; } = "#FF1493";
            public string ReactionColor { get; set; } = "#FFFFE0";
            public string ReplyColor { get; set; } = "#E6E6FA";
            public string RepostColor { get; set; } = "#F0F8FF";
            public string CWColor { get; set; } = "#C0C0C0";
            public string WebViewUrl { get; set; } = "https://lumilumi.app/";

            public Point PostBarLocation { get; set; }
            public Size PostBarSize { get; set; } = new Size(400, 120);
            public string PictureUploadUrl { get; set; } = "https://nikolat.github.io/nostr-learn-nip96/";

            public Point WebViewLocation { get; set; }
            public Size WebViewSize { get; set; } = new Size(400, 400);
        }
        #endregion

        #region プロパティ
        public static Point Location
        {
            get => _data.Location;
            set => _data.Location = value;
        }
        public static Size Size
        {
            get => _data.Size;
            set => _data.Size = value;
        }
        public static int NameColumnWidth
        {
            get => _data.NameColumnWidth;
            set => _data.NameColumnWidth = value;
        }

        public static bool TopMost
        {
            get => _data.TopMost;
            set => _data.TopMost = value;
        }
        public static double Opacity
        {
            get => _data.Opacity;
            set => _data.Opacity = value;
        }
        public static bool GetAvatar
        {
            get => _data.GetAvatar;
            set => _data.GetAvatar = value;
        }
        public static bool ShowOnlyFollowees
        {
            get => _data.ShowOnlyFollowees;
            set => _data.ShowOnlyFollowees = value;
        }
        public static bool ShowRepostsOnlyFromFollowees
        {
            get => _data.ShowRepostsOnlyFromFollowees;
            set => _data.ShowRepostsOnlyFromFollowees = value;
        }
        public static bool ShowOnlySelectedLanguage
        {
            get => _data.ShowOnlySelectedLanguage;
            set => _data.ShowOnlySelectedLanguage = value;
        }
        public static bool ShowDAN
        {
            get => _data.ShowDAN;
            set => _data.ShowDAN = value;
        }
        public static bool ShowDEU
        {
            get => _data.ShowDEU;
            set => _data.ShowDEU = value;
        }
        public static bool ShowENG
        {
            get => _data.ShowENG;
            set => _data.ShowENG = value;
        }
        public static bool ShowFRA
        {
            get => _data.ShowFRA;
            set => _data.ShowFRA = value;
        }
        public static bool ShowITA
        {
            get => _data.ShowITA;
            set => _data.ShowITA = value;
        }
        public static bool ShowJPN
        {
            get => _data.ShowJPN;
            set => _data.ShowJPN = value;
        }
        public static bool ShowKOR
        {
            get => _data.ShowKOR;
            set => _data.ShowKOR = value;
        }
        public static bool ShowNLD
        {
            get => _data.ShowNLD;
            set => _data.ShowNLD = value;
        }
        public static bool ShowNOR
        {
            get => _data.ShowNOR;
            set => _data.ShowNOR = value;
        }
        public static bool ShowPOR
        {
            get => _data.ShowPOR;
            set => _data.ShowPOR = value;
        }
        public static bool ShowRUS
        {
            get => _data.ShowRUS;
            set => _data.ShowRUS = value;
        }
        public static bool ShowSPA
        {
            get => _data.ShowSPA;
            set => _data.ShowSPA = value;
        }
        public static bool ShowSWE
        {
            get => _data.ShowSWE;
            set => _data.ShowSWE = value;
        }
        public static bool ShowZHO
        {
            get => _data.ShowZHO;
            set => _data.ShowZHO = value;
        }
        public static bool MinimizeToTray
        {
            get => _data.MinimizeToTray;
            set => _data.MinimizeToTray = value;
        }
        public static bool SendDSSTP
        {
            get => _data.SendDSSTP;
            set => _data.SendDSSTP = value;
        }
        public static bool AddClient
        {
            get => _data.AddClient;
            set => _data.AddClient = value;
        }

        public static int AvatarSize
        {
            get => _data.AvatarSize;
            set => _data.AvatarSize = value;
        }
        public static string GridColor
        {
            get => _data.GridColor;
            set => _data.GridColor = value;
        }
        public static string ReactionColor
        {
            get => _data.ReactionColor;
            set => _data.ReactionColor = value;
        }
        public static string ReplyColor
        {
            get => _data.ReplyColor;
            set => _data.ReplyColor = value;
        }
        public static string RepostColor
        {
            get => _data.RepostColor;
            set => _data.RepostColor = value;
        }
        public static string CWColor
        {
            get => _data.CWColor;
            set => _data.CWColor = value;
        }
        public static string WebViewUrl
        {
            get => _data.WebViewUrl;
            set => _data.WebViewUrl = value;
        }

        public static Point PostBarLocation
        {
            get => _data.PostBarLocation;
            set => _data.PostBarLocation = value;
        }
        public static Size PostBarSize
        {
            get => _data.PostBarSize;
            set => _data.PostBarSize = value;
        }
        public static string PictureUploadUrl
        {
            get => _data.PictureUploadUrl;
            set => _data.PictureUploadUrl = value;
        }

        public static Point WebViewLocation
        {
            get => _data.WebViewLocation;
            set => _data.WebViewLocation = value;
        }
        public static Size WebViewSize
        {
            get => _data.WebViewSize;
            set => _data.WebViewSize = value;
        }
        #endregion

        #region 設定ファイル操作
        /// <summary>
        /// 設定ファイル読み込み
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool Load(string path)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(Data));
                var xmlSettings = new XmlReaderSettings();
                using var streamReader = new StreamReader(path, Encoding.UTF8);
                using var xmlReader = XmlReader.Create(streamReader, xmlSettings);
                _data = serializer.Deserialize(xmlReader) as Data ?? _data;
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 設定ファイル書き込み
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool Save(string path)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(Data));
                using var streamWriter = new StreamWriter(path, false, Encoding.UTF8);
                serializer.Serialize(streamWriter, _data);
                streamWriter.Flush();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
        #endregion
    }
}
