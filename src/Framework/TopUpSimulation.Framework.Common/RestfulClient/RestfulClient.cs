using Newtonsoft.Json;
using SIMA.Framework.Infrastructure.RestfulClient;
using System.Text;

namespace TopUpSimulation.Framework.Common.RestfulClient;

public class RestfulClient : IRestfulClient
{
    private readonly HttpClient _httpClient;

    public RestfulClient()
    {
        _httpClient = new HttpClient();
    }

    public async Task<TResult> PostAsync<TResult, TRequest>(string actionUrl, TRequest request, Dictionary<string, string>? headers = null) where TResult : class
    {
        if (headers != null)
            foreach (var item in headers)
                _httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);

        var response = await _httpClient.PostAsync(actionUrl, CreateHttpContent(request));
        response.EnsureSuccessStatusCode();
        var data = await response.Content.ReadAsStringAsync();

        if (typeof(TResult) == typeof(string))
        {
            return data as TResult;
        }
        var model = JsonConvert.DeserializeObject<TResult>(data);

        return model;
    }
    #region private methods


    private static JsonSerializerSettings MicrosoftDateFormatSettings
    {
        get
        {
            return new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
            };
        }
    }

    private HttpContent CreateHttpContent<T>(T content)
    {
        var json = string.Empty;
        if (typeof(T) == typeof(string))
        {
            json = content.ToString();
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
        else
        {
            json = JsonConvert.SerializeObject(content, MicrosoftDateFormatSettings);
            return new StringContent(json, Encoding.UTF8, "application/json");


        }
        #endregion
    }
}
