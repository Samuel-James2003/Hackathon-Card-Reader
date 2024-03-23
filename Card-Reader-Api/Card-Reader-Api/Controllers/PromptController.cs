using Azure.AI.OpenAI;
using Card_Reader_Api.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;


namespace Card_Reader_Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PromptController : ControllerBase
    {
  
        [HttpGet(Name = "chatbot")]
        public async Task<string> ConversationAnalysis(string prompt, string system)
        {
            try
            {
                var openai = new OpenAIClient(new Uri(Env.URL_OPEN_AI), new Azure.AzureKeyCredential(Env.KEY_OPEN_AI));

                string conversationHistory = ""; // Initialize a new string to hold the conversation history

                // Append the conversation history to the prompt
                prompt = $"{prompt}\n {conversationHistory}";
                ChatRequestSystemMessage systemMessage = new(system);
                ChatRequestUserMessage message = new ChatRequestUserMessage(prompt);

                // Paramètres de la requête
                var requestOptions = new ChatCompletionsOptions
                {
                    DeploymentName = "gpt4-003",
                    Messages = { message, systemMessage },
                    MaxTokens = 2000
                };

                // Envoyer la requête à Azure OpenAI et attendre la réponse
                var response = await openai.GetChatCompletionsAsync(requestOptions);

                // Extraire la réponse si elle est disponible
                if (response?.Value?.Choices != null && response.Value.Choices.Any())
                {
                    var analysisResult = response.Value.Choices.FirstOrDefault()?.Message?.Content;
                    return analysisResult ?? "Aucune réponse n'a été reçue.";
                }
                else
                {
                    return "Aucune réponse n'a été reçue.";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}

