using System;
using System.IO;
using System.Text;

using Carbon.Pkcs;

namespace Carbon.Acme
{
    public class CertificateChainReader : IDisposable
    {
        private readonly StringReader reader;
        private bool isEof = false;

        public CertificateChainReader(string text)
        {
            reader = new StringReader(text);
        }

        public bool TryReadNext(out byte[] der)
        {
            if (isEof)
            {
                der = null;

                return false;
            }

            var sb = new StringBuilder();

            string line;

            while ((line = reader.ReadLine()) != null)
            {
                if (line == string.Empty)
                {
                    der = Pem.Decode(sb.ToString());

                    return true;
                }
                else
                {
                    sb.AppendLine(line);
                }
            }

            isEof = true;

            der = Pem.Decode(sb.ToString());

            return true;
        }

        public void Dispose()
        {
            reader.Dispose();
        }
    }
}