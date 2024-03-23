using Microsoft.AspNetCore.Mvc;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients.Base;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients.Cards;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients;

namespace Card_Reader_Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class CardController : ControllerBase
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Pokemon name</param>
        /// <param name="number"> Pokemon Identification Number</param>
        /// <returns> <see cref="Card"/> that matches the Name and ID  </returns>
        [HttpGet(Name = "GetPokemonCard")]
        public async Task<Card> GetPokemonCard([FromQuery] string name, [FromQuery] string number)
        {
            PokemonApiClient pokeClient = new PokemonApiClient(Env.KEY_POKEMON);

            var filter = new Dictionary<string, string>
            {
                { "name", name },
                { "number", number }
            };
            var cards = await pokeClient.GetApiResourceAsync<Card>(filter);

            return cards.Results.First();
        }
    }
}

