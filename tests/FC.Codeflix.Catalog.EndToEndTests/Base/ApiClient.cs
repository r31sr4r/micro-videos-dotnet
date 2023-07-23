using Microsoft.VisualStudio.TestPlatform.Utilities;
using System.Text;
using System.Text.Json;

namespace FC.Codeflix.Catalog.EndToEndTests.Base;
public  class ApiClient
{
    private readonly HttpClient _httpClient;

    public ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<(HttpResponseMessage?, TOutupt?)> Post<TOutupt>(
        string route,
        object payload
        )
        where TOutupt : class
    {
        var response = await _httpClient.PostAsync(
                route,
                new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json"
                )
            );

        var outputString = await response.Content.ReadAsStringAsync();
        TOutupt? output = null;
        if (!String.IsNullOrWhiteSpace(outputString))
        {
            output = JsonSerializer.Deserialize<TOutupt>(outputString,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );
        }
        return (response, output);
    }

    public async Task<(HttpResponseMessage?, TOutupt?)> Get<TOutupt>(
        string route
    )
    where TOutupt : class
    {
        var response = await _httpClient.GetAsync(
                route
            );

        var outputString = await response.Content.ReadAsStringAsync();
        TOutupt? output = null;
        if (!String.IsNullOrWhiteSpace(outputString))
        {
            output = JsonSerializer.Deserialize<TOutupt>(outputString,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );
        }
        return (response, output);
    }

    public async Task<(HttpResponseMessage?, TOutupt?)> Delete<TOutupt>(
        string route
    )
    where TOutupt : class
    {
        var response = await _httpClient.DeleteAsync(route);

        var outputString = await response.Content.ReadAsStringAsync();
        TOutupt? output = null;
        if (!String.IsNullOrWhiteSpace(outputString))
        {
            output = JsonSerializer.Deserialize<TOutupt>(outputString,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );
        }
        return (response, output);
    }
}
