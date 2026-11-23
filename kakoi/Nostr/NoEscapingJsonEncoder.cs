using System.Text;
using System.Text.Encodings.Web;

namespace NNostr.Client
{
    /// <summary>
    /// 絵文字や日本語などの非ASCII文字をエスケープせず、RFC 8259 / NIP-01 で必須の文字のみエスケープするエンコーダー
    /// </summary>
    public class NoEscapingJsonEncoder : JavaScriptEncoder
    {
        public static JavaScriptEncoder NoEscaping { get; } = new NoEscapingJsonEncoder();

        public override int MaxOutputCharactersPerInputCharacter => 6;

        public override unsafe int FindFirstCharacterToEncode(char* text, int textLength)
        {
            for (int i = 0; i < textLength; i++)
            {
                char c = text[i];
                if (c <= 0x1F || c == '\"' || c == '\\')
                {
                    return i;
                }
            }
            return -1;
        }

        public override unsafe bool TryEncodeUnicodeScalar(int unicodeScalar, char* buffer, int bufferLength, out int numberOfCharactersWritten)
        {
            char? escapeChar = unicodeScalar switch
            {
                '\b' => 'b',
                '\t' => 't',
                '\n' => 'n',
                '\f' => 'f',
                '\r' => 'r',
                '\"' => '\"',
                '\\' => '\\',
                _ => null,
            };

            if (escapeChar is { } notNull)
            {
                if (bufferLength < 2)
                {
                    numberOfCharactersWritten = 0;
                    return false;
                }

                buffer[0] = '\\';
                buffer[1] = notNull;
                numberOfCharactersWritten = 2;
                return true;
            }

            if (unicodeScalar is >= 0 and <= 0x1F)
            {
                if (bufferLength < 6)
                {
                    numberOfCharactersWritten = 0;
                    return false;
                }

                buffer[0] = '\\';
                buffer[1] = 'u';
                ((ushort)unicodeScalar).TryFormat(new Span<char>(buffer + 2, 4), out _, "x4");
                numberOfCharactersWritten = 6;
                return true;
            }

            if (Rune.TryCreate(unicodeScalar, out var rune))
            {
                if (rune.Utf16SequenceLength > bufferLength)
                {
                    numberOfCharactersWritten = 0;
                    return false;
                }

                numberOfCharactersWritten = rune.EncodeToUtf16(new Span<char>(buffer, bufferLength));
                return true;
            }

            numberOfCharactersWritten = 0;
            return false;
        }

        public override bool WillEncode(int unicodeScalar)
        {
            return unicodeScalar is >= 0 and <= 0x1F || unicodeScalar == '\"' || unicodeScalar == '\\';
        }
    }
}
