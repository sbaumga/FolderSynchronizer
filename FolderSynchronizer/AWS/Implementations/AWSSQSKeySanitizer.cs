using FolderSynchronizer.AWS.Abstractions;
using System.Text.RegularExpressions;

namespace FolderSynchronizer.AWS.Implementations
{
    public class AWSSQSKeySanitizerImp : IAWSSQSKeySanitizer
    {
        public string SanitizeKeyFromSQS(string key)
        {
            var regex = new Regex("((?:%[A-F0-9]{2})+)");

            var matches = regex.Matches(key);
            foreach (var match in matches.ToArray())
            {
                var captures = match.Captures.ToArray();
                foreach (var capture in captures)
                {
                    var hex = string.Join(string.Empty, capture.Value.Split("%"));
                    var character = ConvertHexToUnicode(hex);
                    key = key.Replace(capture.Value, character);
                }
            }

            key = key.Replace("+", " ");

            return key;
        }

        private static string ConvertHexToAscii(string hex)
        {
            var ascii = string.Empty;

            for (var i = 0; i < hex.Length; i += 2)
            {
                var hs = string.Empty;

                hs = hex.Substring(i, 2);
                var decVal = System.Convert.ToUInt32(hs, 16);
                var character = System.Convert.ToChar(decVal);
                ascii += character;
            }

            return ascii;
        }

        private static string ConvertHexToUnicode(string hex)
        {
            var dBytes = StringToByteArray(hex);
            var utf8result = System.Text.Encoding.UTF8.GetString(dBytes);
            return utf8result;
        }

        private static byte[] StringToByteArray(string hex)
        {
            var numberChars = hex.Length / 2;
            var bytes = new byte[numberChars];
            using var sr = new StringReader(hex);
            for (var i = 0; i < numberChars; i++)
            {
                bytes[i] = Convert.ToByte(new string(new char[2] { (char)sr.Read(), (char)sr.Read() }), 16);
            }
            return bytes;
        }
    }
}