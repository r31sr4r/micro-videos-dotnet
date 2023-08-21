using FC.Codeflix.Catalog.EndToEndTests.Extensions.String;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Json;

namespace FC.Codeflix.Catalog.EndToEndTests.Base;

class SnakeCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
        => name.ToSnakeCase();
}

public  class ApiClient
{
    private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _defaultSerializerOptions;

    public ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _defaultSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = new SnakeCaseNamingPolicy(),
            PropertyNameCaseInsensitive = true
        };
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
                    JsonSerializer.Serialize(
                        payload, 
                        _defaultSerializerOptions
                    ),
                    Encoding.UTF8,
                    "application/json"
                )
            );

        var output = await GetOutput<TOutupt>(response);
        return (response, output);
    }

    public async Task<(HttpResponseMessage?, TOutupt?)> Get<TOutupt>(
        string route,
        object? queryStringParametersObject = null
    )
    where TOutupt : class
    {
        var url = PrepareGetRoute(route, queryStringParametersObject);
        var response = await _httpClient.GetAsync(
                url 
            );

        var output = await GetOutput<TOutupt>(response);

        return (response, output);
    }

    private string PrepareGetRoute(
        string route, object? 
        queryStringParametersObject
    )
    {
        if ( queryStringParametersObject is null )
            return route;        
        var parametersJson = JsonSerializer.Serialize(
            queryStringParametersObject,
            _defaultSerializerOptions
        );
        var parametersDictionary = Newtonsoft.Json.JsonConvert
            .DeserializeObject<Dictionary<string, string>>(parametersJson);

        return QueryHelpers.AddQueryString(route, parametersDictionary!);
    }

    public async Task<(HttpResponseMessage?, TOutupt?)> Delete<TOutupt>(
        string route
    )
    where TOutupt : class
    {
        var response = await _httpClient.DeleteAsync(route);

        var output = await GetOutput<TOutupt>(response);

        return (response, output);
    }

    public async Task<(HttpResponseMessage?, TOutupt?)> Put<TOutupt>(
        string route,
        object payload
    )
        where TOutupt : class
    {
        var response = await _httpClient.PutAsync(
                route,
                new StringContent(
                    JsonSerializer.Serialize(
                        payload,
                        _defaultSerializerOptions
                    ),
                    Encoding.UTF8,
                    "application/json"
                )
        );

        var output = await GetOutput<TOutupt>(response);

        return (response, output);
    }

    private async Task<TOutupt?> GetOutput<TOutupt>(HttpResponseMessage response) 
        where TOutupt : class
    {
        var outputString = await response.Content.ReadAsStringAsync();
        TOutupt? output = null;

        if (!string.IsNullOrWhiteSpace(outputString))
        {
            output = JsonSerializer.Deserialize<TOutupt>(
                outputString,
                _defaultSerializerOptions
            );
        }

        return output;
    }
}
