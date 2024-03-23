using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Azure;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Card_Reader_Api.Model;

namespace Card_Reader_Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentIntelligenceController : ControllerBase
    {
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

        [HttpPost("GetCardDetailsLocal")]
        public async Task<DtoAnalysePokemon> GetCardDetailsLocal(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            var credential = new AzureKeyCredential(Env.KEY_DOC_INTEL);
            var client = new DocumentAnalysisClient(new Uri(Env.URL_DOC_INTEL), credential);

            try
            {
                string PokemonName = "";
                string CardCode = "";
                string CardNumber = "";
                string FormattedCardNumber = "";

                using (var stream = file.OpenReadStream())
                {
                    string modelId = "card-reader-model";

                    AnalyzeDocumentOperation operation = await client.AnalyzeDocumentAsync(WaitUntil.Completed, modelId, stream);
                    
                    await operation.WaitForCompletionAsync();

                    AnalyzeResult result = operation.Value;

                    foreach (var document in result.Documents)
                    {
                        foreach (var fieldKvp in document.Fields)
                        {
                            var fieldName = fieldKvp.Key;
                            var fieldValue = fieldKvp.Value;

                            if (fieldName == "Pokemon name" || fieldName == "Card code" || fieldName == "Card Number")
                            {
                                Console.WriteLine($"{fieldName}: {fieldValue.Content}");
                            }
                            if (fieldName == "Pokemon name")
                            {
                                PokemonName = fieldValue.Content;
                            }
                            if (fieldName == "Card code")
                            {
                                CardCode = fieldValue.Content;
                            }
                            if (fieldName == "Card Number")
                            {
                                CardNumber = fieldValue.Content;
                            }
                        }
                    }
                }

                FormattedCardNumber = CardNumber.TrimStart('0').Split('/')[0];

                return new DtoAnalysePokemon
                {
                    PokemonName = PokemonName,
                    CardNumber = FormattedCardNumber
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }

        public class DtoAnalysePokemon
        {
            public string PokemonName { get; set; }
            public string CardNumber { get; set; }
        }
    }
}
