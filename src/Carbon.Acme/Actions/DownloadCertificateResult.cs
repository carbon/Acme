namespace Carbon.Acme;

public class DownloadCertificateResult
{
    public DownloadCertificateResult(string contentType, string body)
    {
        ContentType = contentType;
        Body = body;
    }

    public string ContentType { get; }

    public string Body { get; }

    // public string Link { get; init; }
}
