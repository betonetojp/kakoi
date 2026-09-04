using NNostr.Client;
using System.Diagnostics;
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
        /// 接続状態リスト
        /// </summary>
        private static List<string> _relayStatusList = [];

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
        /// 接続状態リスト
        /// </summary>
        public static List<string> RelayStatusList => _relayStatusList;

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
            if (_clients == null)
            {
                _relays = Tools.GetEnabledRelays();
                if (0 == _relays.Length)
                {
                    _relayStatusList.Clear();
                    return 0;
                }

                _clients = new CompositeNostrClient(_relays);

                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
                try
                {
                    await _clients.Connect(cts.Token);
                }
                catch (OperationCanceledException)
                {
                    Debug.WriteLine("接続がタイムアウトしました。");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"接続中にエラーが発生しました: {ex.Message}");
                }
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
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
                    try
                    {
                        await _clients.Connect(cts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        Debug.WriteLine("再接続がタイムアウトしました。");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"再接続中にエラーが発生しました: {ex.Message}");
                    }
                }
            }

            // 接続状態を確認し、失敗したリレーを記録
            _relayStatusList.Clear();
            int openCount = 0;
            foreach (var relay in _relays)
            {
                if (_clients.States.TryGetValue(relay, out var state))
                {
                    if (state != WebSocketState.Open)
                    {
                        Debug.WriteLine($"リレー {relay} に接続できませんでした。状態: {state}");
                        _relayStatusList.Add($"❌ {relay}");
                    }
                    else
                    {
                        Debug.WriteLine($"リレー {relay} に接続しました。状態: {state}");
                        _relayStatusList.Add(relay.ToString());
                        openCount++;
                    }
                }
            }

            return openCount;
        }
        #endregion

        #region タイムライン購読処理
        /// <summary>
        /// タイムライン購読処理
        /// </summary>
        public static async Task SubscribeAsync()
        {
            if (_clients == null)
            {
                return;
            }

            await _clients.CreateSubscription(
                _subscriptionId,
                [
                        new NostrSubscriptionFilter
                        {
                            Kinds = [1, 6, 7, 16],
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
            if (_clients == null)
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
            if (_clients == null)
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
        }
        #endregion

        #region 購読解除処理
        /// <summary>
        /// 購読解除処理
        /// </summary>
        public static void CloseSubscriptions()
        {
            if (_clients != null)
            {
                _ = _clients.CloseSubscription(_subscriptionId);
                _ = _clients.CloseSubscription(_getFolloweesSubscriptionId);
                _ = _clients.CloseSubscription(_getProfilesSubscriptionId);
            }
        }
        #endregion

        #region インデクサリレーからのプロフィール取得
        private static readonly Uri[] _indexerRelays = [
            new Uri("wss://directory.yabu.me")
        ];
        private static CompositeNostrClient? _indexerClients;

        private static async Task EnsureIndexerConnectedAsync(CancellationToken token = default)
        {
            if (_indexerClients == null)
            {
                _indexerClients = new CompositeNostrClient(_indexerRelays);
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                try
                {
                    await _indexerClients.Connect(cts.Token);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"インデクサ接続エラー: {ex.Message}");
                }
            }
            else
            {
                var hasClosed = false;
                foreach (var state in _indexerClients.States)
                {
                    if (WebSocketState.CloseReceived < state.Value)
                    {
                        hasClosed = true;
                        break;
                    }
                }
                if (hasClosed)
                {
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                    try
                    {
                        await _indexerClients.Connect(cts.Token);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"インデクサ再接続エラー: {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// インデクサリレーから最新プロフィールを取得する
        /// </summary>
        /// <param name="pubkey">取得対象の公開鍵Hex</param>
        /// <param name="timeoutMs">タイムアウト(ms)</param>
        /// <returns>最新のkind:0イベント</returns>
        public static async Task<NostrEvent?> FetchProfileFromIndexerAsync(string pubkey, int timeoutMs = 3000)
        {
            if (string.IsNullOrEmpty(pubkey)) return null;

            try
            {
                await EnsureIndexerConnectedAsync();
                if (_indexerClients == null) return null;

                var subId = Guid.NewGuid().ToString("N");
                var tcs = new TaskCompletionSource<NostrEvent?>(TaskCreationOptions.RunContinuationsAsynchronously);
                NostrEvent? latestEvent = null;

                void OnEventsReceived(object? sender, (string subscriptionId, NostrEvent[] events) args)
                {
                    if (args.subscriptionId == subId)
                    {
                        foreach (var ev in args.events)
                        {
                            if (ev.Kind == 0 && ev.PublicKey == pubkey)
                            {
                                if (latestEvent == null || (ev.CreatedAt > latestEvent.CreatedAt))
                                {
                                    latestEvent = ev;
                                }
                            }
                        }
                    }
                }

                void OnEoseReceived(object? sender, string subscriptionId)
                {
                    if (subscriptionId == subId)
                    {
                        tcs.TrySetResult(latestEvent);
                    }
                }

                _indexerClients.EventsReceived += OnEventsReceived;
                _indexerClients.EoseReceived += OnEoseReceived;

                using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(timeoutMs));
                cts.Token.Register(() => tcs.TrySetResult(latestEvent));

                try
                {
                    await _indexerClients.CreateSubscription(
                        subId,
                        [
                            new NostrSubscriptionFilter
                            {
                                Kinds = [0],
                                Authors = [pubkey],
                                Limit = 1
                            }
                        ],
                        cts.Token
                    );

                    var result = await tcs.Task;
                    return result ?? latestEvent;
                }
                finally
                {
                    _indexerClients.EventsReceived -= OnEventsReceived;
                    _indexerClients.EoseReceived -= OnEoseReceived;
                    _ = _indexerClients.CloseSubscription(subId);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"FetchProfileFromIndexerAsync エラー: {ex.Message}");
                return null;
            }
        }
        #endregion

        #region 切断処理
        /// <summary>
        /// 切断処理
        /// </summary>
        public static void DisconnectAndDispose()
        {
            if (_clients != null)
            {
                _ = _clients.Disconnect();
                _clients.Dispose();
                _clients = null;
            }
            if (_indexerClients != null)
            {
                _ = _indexerClients.Disconnect();
                _indexerClients.Dispose();
                _indexerClients = null;
            }
        }
        #endregion
    }
}
