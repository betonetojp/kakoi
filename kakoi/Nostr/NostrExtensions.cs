using System.Net.WebSockets;
using NBitcoin.Secp256k1;
using NNostr.Client.JsonConverters;

namespace NNostr.Client
{
    public static class NostrExtensions
    {
        public static string ToIdPreimage(this NostrEvent nostrEvent, bool withoutId)
        {
            var content = StringEscaperJsonConverter.JavaScriptStringEncode(nostrEvent.Content ?? string.Empty, false);
            return
                $"[{(withoutId ? 0 : $"\"{nostrEvent.Id}\"")},\"{nostrEvent.PublicKey}\",{nostrEvent.CreatedAt?.ToUnixTimeSeconds()},{nostrEvent.Kind},[{string.Join(',', nostrEvent.Tags.Select(tag => tag))}],\"{content}\"]";
        }

        public static string ComputeEventId(this string eventJson)
        {
            return eventJson.ComputeSha256Hash().AsSpan().ToHex();
        }

        public static string ComputeId(this NostrEvent nostrEvent)
        {
            return nostrEvent.ToIdPreimage(true).ComputeEventId();
        }

        public static string ComputeSignature(this NostrEvent nostrEvent, ECPrivKey priv)
        {
            return nostrEvent.ToIdPreimage(true).ComputeBIP340Signature(priv);
        }

        public static Task<NostrEvent> ComputeIdAndSignAsync(this NostrEvent nostrEvent, ECPrivKey priv)
        {
            var expectedPubkey = priv.CreateXOnlyPubKey().ToHex();
            if (!string.IsNullOrEmpty(nostrEvent.PublicKey) && nostrEvent.PublicKey != expectedPubkey)
            {
                throw new ArgumentException("Public key of the event does not match sender of this event", nameof(priv));
            }
            nostrEvent.PublicKey = expectedPubkey;
            nostrEvent.CreatedAt ??= DateTimeOffset.UtcNow;

            nostrEvent.Id = nostrEvent.ComputeId();
            nostrEvent.Signature = nostrEvent.ComputeSignature(priv);

            return Task.FromResult(nostrEvent);
        }

        public static bool Verify(this NostrEvent nostrEvent)
        {
            var hash = nostrEvent.ToIdPreimage(true).ComputeSha256Hash();
            if (hash.AsSpan().ToHex() != nostrEvent.Id)
            {
                return false;
            }

            var pub = nostrEvent.GetPublicKey();
            if (!SecpSchnorrSignature.TryCreate(Convert.FromHexString(nostrEvent.Signature), out var sig))
            {
                return false;
            }

            return pub.SigVerifyBIP340(sig, hash);
        }

        public static ECXOnlyPubKey GetPublicKey(this NostrEvent nostrEvent)
        {
            return ParsePubKey(nostrEvent.PublicKey);
        }

        public static ECPrivKey ParseKey(string key)
        {
            return ParseKey(key.FromHex().ToArray());
        }

        public static ECPrivKey ParseKey(byte[] key)
        {
            return ECPrivKey.Create(key);
        }

        public static ECXOnlyPubKey ParsePubKey(string key)
        {
            return ParsePubKey(key.FromHex().ToArray());
        }

        public static ECXOnlyPubKey ParsePubKey(byte[] key)
        {
            return Context.Instance.CreateXOnlyPubKey(key);
        }

        public static string ToHex(this ECPrivKey key)
        {
            Span<byte> output = stackalloc byte[32];
            key.WriteToSpan(output);
            return output.ToHex();
        }

        public static byte[] ToBytes(this ECPrivKey key)
        {
            Span<byte> output = stackalloc byte[32];
            key.WriteToSpan(output);
            return output.ToArray();
        }

        public static string ToHex(this ECXOnlyPubKey key)
        {
            return key.ToBytes().AsSpan().ToHex();
        }

        public static string[] GetTaggedEvents(this NostrEvent e)
        {
            return e.GetTaggedData("e");
        }

        public static string[] GetTaggedPublicKeys(this NostrEvent e)
        {
            return e.GetTaggedData("p");
        }

        public static string[] GetTaggedData(this NostrEvent e, string identifier)
        {
            return e.Tags.Where(tag => tag.TagIdentifier == identifier).Select(tag => tag.Data.First()).ToArray();
        }

        public static async Task SendEventsAndWaitUntilReceived(this INostrClient client, NostrEvent[] events,
            CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var evtIds = events.Select(e => e.Id).ToHashSet();
            var subId = Guid.NewGuid().ToString();
            await client.CreateSubscription(subId,
            [
                new NostrSubscriptionFilter
                {
                    Ids = [.. evtIds]
                }
            ], cancellationToken);

            void OnClientOnOkReceived(object? sender, (string eventId, bool success, string messafe) args)
            {
                if (evtIds.Remove(args.eventId) && evtIds.Count == 0)
                {
                    tcs.TrySetResult(true);
                }
            }

            void OnClientOnEventsReceived(object? sender, (string subscriptionId, NostrEvent[] events) args)
            {
                if (args.subscriptionId == subId)
                {
                    foreach (var nostrEvent in args.events)
                    {
                        if (evtIds.Remove(nostrEvent.Id) && evtIds.Count == 0)
                        {
                            tcs.TrySetResult(true);
                            break;
                        }
                    }
                }
            }

            void NostrClientOnStateChanged(object? sender, WebSocketState? e)
            {
                if (e is not WebSocketState.Open)
                    tcs.TrySetResult(true);
            }

            void NostrClientOnStateChanged2(object? sender, (Uri, WebSocketState?) valueTuple)
            {
                if (sender is CompositeNostrClient client2 && client2.States.Values.All(state => state is not WebSocketState.Open))
                    tcs.TrySetResult(true);
            }

            client.EventsReceived += OnClientOnEventsReceived;
            client.OkReceived += OnClientOnOkReceived;

            if (client is NostrClient nostrClient)
            {
                nostrClient.StateChanged += NostrClientOnStateChanged;
            }
            else if (client is CompositeNostrClient nostrClient2)
            {
                nostrClient2.StateChanged += NostrClientOnStateChanged2;
            }

            try
            {
                foreach (var evt in events)
                {
                    await client.PublishEvent(evt, cancellationToken);
                }

                await tcs.Task.WaitAsync(cancellationToken);
            }
            finally
            {
                client.OkReceived -= OnClientOnOkReceived;
                client.EventsReceived -= OnClientOnEventsReceived;
                if (client is NostrClient nostrClient2)
                {
                    nostrClient2.StateChanged -= NostrClientOnStateChanged;
                }
                else if (client is CompositeNostrClient nostrClient3)
                {
                    nostrClient3.StateChanged -= NostrClientOnStateChanged2;
                }
                _ = client.CloseSubscription(subId);
            }
        }
    }
}
