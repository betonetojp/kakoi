﻿using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace kakoi
{
    public class Setting
    {
        private static Data _data = new();

        #region データクラス
        /// <summary>
        /// 設定データクラス
        /// </summary>
        public class Data
        {
            public Point Location { get; set; }
            public Size Size { get; set; } = new Size(320, 320);
            public bool TopMost { get; set; } = false;
            public int CutLength { get; set; } = 40;
            public int CutNameLength { get; set; } = 8;
            public double Opacity { get; set; } = 1.00;
            public bool AddClient { get; set; } = true;
            public bool ShowOnlyJapanese { get; set; } = false;
            public bool ShowOnlyFollowees { get; set; } = false;
            public string NokakoiKey { get; set; } = string.Empty;
            public bool SendDSSTP { get; set; } = false;
            public Point PostBarLocation { get; set; }
            public Size PostBarSize { get; set; } = new Size(300, 132);
            public Point WebLocation { get; set; }
            public Size WebSize { get; set; } = new Size(320, 320);
            public int NameColumnWidth { get; set; } = 60;
            public string GridColor { get; set; } = "#FF1493";
            public string PictureUploadUrl { get; set; } = "https://nikolat.github.io/nostr-learn-nip96/";
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
        public static bool TopMost
        {
            get => _data.TopMost;
            set => _data.TopMost = value;
        }
        public static int CutLength
        {
            get => _data.CutLength;
            set => _data.CutLength = value;
        }
        public static int CutNameLength
        {
            get => _data.CutNameLength;
            set => _data.CutNameLength = value;
        }
        public static double Opacity
        {
            get => _data.Opacity;
            set => _data.Opacity = value;
        }
        public static bool AddClient
        {
            get => _data.AddClient;
            set => _data.AddClient = value;
        }
        public static bool ShowOnlyJapanese
        {
            get => _data.ShowOnlyJapanese;
            set => _data.ShowOnlyJapanese = value;
        }
        public static bool ShowOnlyFollowees
        {
            get => _data.ShowOnlyFollowees;
            set => _data.ShowOnlyFollowees = value;
        }
        public static string NokakoiKey
        {
            get => _data.NokakoiKey;
            set => _data.NokakoiKey = value;
        }
        public static bool SendDSSTP
        {
            get => _data.SendDSSTP;
            set => _data.SendDSSTP = value;
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
        public static Point WebLocation
        {
            get => _data.WebLocation;
            set => _data.WebLocation = value;
        }
        public static Size WebSize
        {
            get => _data.WebSize;
            set => _data.WebSize = value;
        }
        public static int NameColumnWidth
        {
            get => _data.NameColumnWidth;
            set => _data.NameColumnWidth = value;
        }
        public static string GridColor
        {
            get => _data.GridColor;
            set => _data.GridColor = value;
        }
        public static string PictureUploadUrl
        {
            get => _data.PictureUploadUrl;
            set => _data.PictureUploadUrl = value;
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
