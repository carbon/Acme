using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;

using Carbon.Json;

namespace Carbon.Acme
{
    public class Directory
    {
        [DataMember(Name = "newNonce")]
        public string NewNonceUrl { get; set; }

        [DataMember(Name = "newAccount")]
        public string NewAccountUrl { get; set; }

        [DataMember(Name = "newOrder")]
        public string NewOrderUrl { get; set; }

        [DataMember(Name = "newAuthz")]
        public string NewAuthorizationUrl { get; set; }

        [DataMember(Name = "revokeCert")]
        public string RevokeCertificateUrl { get; set; }

        [DataMember(Name = "keyChange")]
        public string KeyChangeUrl { get; set; }

        [DataMember(Name = "meta")]
        public Meta Meta { get; set; }
        
        public static async Task<Directory> GetAsync(string url = "https://acme-v02.api.letsencrypt.org/directory")
        {
            using (var http = new HttpClient())
            {
                var responseText = await http.GetStringAsync(url);

                return JsonObject.Parse(responseText).As<Directory>();
            }
        }
    }
}