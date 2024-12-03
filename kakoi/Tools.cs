using CredentialManagement;
using NBitcoin.Secp256k1;
using NNostr.Client;
using NNostr.Client.JsonConverters;
using NNostr.Client.Protocols;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace kakoi
{
    public class User
    {
        [JsonPropertyName("mute")]
        public bool Mute { get; set; }
        [JsonPropertyName("last_activity")]
        public DateTime? LastActivity { get; set; }
        [JsonPropertyName("petname")]
        public string? PetName { get; set; }
        [JsonPropertyName("display_name")]
        public string? DisplayName { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("nip05")]
        public string? Nip05 { get; set; }
        [JsonPropertyName("picture")]
        public string? Picture { get; set; }
        [JsonPropertyName("created_at")]
        [JsonConverter(typeof(UnixTimestampSecondsJsonConverter))]
        public DateTimeOffset? CreatedAt { get; set; }
        //[JsonPropertyName("language")] 
        //public string? Language { get; set; }
    }

    public class Relay
    {
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }
        [JsonPropertyName("url")]
        public string? Url { get; set; }
    }

    public class Emoji
    {
        [JsonPropertyName("content")]
        public string? Content { get; set; }
        [JsonPropertyName("url")]
        public string? Url { get; set; }
    }

    public class Client
    {
        public string? Name { get; set; }
        public string? ColorCode { get; set; }
    }

    public static class Tools
    {
        private static readonly string _usersJsonPath = Path.Combine(Application.StartupPath, "users.json");
        private static readonly string _relaysJsonPath = Path.Combine(Application.StartupPath, "relays.json");
        private static readonly string _emojisJsonPath = Path.Combine(Application.StartupPath, "emojis.json");
        private static readonly string _clientsJsonPath = Path.Combine(Application.StartupPath, "clients.json");

        private static JsonSerializerOptions GetOption()
        {
            // ユニコードのレンジ指定で日本語も正しく表示、インデントされるように指定
            var options = new JsonSerializerOptions
            {
                //Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                Encoder = new NoEscapingJsonEncoder(),
                WriteIndented = true,
            };
            return options;
        }

        #region ユーザー
        /// <summary>
        /// ユーザー辞書をファイルに保存する
        /// </summary>
        /// <param name="users">ユーザー辞書</param>
        internal static void SaveUsers(Dictionary<string, User?> users)
        {
            // users.jsonに保存
            try
            {
                var jsonContent = JsonSerializer.Serialize(users, GetOption());
                File.WriteAllText(_usersJsonPath, jsonContent);
            }
            catch (JsonException e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// ファイルからユーザー辞書を読み込む
        /// </summary>
        /// <returns>ユーザー辞書</returns>
        internal static Dictionary<string, User?> LoadUsers()
        {
            // users.jsonを読み込み
            if (!File.Exists(_usersJsonPath))
            {
                return [];
            }
            try
            {
                var jsonContent = File.ReadAllText(_usersJsonPath);
                var users = JsonSerializer.Deserialize<Dictionary<string, User?>>(jsonContent, GetOption());
                if (users != null)
                {
                    return users;
                }
                return [];
            }
            catch (JsonException e)
            {
                Debug.WriteLine(e.Message);
                return [];
            }
        }

        /// <summary>
        /// JSONからユーザーを作成
        /// </summary>
        /// <param name="contentJson">kind:0のcontent JSON</param>
        /// <param name="createdAt">kind:0の作成日時</param>
        /// <returns>ユーザー</returns>
        public static User? JsonToUser(string contentJson, DateTimeOffset? createdAt, bool shouldMuteMostr = true)
        {
            if (string.IsNullOrEmpty(contentJson))
            {
                return null;
            }
            try
            {
                var user = JsonSerializer.Deserialize<User>(contentJson, GetOption());
                if (user != null)
                {
                    user.CreatedAt = createdAt;
                    if (shouldMuteMostr && user.Nip05 != null && user.Nip05.Contains("mostr"))
                    {
                        user.Mute = true;
                    }
                }
                return user;
            }
            catch (JsonException e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }
        #endregion

        #region リレー
        internal static void SaveRelays(List<Relay> relays)
        {
            // relays.jsonに保存
            try
            {
                var jsonContent = JsonSerializer.Serialize(relays, GetOption());
                File.WriteAllText(_relaysJsonPath, jsonContent);
            }
            catch (JsonException e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        internal static List<Relay> LoadRelays()
        {
            List<Relay> defaultRelays = [
                new Relay { Enabled = true, Url = "wss://yabu.me/" },
                new Relay { Enabled = true, Url = "wss://r.kojira.io/" },
                new Relay { Enabled = true, Url = "wss://relay-jp.nostr.wirednet.jp/" },
                new Relay { Enabled = true, Url = "wss://nos.lol/" },
                new Relay { Enabled = true, Url = "wss://relay.damus.io/" },
                new Relay { Enabled = true, Url = "wss://relay.nostr.band/" },
                ];

            // relays.jsonを読み込み
            if (!File.Exists(_relaysJsonPath))
            {
                return defaultRelays;
            }
            try
            {
                var jsonContent = File.ReadAllText(_relaysJsonPath);
                var relays = JsonSerializer.Deserialize<List<Relay>>(jsonContent, GetOption());
                if (relays != null)
                {
                    return relays;
                }
                return [];
            }
            catch (JsonException e)
            {
                Debug.WriteLine(e.Message);
                return [];
            }
        }

        internal static Uri[] GetEnabledRelays()
        {
            return GetEnabledRelays(LoadRelays());
        }

        internal static Uri[] GetEnabledRelays(List<Relay> relays)
        {
            List<Uri> enabledRelays = [];
            foreach (var relay in relays)
            {
                if (relay.Enabled && relay.Url != null)
                {
                    enabledRelays.Add(new Uri(relay.Url));
                }
            }
            return [.. enabledRelays];
        }
        #endregion

        #region 絵文字
        internal static void SaveEmojis(List<Emoji> emojis)
        {
            // emojis.jsonに保存
            try
            {
                var jsonContent = JsonSerializer.Serialize(emojis, GetOption());
                File.WriteAllText(_emojisJsonPath, jsonContent);
            }
            catch (JsonException e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        internal static List<Emoji> LoadEmojis()
        {
            List<Emoji> defaultemojis = [
                new Emoji { Content = "+" },
                new Emoji { Content = "✅" },
                new Emoji { Content = "👀" },
                new Emoji { Content = "🤔" },
                new Emoji { Content = "🎉" },
                new Emoji { Content = "🫂" },
                new Emoji { Content = "nice", Url = "https://nokakoi.com/media/kakoi.png" },
                new Emoji { Content = "kusa", Url = "https://image.nostr.build/18fa1ce2d056e3d28c05b566969ea7c0a8de4cf5c2cd9422242278ff53910a9d.png" },
                ];

            // emojis.jsonを読み込み
            if (!File.Exists(_emojisJsonPath))
            {
                SaveEmojis(defaultemojis);
                return defaultemojis;
            }
            try
            {
                var jsonContent = File.ReadAllText(_emojisJsonPath);
                var emojis = JsonSerializer.Deserialize<List<Emoji>>(jsonContent, GetOption());
                if (emojis != null)
                {
                    return emojis;
                }
                return [];
            }
            catch (JsonException e)
            {
                Debug.WriteLine(e.Message);
                return [];
            }
        }
        #endregion

        #region クライアント
        internal static void SaveClients(List<Client> clients)
        {
            // clients.jsonに保存
            try
            {
                var jsonContent = JsonSerializer.Serialize(clients, GetOption());
                File.WriteAllText(_clientsJsonPath, jsonContent);
            }
            catch (JsonException e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        internal static List<Client> LoadClients()
        {
            List<Client> defaultClients = [
                // オリジナルカラー
                //new Client { Name = "kakoi", ColorCode = "#E4007D" },
                //new Client { Name = "lumilumi", ColorCode = "#DC7724" },
                //new Client { Name = "noStrudel", ColorCode = "#8EB61A" },
                // 50%カラー
                new Client { Name = "nokakoi", ColorCode = "#F280BE" },
                new Client { Name = "nokako", ColorCode = "#F280BE" },
                new Client { Name = "kakoi", ColorCode = "#F280BE" },
                new Client { Name = "lumilumi", ColorCode = "#EEBB92" },
                new Client { Name = "noStrudel", ColorCode = "#C7DB8D" },
                ];

            // clients.jsonを読み込み
            if (!File.Exists(_clientsJsonPath))
            {
                SaveClients(defaultClients);
                return defaultClients;
            }
            try
            {
                var jsonContent = File.ReadAllText(_clientsJsonPath);
                var clients = JsonSerializer.Deserialize<List<Client>>(jsonContent, GetOption());
                if (clients != null)
                {
                    return clients;
                }
                return [];
            }
            catch (JsonException e)
            {
                Debug.WriteLine(e.Message);
                return [];
            }
        }
        #endregion

        #region 色
        public static Color HexToColor(string hex)
        {
            try
            {
                hex = hex.TrimStart('#');

                int r = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                int g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                int b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

                return Color.FromArgb(r, g, b);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return Color.Silver;
            }
        }

        public static string ColorToHex(Color color)
        {
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }
        #endregion

        #region Nostrツール
        /// <summary>
        /// nsecからnpubを取得する
        /// </summary>
        /// <param name="nsec">nsec</param>
        /// <returns>npub</returns>
        public static string GetNpub(this string nsec)
        {
            try
            {
                return nsec.FromNIP19Nsec().CreateXOnlyPubKey().ToNIP19();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return string.Empty;
            }
        }

        /// <summary>
        /// nsecからnpub(HEX)を取得する
        /// </summary>
        /// <param name="nsec">nsec</param>
        /// <returns>npub(HEX)</returns>
        public static string GetNpubHex(this string nsec)
        {
            try
            {
                return nsec.FromNIP19Nsec().CreateXOnlyPubKey().ToHex();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return string.Empty;
            }
        }

        /// <summary>
        /// nsecからnsec(HEX)を取得する
        /// </summary>
        /// <param name="nsec"></param>
        /// <returns>nsec(HEX)</returns>
        public static string GetNsecHex(this string nsec)
        {
            try
            {
                return nsec.FromNIP19Nsec().ToHex();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return string.Empty;
            }
        }


        /// <summary>
        /// npubまたはnprofileのpubkeyをHEXに変換する
        /// </summary>
        /// <param name="npubOrNprofile">npub</param>
        /// <returns>HEX</returns>
        public static string ConvertToHex(this string npubOrNprofile)
        {
            try
            {
                // npubが"npub"で始まるとき
                if (npubOrNprofile.StartsWith("npub"))
                {
                    return npubOrNprofile.FromNIP19Npub().ToHex();
                }
                // npubが"nprofile"で始まるとき
                else if (npubOrNprofile.StartsWith("nprofile"))
                {
                    var profile = (NIP19.NosteProfileNote?)npubOrNprofile.FromNIP19Note();
                    if (profile != null)
                    {
                        return profile.PubKey;
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return string.Empty;
            }
        }

        /// <summary>
        /// HEXをnpubに変換する
        /// </summary>
        /// <param name="hex">HEX</param>
        /// <returns>npub</returns>
        public static string ConvertToNpub(this string hex)
        {
            try
            {
                return ECXOnlyPubKey.Create(hex.FromHex()).ToNIP19();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return string.Empty;
            }
        }
        #endregion

        #region DPAPI暗号化
        public static string EncryptPassword(string password)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] encryptedBytes = ProtectedData.Protect(passwordBytes, null, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encryptedBytes);
        }

        public static string DecryptPassword(string encryptedPassword)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedPassword);
            byte[] decryptedBytes = ProtectedData.Unprotect(encryptedBytes, null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(decryptedBytes);
        }
        #endregion

        #region Windows資格情報管理
        public static void SavePassword(string target, string username, string password)
        {
            using var cred = new Credential();
            cred.Target = target;
            cred.Username = username;
            cred.Password = EncryptPassword(password); // パスワードを暗号化
            cred.Type = CredentialType.Generic;
            cred.PersistanceType = PersistanceType.LocalComputer;
            cred.Save();
        }

        public static string LoadPassword(string target)
        {
            using var cred = new Credential();
            cred.Target = target;
            cred.Load();
            return DecryptPassword(cred.Password); // パスワードを復号化
        }

        public static void DeletePassword(string target)
        {
            var cred = new Credential { Target = target };
            cred.Delete();
        }
        #endregion
    }
}
