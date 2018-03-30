using System;
using System.Runtime.Serialization;
using Carbon.Extensions;

namespace Carbon.Acme
{
    public class FinalizeOrderRequest
    {
        public FinalizeOrderRequest(string url, byte[] csr)
        {
            Url = url ?? throw new ArgumentNullException(nameof(url));
            Csr = Base64Url.Encode(csr);
        }

        public FinalizeOrderRequest(string url, string csr)
        {
            Url = url ?? throw new ArgumentNullException(nameof(url));
            Csr = csr ?? throw new ArgumentNullException(nameof(csr));
        }

        [IgnoreDataMember]
        public string Url { get; }

        /// <summary>
        /// A CSR encoding the parameters for the certificate being requested {{!RFC2986}}.
        /// The CSR is sent in the base64url-encoded version of the DER format. 
        /// Note: Because this field uses base64url, and does not include headers, it is different from PEM.).
        /// </summary>
        [DataMember(Name = "csr", IsRequired = true)]
        public string Csr { get; }
    }
}
