import React, { useState, useContext } from "react";
import "../index.css";
import "../styles/chat.css";
import axios from "axios";
import { ImageResponseContext } from "../context/ImageResponseContext";

interface chatMessage {
  role: string;
  content: string;
}

const OpenAIRequest = () => {
  const [messageHistory, setMessageHistory] = useState<chatMessage[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);
  const [inputMessage, setInputMessage] = useState<string>("");
  const [messageContent, setMessageContent] = useState<string>("");
  const { responseData } = useContext(ImageResponseContext);

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setInputMessage(event.target.value);
  };

  const handleMessageContext = async () => {
    try {
      const params = {
        name: responseData?.pokemonName,
        number: responseData?.formatNumber,
      };
      const response = await axios.get("http://localhost:5185/Card", {
        params,
      });
      console.log(response.data.messageContent);
      return JSON.stringify(response.data);
    } catch (error) {
      console.error(error);
    }
  };

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    if (!inputMessage) {
      alert("Please enter a message.");
      return;
    }

    setLoading(true);
    setError(null);
    let Messages: chatMessage[] = messageHistory;

    try {
      let url = "http://localhost:5185/Prompt/ConversationAnalysis";

      const message = await handleMessageContext();
      messageHistory.push({
        role: "system",
        content:
          "En te basant sur les données d'une carte pokemon tu es un professionel de carte pokemon et tu dois repondre a des question concernant les informations",
      });

      const formData = new FormData();
      let str: string = "";
      messageHistory.forEach((chatMessage: any) => {
        str += chatMessage.role + ": " + chatMessage.content + ";\n";
        console.log("str :", str);
      });
      console.log("inputMessage :", inputMessage);
      messageHistory.push({
        role: "user",
        content: inputMessage,
      });
      formData.append("prompt", JSON.stringify(message) + " : " + inputMessage);

      formData.append("conversationHistory", str);
      console.log("formData: ", formData.entries());
      // Using Axios for the POST request
      const response = await axios.post(url, formData, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      });
      console.log(response);

      if (response.status !== 200) {
        throw new Error("Failed to fetch response from the server.");
      }

      const responseData = response.data;
      console.log(responseData);

      setMessageHistory([
        ...Messages,
        {
          role: "assistant",
          content: responseData,
        },
      ]);
      setInputMessage("");
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const showChat = (message: chatMessage) => {
    if (message.role === "assistant") {
      return (
        <>
          <div className="chatBubble assistant">{message.content}</div>
        </>
      );
    }
    if (message.role === "user") {
      return (
        <>
          <div className="chatBubble user">{message.content}</div>
        </>
      );
    }
  };

  const askDeck = () => {
    let History: chatMessage[];
    const handleFileRead = (event: ProgressEvent<FileReader>) => {
      const content = event.target?.result as string;
      console.log(content);
      History = [
        {
          content:
            "Recherche dans les decks que je vais te donner, redonne moi le deck en entier qui comprends ce pokémon",
          role: "system",
        },
        { content: content, role: "user" },
        {
          content: "Voici mon pokemon : " + responseData?.pokemonName,
          role: "user",
        },
      ];
      try {
        let url = "http://localhost:5185/Prompt/ConversationAnalysis";
        const formData = new FormData();
      let str: string = "";
      History.forEach((chatMessage: any) => {
        str += chatMessage.role + ": " + chatMessage.content + ";\n";
        console.log("str :", str);
      });
      formData.append("conversationHistory",str);
    } catch (error) {
      console.error(error);
    }
  };
};

  return (
    <div className="openai-container">
      <div className="centered-content">
        <div className="message-history">
          {messageHistory.map((message: chatMessage, index: any) => (
            <p key={index}>{showChat(message)}</p>
          ))}
        </div>
        <form onSubmit={handleSubmit}>
          <input
            type="text"
            value={inputMessage}
            onChange={handleChange}
            placeholder="Type your message..."
          />
          <button
            type="submit"
            aria-label="Send Message"
            disabled={loading || !inputMessage}
          >
            {loading ? "Loading..." : "Send"}
          </button>
        </form>
        <form onSubmit={handleSubmit}>
        <input
            type="text"
            value={inputMessage}
            onChange={handleChange}
            placeholder="Type your message..."
          />
          <button
            type="submit"
            aria-label="Create Deck"
            disabled={loading || !inputMessage}
          >
            {loading ? 'Loading...' : 'Send'}
            </button>
        </form>
        {error && <p className="error-message">{error}</p>}
      </div>
    </div>
  );
};

export default OpenAIRequest;
