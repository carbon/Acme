namespace Carbon.Acme
{
    public enum AcmeErrorType
    {
        /// <summary>
        /// The request specified an account that does not exist
        /// </summary>
        AccountDoesNotExist,

        /// <summary>
        /// The CSR is unacceptable(e.g., due to a short key)
        /// </summary>
        BadCSR,

        /// <summary>
        /// The client sent an unacceptable anti-replay nonce
        /// </summary>
        BadNonce,

        /// <summary>
        /// The revocation reason provided is not allowed by the server
        /// </summary>
        BadRevocationReason,

        /// <summary>
        /// The JWS was signed with an algorithm the server does not support
        /// </summary>
        BadSignatureAlgorithm,

        /// <summary>
        /// Certification Authority Authorization(CAA) records forbid the CA from issuing
        /// </summary>
        Caa,

        /// <summary>
        /// A contact URL for an account was invalid
        /// </summary>
        InvalidContact,

        /// <summary>
        /// A contact URL for an account used an unsupported protocol scheme
        /// </summary>
        UnsupportedContact,

        /// <summary>
        /// The request must include a value for the "externalAccountBinding" field
        /// </summary>
        ExternalAccountRequired,

        /// <summary>
        /// The request message was malformed
        /// </summary>
        Malformed,

        /// <summary>
        /// The request exceeds a rate limit
        /// </summary>
        RateLimited,

        /// <summary>
        /// The server will not issue for the identifier
        /// </summary>
        RejectedIdentifier,

        /// <summary>
        /// The server experienced an internal error
        /// </summary>
        ServerInternal,

        /// <summary>
        /// The client lacks sufficient authorization
        /// </summary>
        Unauthorized,

        /// <summary>
        /// Identifier is not supported, but may be in future
        /// </summary>
        UnsupportedIdentifier,

        /// <summary>
        /// Visit the "instance" URL and take actions specified there
        /// </summary>
        UserActionRequired,

        /// <summary>
        /// There was a problem with a DNS query
        /// </summary>
        Dns,

        /// <summary>
        /// The server could not connect to validation target
        /// </summary>
        Connection,

        /// <summary>
        /// The server received a TLS error during validation
        /// </summary>
        Tls,

        /// <summary>
        /// sponse received didn't match the challenge's requirements
        /// </summary>
        IncorrectResponse
    }
}

/*
{
  "type": "urn:ietf:params:acme:error:userActionRequired",
  "detail": "Terms of service have changed",
  "instance": "https://example.com/acme/agreement/?token=W8Ih3PswD-8"
}
*/