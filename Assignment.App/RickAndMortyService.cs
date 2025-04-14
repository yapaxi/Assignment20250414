using Assignment.App.Data;
using Assignment.Contracts.Input;
using Microsoft.EntityFrameworkCore;

namespace Assignment.LoaderConsole
{
    public class RickAndMortyService(IRepository repository, LazyCache.IAppCache lazyCache, StatisticsContext statisticsContext) : IRickAndMortyService
    {
        private class Cache(Dictionary<int, Contracts.Output.Character> characters)
        {
            public const string PlanetType = "Planet";

            private readonly object rwLock = new();
            private readonly List<Contracts.Output.Character> charactersPlanetOriginSubset = characters.Values.Where(IsFromPlanet).ToList();

            public void Update(Contracts.Output.Character character)
            {
                lock (rwLock)
                {
                    characters[character.Id] = character;

                    UpdateIndexes();

                    void UpdateIndexes()
                    {
                        if (IsFromPlanet(character))
                        {
                            charactersPlanetOriginSubset.Add(character);
                        }
                    }
                }
            }

            private static bool IsFromPlanet(Contracts.Output.Character c) => c.Origin?.Type?.Equals(PlanetType, StringComparison.OrdinalIgnoreCase) == true;

            public Contracts.Output.Character? GetCharacter(int id)
            {
                lock (rwLock)
                {
                    return characters.TryGetValue(id, out var c) ? c : null;
                }
            }

            public IReadOnlyList<Contracts.Output.Character> FindCharacters(string? originPlanet, int skip, int take)
            {
                lock (rwLock)
                {
                    if (!string.IsNullOrWhiteSpace(originPlanet))
                    {
                        return charactersPlanetOriginSubset.Where(q => q.Origin?.Name.Contains(originPlanet) == true).Skip(skip).Take(take).ToArray();
                    }
                    else
                    {
                        return characters.Values.Skip(skip).Take(take).ToArray();
                    }
                }
            }
        }

        private async Task<T> WithCache<T>(Func<Cache, T> func)
        {
            var cache = await lazyCache.GetOrAdd(
                "all-characters",
                async q =>
                {
                    statisticsContext.RegisterDbCall();
                    q.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                    return new Cache(await repository.CharactersQuery.ToDictionaryAsync(q => q.Id, q => Map(q)));
                }
            );

            return func(cache);
        }

        public Task<Contracts.Output.Character?> GetCharacter(int id)
        {
            return WithCache((q) => q.GetCharacter(id));
        }

        public Task<IReadOnlyList<Contracts.Output.Character>> FindCharacters(string? originPlanet, int skip, int take)
        {
            return WithCache((q) => q.FindCharacters(originPlanet, skip, take));
        }

        public async Task<Contracts.Output.Character> AddCharacter(Character character)
        {
            var c = new Data.Models.Character()
            {
                Name = character.Name,
                Status = character.Status,
                Gender = character.Gender,
                Species = character.Species,
                Type = character.Type,

                Created = DateTime.UtcNow
            };

            if (character.KnownLocationId is { } lid && lid > 0)
            {
                c.LocationId = lid;
            }
            else if (character.NewLocation is { } nl)
            {
                c.Location = new Data.Models.LocationInfo()
                {
                    Name = nl.Name,
                    Type = nl.Type,
                    Dimension = nl.Dimension,
                    Created = DateTime.UtcNow
                };
            }

            if (character.KnownOriginId is { } oid && oid > 0)
            {
                c.OriginId = oid;
            }
            else if (character.NewOrigin is { } no)
            {
                c.Origin = new Data.Models.LocationInfo()
                {
                    Name = no.Name,
                    Type = no.Type,
                    Dimension = no.Dimension,
                    Created = DateTime.UtcNow
                };
            }

            if (character.KnownEpisodes is { Count: > 0 } ke)
            {
                c.CharacterEpisodes ??= [];
                foreach (var ep in ke)
                {
                    if (ep is 0)
                    {
                        continue;
                    }

                    c.CharacterEpisodes.Add(new Data.Models.CharacterEpisode() { Character = c, EpisodeId = ep });
                }
            }

            if (character.NewEpisodes is { Count: > 0 } ne)
            {
                c.CharacterEpisodes ??= [];
                foreach (var ep in ne)
                {
                    c.CharacterEpisodes.Add(new Data.Models.CharacterEpisode()
                    {
                        Character = c,
                        Episode = new Data.Models.Episode()
                        {
                            AirDate = ep.AirDate,
                            Name = ep.Name,
                            Created = DateTime.UtcNow,
                        }
                    });
                }
            }

            c = await repository.Add(c);

            await repository.SaveChanges();

            var r = Map(c);

            await WithCache((q) =>
            {
                q.Update(r);
                return r;
            });

            return r;
        }

        private static Contracts.Output.Character Map(Data.Models.Character c)
        {
            return new Contracts.Output.Character()
            {
                Id = c.Id,
                Name = c.Name,
                Status = c.Status,
                Gender = c.Gender,
                Species = c.Species,
                Type = c.Type,
                Origin = c.Origin is { } o
                          ? new Contracts.Output.Location() { Id = o.Id, Name = o.Name, Dimension = o.Dimension, Type = o.Type }
                          : null,
                Location = c.Location is { } l
                            ? new Contracts.Output.Location() { Id = l.Id, Name = l.Name, Dimension = l.Dimension, Type = l.Type }
                            : null,
                Episodes = c.CharacterEpisodes is { Count: > 0 } ce
                            ? ce.Select(q => new Contracts.Output.Episode() { Id = q.EpisodeId, Name = q.Episode.Name, AirDate = q.Episode.AirDate }).ToArray()
                            : []
            };
        }
    }
}
