using System.IO.Compression;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Traceability;

public class HttpClientDecompressionHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

        if (response.Content.Headers.ContentEncoding.Any(x => x == "gzip"))
        {
            await using (var compressedStream = await response.Content.ReadAsStreamAsync())
            {
                using (var decompressedStream = new GZipStream(compressedStream, CompressionMode.Decompress))
                {
                    var memoryStream = new MemoryStream();
                    await decompressedStream.CopyToAsync(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    response.Content = new StreamContent(memoryStream);
                }
            }
        }

        return response;
    }
}