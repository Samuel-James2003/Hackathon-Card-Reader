import React, { useState } from 'react';
import './index.css';
import OpenAI, { Completion } from 'openai';

interface Message {
  role: string;
  content: string;
}

const Chatbot = () => {
  const [messageHistory, setMessageHistory] = useState<Message[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  const [selectedFile, setSelectedFile] = useState<File | null>(null);

  const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    if (event.target.files && event.target.files.length > 0) {
      setSelectedFile(event.target.files[0]);
    }
  };

  const openaiClient = new OpenAI({
    apiKey: process.env.REACT_APP_OPENAI_API_KEY || '',
    dangerouslyAllowBrowser: true
  });

  const generateResponse = async () => {
    if (!selectedFile) {
      alert('Please select a file.');
      return;
    }

    setLoading(true);
    setError(null);
    try {
      const reader = new FileReader();
      reader.readAsText(selectedFile);
      reader.onload = async () => {
        if (reader.result) {
          let userInputFromFile: string = reader.result.toString();
          const question = " donne moi les noms de toute les personnes que tu vois ";
          userInputFromFile += question;

          const newMessageHistory: Message[] = [...messageHistory, { role: 'user', content: userInputFromFile }];

          const response: Completion | null = await openaiClient.chat.completions.create({
            model: 'gpt-3.5-turbo',
            messages: newMessageHistory,
            max_tokens:  500,
          });

          if (response && response.choices && response.choices.length > 0) {
            setMessageHistory([...newMessageHistory, response.choices[0].message]);
          } else {
            throw new Error('Unexpected response structure');
          }
        }
      };
    } catch (err) {
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
            <p key={index} className={message.role === 'user' ? 'user_msg' : ''}>
              <span>{message.content}</span>
            </p>
          ))}
        </div>
        <input
          type="file"
          onChange={handleFileChange}
        />
        <button
          type="submit"
          aria-label="Send Message"
          onClick={generateResponse}
          disabled={loading || !selectedFile}
        >
          {loading ? 'Loading...' : 'Send'}
        </button>
        {error && <p className="error-message">{error}</p>}
      </div>
    </div>
  );
};

export default Chatbot;