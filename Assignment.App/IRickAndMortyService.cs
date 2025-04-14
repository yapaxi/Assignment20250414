using Assignment.Contracts.Input;

namespace Assignment.LoaderConsole
{
    public interface IRickAndMortyService
    {
        Task<Contracts.Output.Character> AddCharacter(Character character);

        Task<Contracts.Output.Character?> GetCharacter(int id);

        Task<IReadOnlyList<Contracts.Output.Character>> FindCharacters(string? originPlanet, int skip, int take);
    }
}