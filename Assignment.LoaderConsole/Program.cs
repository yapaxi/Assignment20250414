using Assignment.App.Data;
using Assignment.LoaderConsole.Clients;
using Assignment.LoaderConsole.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

var cl = new RickAndMortyApiClient(
    new HttpClient()
    {
        BaseAddress = new Uri("https://rickandmortyapi.com/")
    }
);

var charactersSrc = await cl.GetCharacters(onlyAlive: true, default);
Console.WriteLine($"loaded characters:\t{charactersSrc.Count}");

var episodesSrc = await cl.GetEpisodes(default);
Console.WriteLine($"loaded episodes:\t{episodesSrc.Count}");

var locationsSrc = await cl.GetLocations(default);
Console.WriteLine($"loaded locations:\t{locationsSrc.Count}");

var context = new Context();
await context.Database.EnsureDeletedAsync();
await context.Database.EnsureCreatedAsync();
Console.WriteLine("db ready");

context.Episodes.AddRange(episodesSrc.Select(q => new Assignment.LoaderConsole.Data.Models.Episode()
{
    Name = q.Name,
    AirDate = q.AirDate,
    Created = DateTime.UtcNow,
    SourceId = q.Id,
    Url = q.Url
}));
context.Episodes.Add(new Assignment.LoaderConsole.Data.Models.Episode()
{
    Name = "unknown",
    Created = DateTime.UtcNow
});

context.LocationInfos.AddRange(locationsSrc.Select(q => new Assignment.LoaderConsole.Data.Models.LocationInfo()
{
    Name = q.Name,
    SourceId = q.Id,
    Url = q.Url,
    Created = DateTime.UtcNow,
    Type = q.Type,
    Dimension = q.Dimension
}));
context.LocationInfos.Add(new Assignment.LoaderConsole.Data.Models.LocationInfo()
{
    Name = "unknown",
    Created = DateTime.UtcNow
});

await context.SaveChangesAsync();
Console.WriteLine("episodes and locations inserted");

var locations = await context.LocationInfos.ToArrayAsync();
var episodes = await context.Episodes.ToArrayAsync();

var locationUrlCache = locations.Where(q => q.Url != null).GroupBy(q => FormatUrl(q.Url!)).ToDictionary(q => q.Key, q => q.Single(), StringComparer.OrdinalIgnoreCase);
var locationNameCache = locations.Where(q => q.Url == null).GroupBy(q => FormatName(q.Name)).ToDictionary(q => q.Key, q => q.Single(), StringComparer.OrdinalIgnoreCase);
var episodeUrlCache = episodes.Where(q => q.Url != null).ToDictionary(q => FormatUrl(q.Url!), StringComparer.OrdinalIgnoreCase);
var episodeNameCache = episodes.Where(q => q.Url == null).ToDictionary(q => FormatName(q.Name), StringComparer.OrdinalIgnoreCase);

static string FormatUrl(string s) => s.TrimEnd('/', '?', '&').Trim();

static string FormatName(string s) => s.Trim();


await context.Characters.AddRangeAsync(charactersSrc.Select(q =>
{
    var character = new Assignment.LoaderConsole.Data.Models.Character()
    {
        SourceId = q.Id,
        Name = q.Name,
        Status = q.Status,
        Created = q.Created,
        Gender = q.Gender,
        Image = q.Image,
        Species = q.Species,
        Type = q.Type,
        Url = q.Url
    };

    if (q.Location is { } l)
    {
        if (!string.IsNullOrWhiteSpace(l.Url))
        {
            character.Origin = locationUrlCache[FormatUrl(l.Url)];
        }
        else
        {
            character.Origin = locationNameCache[FormatName(l.Name)];
        }
    }

    if (q.Origin is { } o)
    {
        if (!string.IsNullOrWhiteSpace(o.Url))
        {
            character.Origin = locationUrlCache[FormatUrl(o.Url)];
        }
        else
        {
            character.Origin = locationNameCache[FormatName(o.Name)];
        }
    }

    if (q.Episode is { } el && el.Any())
    {
        character.CharacterEpisodes = el.Select(FormatUrl).Distinct(StringComparer.OrdinalIgnoreCase).Select(q => new Assignment.LoaderConsole.Data.Models.CharacterEpisode()
        {
            Character = character,
            EpisodeId = string.IsNullOrWhiteSpace(q) ? episodeNameCache["unknown"].Id : episodeUrlCache[q].Id,
        }).ToArray();
    }


    return character;
}));

await context.SaveChangesAsync();

class NullLogger : ILogger<RickAndMortyApiClient>
{
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {

    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }
}

class FixedOptions<T>: IOptions<T> where T: class
{
    public required T Value { get; init; }
}