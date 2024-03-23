using Microsoft.AspNetCore.Mvc;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients.Base;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients.Cards;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients;

namespace Card_Reader_Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CardController : ControllerBase
    {
        [HttpGet(Name = "pokemoncard")]
        public async Task<ApiResourceList<Card>> Get([FromQuery] string name, [FromQuery] string number)
        {
            PokemonApiClient pokeClient = new PokemonApiClient(Env.KEY_POKEMON);

            var filter = new Dictionary<string, string>
            {
                { "name", name },
                { "number", number }
            };
            var cards = await pokeClient.GetApiResourceAsync<Card>(filter);
            return cards;
        }
    }
}