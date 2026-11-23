using System.Text;
using NBitcoin.Secp256k1;

namespace NNostr.Client
{
    public static class StringExtensions
    {
        public static string ComputeBIP340Signature(this string rawData, ECPrivKey privKey)
        {
            Span<byte> buf = stackalloc byte[64];
            using var sha256 = System.Security.Cryptography.SHA256.Create();

            sha256.TryComputeHash(Encoding.UTF8.GetBytes(rawData), buf, out _);
            privKey.SignBIP340(buf[..32]).WriteToSpan(buf);

            return buf.ToHex();
        }

        public static byte[] ComputeSha256Hash(this string rawData)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
        }

        public static string ToHex(this Span<byte> bytes)
        {
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }

        public static Span<byte> FromHex(this string hex)
        {
            return Convert.FromHexString(hex).AsSpan();
        }
    }
}
