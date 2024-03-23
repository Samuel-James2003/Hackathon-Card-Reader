import React, { useState } from 'react';
import './index.css';

const OpenAIRequest = () => {
  const [messageHistory, setMessageHistory] = useState<string[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);
  const [inputMessage, setInputMessage] = useState<string>('');

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

    try {
      const response = await fetch('/prompt', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ message: inputMessage }),
      });

      if (!response.ok) {
        throw new Error('Failed to fetch response from the server.');
      }

      const responseData = await response.json();
      setMessageHistory([...messageHistory, responseData.message]);
      setInputMessage('');
    } catch (err : any) {
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
            <p key={index}>{message}</p>
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
