using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace RaceDay.Web.Services;

public class RaceDayApiClient(HttpClient http, IHttpContextAccessor accessor)
{
    private void Authorise()
    {
        var token = accessor.HttpContext?.Session.GetString("JwtToken");
        http.DefaultRequestHeaders.Authorization = string.IsNullOrWhiteSpace(token)
            ? null
            : new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<JsonElement?> GetAsync(string path)
    {
        Authorise();
        using var response = await http.GetAsync(path);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<JsonElement>();
    }

    public async Task<(bool ok, string message, JsonElement? data)> PostAsync<T>(string path, T body)
    {
        Authorise();
        using var response = await http.PostAsJsonAsync(path, body);
        var raw = await response.Content.ReadAsStringAsync();
        JsonElement? data = null;
        try
        {
            if (!string.IsNullOrWhiteSpace(raw))
                data = JsonDocument.Parse(raw).RootElement.Clone();
        }
        catch (JsonException) { }
        return (response.IsSuccessStatusCode, raw, data);
    }

    public async Task<(bool ok, string message)> SendAsync<T>(HttpMethod method, string path, T body)
    {
        Authorise();
        using var request = new HttpRequestMessage(method, path) { Content = JsonContent.Create(body) };
        using var response = await http.SendAsync(request);
        return (response.IsSuccessStatusCode, await response.Content.ReadAsStringAsync());
    }

    public async Task<(bool ok, string message)> DeleteAsync(string path)
    {
        Authorise();
        using var response = await http.DeleteAsync(path);
        return (response.IsSuccessStatusCode, await response.Content.ReadAsStringAsync());
    }
}
