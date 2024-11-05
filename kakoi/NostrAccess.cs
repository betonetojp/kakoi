using NNostr.Client;
using System.Net.WebSockets;

namespace kakoi
{
    public static class NostrAccess
    {
        #region フィールド
        /// <summary>
        /// タイムライン購読をさかのぼる時間
        /// </summary>
        private static readonly TimeSpan _timeSpan = new(0, 0, 0, 0);

        /// <summary>
        /// 複数クライアント
        /// </summary>
        private static CompositeNostrClient? _clients;
        /// <summary>
        /// 接続リレー配列
        /// </summary>
        private static Uri[] _relays = [];

        /// <summary>
        /// タイムライン購読ID
        /// </summary>
        private static readonly string _subscriptionId = Guid.NewGuid().ToString("N");
        /// <summary>
        /// フォロイー購読ID
        /// </summary>
        private static readonly string _getFolloweesSubscriptionId = Guid.NewGuid().ToString("N");
        /// <summary>
        /// プロフィール購読ID
        /// </summary>
        private static readonly string _getProfilesSubscriptionId = Guid.NewGuid().ToString("N");
        #endregion

        #region プロパティ
        /// <summary>
        /// 複数クライアント
        /// </summary>
        public static CompositeNostrClient? Clients { get => _clients; set => _clients = value; }
        /// <summary>
        /// 接続リレー配列
        /// </summary>
        public static Uri[] Relays { get => _relays; set => _relays = value; }

        /// <summary>
        /// タイムライン購読ID
        /// </summary>
        public static string SubscriptionId => _subscriptionId;
        /// <summary>
        /// フォロイー購読ID
        /// </summary>
        public static string GetFolloweesSubscriptionId => _getFolloweesSubscriptionId;
        /// <summary>
        /// プロフィール購読ID
        /// </summary>
        public static string GetProfilesSubscriptionId => _getProfilesSubscriptionId;
        #endregion

        #region 接続処理
        /// <summary>
        /// 接続処理
        /// </summary>
        /// <returns></returns>
        public static async Task<int> ConnectAsync()
        {
            if (null == _clients)
            {
                _relays = Tools.GetEnabledRelays();
                if (0 == _relays.Length)
                {
                    return 0;
                }

                _clients = new CompositeNostrClient(_relays);

                await _clients.Connect();
            }
            else
            {
                var hasClosed = false;
                foreach (var state in _clients.States)
                {
                    if (WebSocketState.CloseReceived < state.Value)
                    {
                        hasClosed = true;
                        break;
                    }
                }
                if (hasClosed)
                {
                    await _clients.Connect();
                }
            }
            return _clients.States.Count;
        }
        #endregion

        #region タイムライン購読処理
        /// <summary>
        /// タイムライン購読処理
        /// </summary>
        public static async Task SubscribeAsync()
        {
            if (null == _clients)
            {
                return;
            }

            await _clients.CreateSubscription(
                    _subscriptionId,
                    [
                        new NostrSubscriptionFilter()
                        {
                            Kinds = [1,6,7,16],
                            Since = DateTimeOffset.Now - _timeSpan,
                        }
                    ]
                 );
        }
        #endregion

        #region フォロイー購読処理
        /// <summary>
        /// フォロイー購読処理
        /// </summary>
        /// <param name="author"></param>
        public static async Task SubscribeFollowsAsync(string author)
        {
            if (null == _clients)
            {
                return;
            }

            await _clients.CreateSubscription(
                    _getFolloweesSubscriptionId,
                    [
                        new NostrSubscriptionFilter
                        {
                            Kinds = [3],
                            Authors = [author]
                        }
                    ]
                 );
        }
        #endregion

        #region プロフィール購読処理
        /// <summary>
        /// プロフィール購読処理
        /// </summary>
        /// <param name="authors"></param>
        public static async Task SubscribeProfilesAsync(string[] authors)
        {
            if (null == _clients)
            {
                return;
            }

            await _clients.CreateSubscription(
                    _getProfilesSubscriptionId,
                    [
                        new NostrSubscriptionFilter
                        {
                            Kinds = [0],
                            Authors = authors
                        }
                    ]
                 );

            // 待機
            await Task.Delay(500);
        }
        #endregion

        #region 購読解除処理
        /// <summary>
        /// 購読解除処理
        /// </summary>
        public static void CloseSubscriptions()
        {
            if (null != _clients)
            {
                _ = _clients.CloseSubscription(_subscriptionId);
                _ = _clients.CloseSubscription(_getFolloweesSubscriptionId);
                _ = _clients.CloseSubscription(_getProfilesSubscriptionId);
            }
        }
        #endregion

        #region 切断処理
        /// <summary>
        /// 切断処理
        /// </summary>
        public static void DisconnectAndDispose()
        {
            if (null != _clients)
            {
                _ = _clients.Disconnect();
                _clients.Dispose();
                _clients = null;
            }
        }
        #endregion
    }
}
