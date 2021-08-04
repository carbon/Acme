#nullable enable

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Carbon.Acme
{
    public sealed class CertificateChainReader : IDisposable
    {
        private readonly StringReader reader;

        private bool isEof;

        public CertificateChainReader(string text)
        {
            reader = new StringReader(text);
            isEof = false;
        }

        public bool TryReadNext(out byte[]? der)
        {
            if (isEof)
            {
                der = null;

                return false;
            }

            var sb = new StringBuilder();

            string? line;

            while ((line = reader.ReadLine()) is not null)
            {
                if (line.Length == 0)
                {
                    der = DecodePemData(sb.ToString());

                    return true;
                }
                else
                {
                    sb.Append(line);
                    sb.Append('\n');
                }
            }

            isEof = true;

            if (sb.Length == 0)
            {
                der = null;
                return false;
            }

            der = DecodePemData(sb.ToString());

            return true;
        }

        public void Dispose()
        {
            reader.Dispose();
        }

        private static byte[] DecodePemData(ReadOnlySpan<char> pemData)
        {
            PemEncoding.TryFind(pemData, out PemFields fields);

            byte[] decodedData = new byte[fields.DecodedDataLength];

            if (!Convert.TryFromBase64Chars(pemData[fields.Base64Data], decodedData, out _))
            {
                throw new Exception("Error decoding pem data");
            }

            return decodedData;
        }
    }
}