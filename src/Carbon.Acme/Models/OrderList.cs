#nullable disable

using System.Runtime.Serialization;

namespace Carbon.Acme
{
    public class OrderList
    {
        [DataMember(Name = "orders")]
        public string[] Urls { get; set; }

        // TODO Next
        // Link: <https://example.com/acme/acct/1/orders?cursor=2>, rel="next"
    }
}