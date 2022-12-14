using System.Net;
using System.Net.Http.Json;

namespace UnitTestTraining;

public class Node
{
    public int Id { get; set; }
}

public interface ILoginProvider
{
    void Login();
}

public class HttpTraining
{
    private readonly HttpClient _httpClient;
    private readonly ILoginProvider _loginProvider;

    public HttpTraining(HttpClient httpClient, ILoginProvider loginProvider)
    {
        _httpClient = httpClient;
        _loginProvider = loginProvider;
    }

    public async Task<List<Node>> GetNodesAsync()
    {
        var response = await _httpClient.GetAsync("https://localhost/nodes");

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            _loginProvider.Login();
            return new List<Node>();
        }

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<List<Node>>();

        return result;
    }
}