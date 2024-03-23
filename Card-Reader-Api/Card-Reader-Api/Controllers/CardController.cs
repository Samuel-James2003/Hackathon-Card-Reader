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
            public async Task<ApiResourceList<Card>> Get(string name,string number)
            {
                PokemonApiClient pokeClient = new PokemonApiClient("1c32b871-0b3c-4c43-97c3-fcf646079c38");

                var filter = new Dictionary<string, string>
            {
                {"name",name},
                {"number",number}
            };
                var cards = await pokeClient.GetApiResourceAsync<Card>(filter);
                return cards;
            }
        }
}
