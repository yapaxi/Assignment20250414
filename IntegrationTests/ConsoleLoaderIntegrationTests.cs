using Assignment.LoaderConsole.Clients;

namespace UnitTests
{
    [TestClass]
    public sealed class ConsoleLoaderIntegrationTests
    {
        [TestMethod]
        public async Task LoadAllCharacters()
        {

            var rm = new RickAndMortyApiClient(
                new HttpClient()
                {
                    BaseAddress = new Uri("https://rickandmortyapi.com/")
                }
            );

            var characters = await rm.GetCharacters(onlyAlive: true, default);

            Assert.IsNotNull(characters);
            Assert.AreEqual(439, characters.Count);
        }
    }
}
