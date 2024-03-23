using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Card_Reader_Api.Model
{
    /// <summary>
    /// Represents a chat model containing messages.
    /// </summary>
    public class ChatModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChatModel"/> class.
        /// </summary>
        public ChatModel()
        {
            Messages = new List<ChatMessage>();

        }
        public ChatModel(List<ChatMessage> messages, string model, double temperature)
        {
            Messages = messages;
            Model = model;
            Temperature = temperature;
        }
        [JsonProperty("model")]
        public string Model { get; set; }
        [JsonProperty("temperature")]
        public double Temperature { get; set; }


        /// <summary>
        /// Gets or sets the list of chat messages.
        /// </summary>
        [JsonProperty("messages")]
        public List<ChatMessage> Messages { get; set; }

        /// <summary>
        /// Serializes the chat model to JSON.
        /// </summary>
        /// <returns>A JSON string representing the chat model.</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// Adds multiple chat messages to the chat model.
        /// </summary>
        /// <param name="chatMessages">The chat messages to add.</param>
        public void AddMessage(params ChatMessage[] chatMessages)
        {
            Messages.AddRange(chatMessages);
        }

        /// <summary>
        /// Insert a chat message to the start of the list of messages in chat model.
        /// </summary>
        /// <param name="chatMessage">The chat message to add.</param>
        public void AddFirstMessage(ChatMessage chatMessage)
        {
            Messages.Insert(0, chatMessage);
        }



        /// <summary>
        /// Adds a chat message with the specified role and content to the chat model.
        /// </summary>
        /// <param name="role">The role of the message sender.</param>
        /// <param name="content">The content of the message.</param>
        public void AddMessage(string role, string content)
        {
            Messages.Add(new ChatMessage { Role = role, Content = content });
        }
    }

    /// <summary>
    /// Represents a chat message with a role and content.
    /// </summary>
    public class ChatMessage
    {

        /// <summary>
        /// chat message constructor.
        /// </summary>
        public ChatMessage()
        { }
        /// <summary>
        /// chat message constructor with params.
        /// </summary>
        public ChatMessage(string role, string content)
        {
            Role = role;
            Content = content;
        }
        /// <summary>
        /// Gets or sets the role of the message sender.
        /// </summary>
        [JsonProperty("role")]
        public string Role { get; set; }

        /// <summary>
        /// Gets or sets the content of the message.
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; }


        /// <summary>
        /// Serializes the chat model to JSON.
        /// </summary>
        /// <returns>A JSON string representing the chat model.</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }


}
