using Microsoft.AspNetCore.Http;

namespace Kentico.Xperience.AiraUnified;

internal static class HttpRequestExtensions
{
    public static string GetBaseUrl(this HttpRequest request)
    {
        var pathBase = request.PathBase.ToString();
        var baseUrl = $"{request.Scheme}://{request.Host}";

        return !string.IsNullOrWhiteSpace(pathBase) ? $"{baseUrl}{pathBase}" : baseUrl;
    }

    public static string GetUrlWithSchema(this HttpRequest request, string? url) => !string.IsNullOrEmpty(url) ? $"{request.Scheme}://{url}" : request.GetBaseUrl();
}
