using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Azure;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Card_Reader_Api.Model;
using Newtonsoft.Json;

namespace Card_Reader_Api.Controllers
{
    /// <summary></summary>
    [ApiController]
    [Route("[controller]")]
    public class DocumentIntelligenceController : ControllerBase
    {
        /// <summary>
        /// Get card details based on the url of a card 
        /// </summary>
        /// <param name="imageUrl">The url of the card</param>
        /// <returns>The pokemon Identifiaction number and pokemon name</returns>
        [HttpGet(Name = "GetCardDetailsPost")]
        public async Task<IActionResult> GetCardDetailsPost([FromQuery] string imageUrl)
        {
            try
            {
                Console.WriteLine(imageUrl);
                if (string.IsNullOrEmpty(imageUrl))
                {
                    return BadRequest("Image URL cannot be null or empty.");
                }

                var credential = new AzureKeyCredential(Env.KEY_DOC_INTEL);
                var client = new DocumentAnalysisClient(new Uri(Env.URL_DOC_INTEL), credential);

                using (HttpClient httpClient = new HttpClient())
                {
                    using (HttpResponseMessage response = await httpClient.GetAsync(imageUrl))
                    {
                        using (Stream stream = await response.Content.ReadAsStreamAsync())
                        {
                            Console.Write(imageUrl);
                            string modelId = "card-reader-model";
                            AnalyzeDocumentOperation operation = await client.AnalyzeDocumentAsync(WaitUntil.Completed, modelId, stream);
                            await operation.WaitForCompletionAsync();
                            AnalyzeResult result = operation.Value;

                            string PokemonName = "";
                            string CardCode = "";
                            string CardNumber = "";
                            string FormattedCardNumber = "";

                            foreach (var document in result.Documents)
                            {
                                foreach (var fieldKvp in document.Fields)
                                {
                                    var fieldName = fieldKvp.Key;
                                    var fieldValue = fieldKvp.Value;

                                    if (fieldName == "Pokemon name")
                                    {
                                        PokemonName = fieldValue.Content;
                                    }
                                    else if (fieldName == "Card code")
                                    {
                                        CardCode = fieldValue.Content;
                                    }
                                    else if (fieldName == "Card Number")
                                    {
                                        CardNumber = fieldValue.Content;
                                    }
                                }
                            }

                            FormattedCardNumber = CardNumber.TrimStart('0').Split('/')[0];
                            string pokemonApiUrl = $"https://api.pokemontcg.io/v2/cards?q=name:{PokemonName}+number:{FormattedCardNumber}";

                            using (HttpResponseMessage apiResponse = await httpClient.GetAsync(pokemonApiUrl))
                            {
                                if (apiResponse.IsSuccessStatusCode)
                                {
                                    string resultContent = await apiResponse.Content.ReadAsStringAsync();
                                    return Ok(resultContent);
                                }
                                else
                                {
                                    return StatusCode((int)apiResponse.StatusCode);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        /// <summary>
        /// Get card details based on the url of a card
        /// </summary>
        /// <param name="file">Image file </param>
        /// <returns>he pokemon Identifiaction number and pokemon name</returns>
        [HttpPost("GetCardDetailsLocal")]
        public async Task<DtoAnalysePokemon> GetCardDetailsLocal(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return new DtoAnalysePokemon();
            }
            try
            {
                return await GetCardDetails(file, "card-reader-model");
            }
            catch (Exception)
            {
                try
                {
                    return await GetCardDetails(file, "card-reader-model-right");
                }
                catch (Exception)
                {
                    return new DtoAnalysePokemon();
                }
            }
        }

        private static async Task<DtoAnalysePokemon> GetCardDetails(IFormFile file, string model)
        {
            var credential = new AzureKeyCredential(Env.KEY_DOC_INTEL);
            var client = new DocumentAnalysisClient(new Uri(Env.URL_DOC_INTEL), credential);

            using (HttpClient httpClient = new HttpClient())
            {
                using (Stream stream = file.OpenReadStream())
                {
                    string modelId = model;
                    AnalyzeDocumentOperation operation = await client.AnalyzeDocumentAsync(WaitUntil.Completed, modelId, stream);
                    await operation.WaitForCompletionAsync();
                    AnalyzeResult result = operation.Value;

                    string PokemonName = "";
                    string CardCode = "";
                    string CardNumber = "";
                    string FormattedCardNumber = "";

                    foreach (var document in result.Documents)
                    {
                        foreach (var fieldKvp in document.Fields)
                        {
                            var fieldName = fieldKvp.Key;
                            var fieldValue = fieldKvp.Value;

                            if (fieldName == "Pokemon name")
                            {
                                PokemonName = fieldValue.Content;
                            }
                            else if (fieldName == "Card code")
                            {
                                CardCode = fieldValue.Content;
                            }
                            else if (fieldName == "Card Number")
                            {
                                CardNumber = fieldValue.Content;
                            }
                        }
                    }

                    string jsonName = $"https://tyradex.tech/api/v1/pokemon/{PokemonName}";
                    using (HttpResponseMessage responseTranslated = await httpClient.GetAsync(jsonName))
                    {
                        if (responseTranslated.IsSuccessStatusCode)
                        {
                            string resultContent = await responseTranslated.Content.ReadAsStringAsync();
                            dynamic pokemonNameTranslated = JsonConvert.DeserializeObject(resultContent);
                            string nameTranslated = pokemonNameTranslated.name.en.ToString();
                            FormattedCardNumber = CardNumber.TrimStart('0').Split('/')[0];

                            return new DtoAnalysePokemon
                            {
                                PokemonName = nameTranslated,
                                CardNumber = CardNumber,
                                FormatNumber = FormattedCardNumber
                            };
                        }
                        else
                        {
                            return new DtoAnalysePokemon();
                        }
                    }
                }
            }
        }
    }
    /// <summary/>
    public class DtoAnalysePokemon
    {
        public string PokemonName { get; set; }
        public string CardNumber { get; set; }
        public string FormatNumber { get; set; }
    }
}
