using Microsoft.AspNetCore.Mvc;

using Azure.AI.DocumentIntelligence;
using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Card_Reader_Api.Model;
using AnalyzeResult = Azure.AI.FormRecognizer.DocumentAnalysis.AnalyzeResult;

namespace Card_Reader_Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentIntelligenceController : ControllerBase
    {
        /// <summary>
        /// Gets the details from the card sent
        /// </summary>
        /// <param name="file">Image of the card</param>
        /// <param name="url">Url to the image to be treated</param>
        /// <returns>A <see cref="string"/> containing the pokemon name and a <seealso cref="string"/> containg the Pokemon ID </returns>
        /// <exception cref="ArgumentNullException">both file and url are null</exception>
        /// <exception cref="NotImplementedException">Not ready for HEIF</exception
        [HttpGet(Name = "GetCardDetailsPost")]
        public async Task<IActionResult> GetCardDetailsPost([FromQuery] string imageUrl)
        {
            var credential = new AzureKeyCredential(Env.KEY_DOC_INTEL);
            var client = new DocumentAnalysisClient(new Uri(Env.URL_DOC_INTEL), credential);

            if (string.IsNullOrEmpty(imageUrl))
            {
                return BadRequest("Image URL cannot be null or empty.");
            }

            try
            {
                string PokemonName = "";
                string CardCode = "";
                string CardNumber = "";
                string FormattedCardNumber = "";

                using (HttpClient httpClient = new HttpClient())
                {
                    using (HttpResponseMessage response = await httpClient.GetAsync(imageUrl))
                    {
                        using (Stream stream = await response.Content.ReadAsStreamAsync())
                        {
                            string modelId = "card-reader-model";

                            // Analyse du document
                            Console.WriteLine("Analyzing document...");
                            AnalyzeDocumentOperation operation = await client.AnalyzeDocumentAsync(WaitUntil.Completed, modelId, stream);
                            // Attente de la compl�tion de l'op�ration
                            await operation.WaitForCompletionAsync();

                            // R�cup�ration des r�sultats
                            AnalyzeResult result = operation.Value;

                            // Afficher les donn�es extraites
                            Console.WriteLine("Document analysis completed.");

                            // Parcourir les documents analys�s
                            foreach (var document in result.Documents)
                            {
                                // Parcourir les champs du document
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
                    }
                }

                FormattedCardNumber = CardNumber.TrimStart('0').Split('/')[0];

                string pokemonApiUrl = $"https://api.pokemontcg.io/v2/cards?q=name:{PokemonName}+number:{FormattedCardNumber}";
                using (HttpClient httpClient = new HttpClient())
                {
                    using (HttpResponseMessage response = await httpClient.GetAsync(pokemonApiUrl))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            string result = await response.Content.ReadAsStringAsync();
                            Console.WriteLine($"API Response: {result}");
                            return Ok(result);
                        }
                        else
                        {
                            Console.WriteLine($"Error: {response.StatusCode}");
                            return StatusCode((int)response.StatusCode);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
