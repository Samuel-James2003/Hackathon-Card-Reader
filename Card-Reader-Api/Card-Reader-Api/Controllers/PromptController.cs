using Azure.AI.OpenAI;
using Card_Reader_Api.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;


namespace Card_Reader_Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class PromptController : ControllerBase
    {

        /// <summary>
        /// Sends a message to chatgpt
        /// </summary>
        /// <remarks>If the conversationHistory is null a system prompt is required </remarks>
        /// <param name="conversationHistory"> Assuming the format of each message is "sender: content" </param>
        /// <param name="prompt"> The user's question </param>
        /// <param name="system"> The system prompt </param>
        /// <returns> Returns chatgpt's response  </returns>
        [HttpPost("ConversationAnalysis")]
        public async Task<string> ConversationAnalysis([FromForm(Name ="conversationHistory")]string? conversationHistory,[FromForm(Name = "prompt")] string prompt, [FromForm(Name ="system")] string? system)
        {
            try
            {
                var openai = new OpenAIClient(new Uri(Env.URL_OPEN_AI), new Azure.AzureKeyCredential(Env.KEY_OPEN_AI));

                IList<ChatRequestMessage> Messages = [];
                ChatCompletionsOptions requestOptions = new();
                if (string.IsNullOrEmpty(conversationHistory))
                {
                    ChatRequestSystemMessage systemMessage = new(system);
                    ChatRequestUserMessage promptMessage = new(prompt);
                    requestOptions = new ChatCompletionsOptions
                    {
                        DeploymentName = Env.MODEL_OPEN_AI,
                        Messages = { systemMessage, promptMessage }
                    };
                }
                else if (string.IsNullOrEmpty(system))
                {
                   
                    if (!string.IsNullOrEmpty(conversationHistory))
                    {
                        string[] messages = conversationHistory.Split(';');
                        foreach (string message in messages)
                        {
                           
                            string[] parts = message.Split(':');
                            if (parts.Length == 2)
                            {
                                string sender = parts[0].Trim();
                                string content = parts[1].Trim();
                                switch (sender.ToLower())
                                {
                                    case "system":
                                        Messages.Add(new ChatRequestSystemMessage(content));
                                        break;
                                    case "user":
                                        Messages.Add(new ChatRequestUserMessage(content));
                                        break;
                                    case "assistant":
                                        Messages.Add(new ChatRequestAssistantMessage(content));
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        Messages.Add(new ChatRequestUserMessage(prompt));
                        // Construct requestOptions with the parsed messages
                        requestOptions = new ChatCompletionsOptions(Env.MODEL_OPEN_AI, Messages);
                    }
                }
                requestOptions.MaxTokens = 2000;
                var response = await openai.GetChatCompletionsAsync(requestOptions);
                // Extraire la réponse
                var analysisResult = response.Value.Choices.FirstOrDefault()?.Message?.Content;
                return analysisResult ?? "Aucune réponse n'a été reçue.";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}

