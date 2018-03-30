using System;
using System.Runtime.Serialization;
using Carbon.Extensions;

namespace Carbon.Acme
{
    public class RevokeCertificateRequest
    {
        public RevokeCertificateRequest(byte[] certificate, int? reason = null)
        {
            Certificate = Base64Url.Encode(certificate);
            Reason = reason;
        }

        public RevokeCertificateRequest(string certificate, int? reason = null)
        {
            Certificate = certificate ?? throw new ArgumentNullException(nameof(certificate));
            Reason      = reason;
        }

        /// <summary>
        /// The certificate to be revoked, in the base64url-encoded version of the DER format. 
        /// Note: Because this field uses base64url, and does not include headers, it is different from PEM.
        /// </summary>
        [DataMember(Name = "certificate", IsRequired = true)]
        public string Certificate { get; }

        [DataMember(Name = "reason", EmitDefaultValue = false)]
        public int? Reason { get; }
    }
}

// https://tools.ietf.org/html/draft-ietf-acme-acme-10#section-7.6