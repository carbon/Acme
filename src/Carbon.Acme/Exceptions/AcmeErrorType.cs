namespace Carbon.Acme.Exceptions
{
    public enum AcmeErrorType : byte
    {
        Unknown = 0,

        /// <summary>
        /// The request specified an account that does not exist
        /// </summary>
        AccountDoesNotExist = 1,

        /// <summary>
        /// The request specified a certificate to be revoked that has already been revoked.
        /// </summary>
        AlreadyRevoked = 2,

        /// <summary>
        /// The CSR is unacceptable(e.g., due to a short key)
        /// </summary>
        BadCSR = 3,

        /// <summary>
        /// The client sent an unacceptable anti-replay nonce
        /// </summary>
        BadNonce = 4,

        /// <summary>
        /// The JWS was signed by a public key the server does not support
        /// </summary>
        BadPublicKey = 5,

        /// <summary>
        /// The revocation reason provided is not allowed by the server
        /// </summary>
        BadRevocationReason = 6,

        /// <summary>
        /// The JWS was signed with an algorithm the server does not support
        /// </summary>
        BadSignatureAlgorithm = 7,

        /// <summary>
        /// Certification Authority Authorization(CAA) records forbid the CA from issuing a certificate
        /// </summary>
        Caa = 8,

        /// <summary>
        /// Specific error conditions are indicated in the "subproblems" array
        /// </summary>
        Compound = 9,

        /// <summary>
        /// The server could not connect to validation target.
        /// </summary>
        Connection = 10,

        /// <summary>
        /// There was a problem with a DNS query during identifier validation
        /// </summary>
        Dns = 11,

        /// <summary>
        /// The request must include a value for the "externalAccountBinding" field
        /// </summary>
        ExternalAccountRequired = 12,

        /// <summary>
        /// Response received didn't match the challenge's requirements
        /// </summary>
        IncorrectResponse = 13,

        /// <summary>
        /// A contact URL for an account was invalid
        /// </summary>
        InvalidContact = 14,

        /// <summary>
        /// The request message was malformed
        /// </summary>
        Malformed = 15,

        /// <summary>
        /// The request attempted to finalize an order that is not ready to be finalized
        /// </summary>
        OrderNotReady = 16,

        /// <summary>
        /// The request exceeds a rate limit
        /// </summary>
        RateLimited = 17,

        /// <summary>
        /// The server will not issue for the identifier
        /// </summary>
        RejectedIdentifier = 18,

        /// <summary>
        /// The server experienced an internal error
        /// </summary>
        ServerInternal = 19,

        /// <summary>
        /// The server received a TLS error during validation
        /// </summary>
        Tls = 20,

        /// <summary>
        /// The client lacks sufficient authorization
        /// </summary>
        Unauthorized = 21,

        /// <summary>
        /// A contact URL for an account used an unsupported protocol scheme
        /// </summary>
        UnsupportedContact = 22,

        /// <summary>
        /// An identifier is of an unsupported type
        /// </summary>
        UnsupportedIdentifier = 24,

        /// <summary>
        /// Visit the "instance" URL and take actions specified there
        /// </summary>
        UserActionRequired = 25
    }
}

/*
{
  "type": "urn:ietf:params:acme:error:userActionRequired",
  "detail": "Terms of service have changed",
  "instance": "https://example.com/acme/agreement/?token=W8Ih3PswD-8"
}
*/
