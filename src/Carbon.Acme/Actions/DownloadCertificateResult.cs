namespace Carbon.Acme;

public sealed class DownloadCertificateResult(string contentType, string body)
{
    public string ContentType { get; } = contentType;

    public string Body { get; } = body;

    // public string Link { get; init; }
}
