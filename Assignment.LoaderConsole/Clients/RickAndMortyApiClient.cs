using Assignment.LoaderConsole.Clients.Models;
using System.Net.Http.Json;

namespace Assignment.LoaderConsole.Clients;

public class RickAndMortyApiClient(
    HttpClient httpClient
)
{
    public static readonly string HTTP_CLIENT_NAME = Guid.NewGuid().ToString();

    public Task<IReadOnlyList<CharacterView>> GetCharacters(bool onlyAlive, CancellationToken cancellationToken)
    {
        return Get<CharacterView>($"/api/character/", query: onlyAlive ? "&status=alive" : "", cancellationToken);
    }

    public Task<IReadOnlyList<EpisodeView>> GetEpisodes(CancellationToken cancellationToken)
    {
        return Get<EpisodeView>($"/api/episode/", query: null, cancellationToken);
    }

    public Task<IReadOnlyList<LocationView>> GetLocations(CancellationToken cancellationToken)
    {
        return Get<LocationView>($"/api/location/", query: null, cancellationToken);
    }

    private async Task<IReadOnlyList<T>> Get<T>(string path, string? query, CancellationToken cancellationToken)
    {
        try
        {
            var page = 1;

            var lst = new List<T>(capacity: 439);

            while (true)
            {
                var httpResponse = await httpClient.GetAsync($"{path}?page={page}{query}", cancellationToken);

                httpResponse.EnsureSuccessStatusCode();

                var obj = await httpResponse.Content.ReadFromJsonAsync<PagedResponse<T>>(cancellationToken);

                if (obj?.Results is not { } r || !r.Any())
                {
                    return lst;
                }

                lst.AddRange(r);

                if (page++ >= obj.Info.Pages)
                {
                    return lst;
                }
            }
        }
        catch (HttpRequestException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return [];
        }
    }
}
