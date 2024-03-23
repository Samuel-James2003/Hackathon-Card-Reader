using Microsoft.AspNetCore.Mvc;
using PokemonTcgSdk.Standard.Features.FilterBuilder.Pokemon;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients.Base;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients.Cards;

namespace Card_Reader_Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        [HttpGet(Name = "pokemoncard")]
        public async Task<ApiResourceList<Card>> Get()
        {
            PokemonApiClient pokeClient = new PokemonApiClient("1c32b871-0b3c-4c43-97c3-fcf646079c38");
            
            var filter =new Dictionary<string, string>
            {
                {"name","Venusaur"},
                {"number","1"}
            };
            var cards = await pokeClient.GetApiResourceAsync<Card>(filter);
            return cards;
        }
    }
}