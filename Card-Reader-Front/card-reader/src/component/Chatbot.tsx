import React, { useState } from 'react';
import '../index.css';
import axios from 'axios';

interface chatMessage{
  role:string;
  content:string;
}

const OpenAIRequest = () => {
  const [messageHistory, setMessageHistory] = useState<chatMessage[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);
  const [inputMessage, setInputMessage] = useState<string>('');
  const [contextObject, setContextObject] = useState<string>('');

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setInputMessage(event.target.value);
  };

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    if (!inputMessage) {
      alert('Please enter a message.');
      return;
    }

    setLoading(true);
    setError(null);
    let Messages: chatMessage[] = messageHistory;

    try {
      let url = 'http://localhost:5185/Prompt/ConversationAnalysis';
      
        messageHistory.push({
          role:"system",
          content:"hello you are an AI assistant",
        });
     
      
      const formData = new FormData();
      let str:string ='';
      messageHistory.forEach((chatMessage: any)=>{

        str+=chatMessage.role+": "+chatMessage.content+";\n";
        console.log("str :",str);
      })
      console.log("inputMessage :",inputMessage);
      formData.append('prompt',inputMessage)
      formData.append('conversationHistory',str);
      console.log("formData: ",formData.entries());
      // Using Axios for the POST request
      const response = await axios.post(url, formData ,{
        headers: {
          'Content-Type': 'multipart/form-data'
        }
      });
      console.log(response)

      messageHistory.push({
        role:"user",
        
        content:inputMessage,
      });

      if (response.status !== 200) {
        throw new Error('Failed to fetch response from the server.');
      }

      const responseData = response.data;
      console.log(responseData)

      setMessageHistory([...Messages, {
        role:"assistant",
        content:responseData}]);
      setInputMessage('');
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="openai-container">
      <div className="centered-content">
        <div className="message-history">
          {messageHistory.map((message, index) => (
            <p key={index}>{message.content}</p>
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
            {loading ? 'Loading...' : 'Send'}
          </button>
        </form>
        {error && <p className="error-message">{error}</p>}
      </div>
    </div>
  );
};

export default OpenAIRequest;
