using System.Net.Http.Json;

namespace UserService.Client.Http.Abstractions;

public abstract class HttpClientBase
{
  protected readonly HttpClient Http;

  protected HttpClientBase(HttpClient http)
  {
    Http = http;
  }

  protected async Task<T> GetAsync<T>(string url, CancellationToken ct)
  {
    var result = await Http.GetFromJsonAsync<T>(url, ct);
    return result!;
  }

  protected async Task<TResponse> PostAsync<TRequest, TResponse>(
      string url,
      TRequest request,
      CancellationToken ct)
  {
    var response = await Http.PostAsJsonAsync(url, request, ct);
    response.EnsureSuccessStatusCode();

    return (await response.Content.ReadFromJsonAsync<TResponse>(ct))!;
  }

  protected async Task PostAsync<TRequest>(
      string url,
      TRequest request,
      CancellationToken ct)
  {
    var response = await Http.PostAsJsonAsync(url, request, ct);
    response.EnsureSuccessStatusCode();
  }
}