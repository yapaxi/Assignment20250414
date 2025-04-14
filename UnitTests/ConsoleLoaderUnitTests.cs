using Assignment.LoaderConsole.Clients;
using Assignment.LoaderConsole.Clients.Models;
using System.Text.Json;
using System.Web;

namespace UnitTests
{
    [TestClass]
    public sealed class ConsoleLoaderUnitTests
    {
        [TestMethod]
        public async Task PagingTest()
        {
            var httpClient = new HttpClient(new FuncHttpMessageHandler(req => new HttpResponseMessage()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(new PagedResponse<CharacterView>()
                {
                    Info = new Info() { Pages = 3 },
                    Results = HttpUtility.ParseQueryString(req.RequestUri!.Query)["page"] switch
                    {
                        "1" => [new CharacterView() { Name = "1", Status = "Alive" }],
                        "2" => [new CharacterView() { Name = "2", Status = "Alive" }],
                        "3" => [new CharacterView() { Name = "3", Status = "Alive" }],
                    }
                }))
            }))
            {
                BaseAddress = new Uri("https://xxx.yyy")
            };

            var rm = new RickAndMortyApiClient(httpClient);

            var characters = await rm.GetCharacters(default, default);

            Assert.IsNotNull(characters);
            Assert.AreEqual(3, characters.Count);
            Assert.AreEqual(3, characters.DistinctBy(q => q.Name).Count());
        }
    }

    public class FuncHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> func) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(func(request));
        }
    }
}
