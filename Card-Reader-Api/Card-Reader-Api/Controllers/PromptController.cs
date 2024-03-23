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
        [HttpGet(Name = "customGpt")]
        public static async Task<string> AskGpt(string content, string prompt, string system)
        {
            ChatModel chatModel = new();
            chatModel.AddMessage(
                new("system", system),
                new("user", prompt),
                new("user", content));

            return await CallOpenAI(chatModel);
        }

            public static async Task<string> CallOpenAI(ChatModel messages)
            {
                var jsonPayloadBuilder = new JsonPayloadBuilder("gpt-35-turbo");
                messages.Messages.ForEach(chatModel => { jsonPayloadBuilder.AddMessage(chatModel.Role, chatModel.Content); });
                string jsonPayload = jsonPayloadBuilder.BuildPayload();

                using (HttpClient httpClient = new HttpClient())
                {
                    string url = Env.URL_OPEN_AI;
                    string apiKey = Env.KEY_OPEN_AI;

                    // Adding API-Key header
                    httpClient.DefaultRequestHeaders.Add("api-key", apiKey);

                    // Creating HTTP request
                    HttpRequestMessage request = new(HttpMethod.Post, url)
                    {
                        Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json")
                    };

                    // Sending HTTP request
                    HttpResponseMessage response = await httpClient.SendAsync(request);

                    // Processing the response
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        var responseObject = System.Text.Json.JsonSerializer.Deserialize<Result>(responseBody);
                        var firstChoice = responseObject?.choices.FirstOrDefault();
                        if (firstChoice != null)
                        {
                            return firstChoice.message.content;
                        }
                    }

                    // Handling request errors
                    return response.StatusCode.ToString();
                }
            }
    }
 }

