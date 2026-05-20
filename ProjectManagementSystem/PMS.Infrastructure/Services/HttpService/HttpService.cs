using PMS.Application.Interfaces.HttpService;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace PMS.Infrastructure.Services.HttpService;

public class HttpService : IHttpService
{
    //private readonly IHttpClientFactory _httpClientFactory;

    //public HttpService(
    //    IHttpClientFactory httpClientFactory)
    //{
    //    _httpClientFactory = httpClientFactory;
    //}
    //public async Task<TResponse?> PostAsync<TRequest, TResponse>(
    //        string clientName,
    //        string endpoint,
    //        TRequest payload,
    //        string? bearerToken = null,
    //        CancellationToken cancellationToken = default)
    //{
    //    var client = _httpClientFactory.CreateClient(clientName);

    //    var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
    //    {
    //        Content = JsonContent.Create(payload)
    //    };

    //    if (!string.IsNullOrWhiteSpace(bearerToken))
    //    {
    //        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
    //    }

    //    var response = await client.SendAsync(request, cancellationToken);

    //    response.EnsureSuccessStatusCode();

    //    return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken);
    //}
}
