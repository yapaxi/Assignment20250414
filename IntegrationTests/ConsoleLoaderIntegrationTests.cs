using Assignment.LoaderConsole.Clients;
using Assignment.LoaderConsole.Clients.Models;
using System.Text.Json;
using System.Web;

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
