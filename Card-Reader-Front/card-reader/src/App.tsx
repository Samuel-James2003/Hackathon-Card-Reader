import React from 'react';
import ImageUploader from './component/ImportImage';
import Chatbot from './component/Chatbot'
import './App.css';

function App() {
  return (
    <div className="App">
      <ImageUploader></ImageUploader>
      <Chatbot/>
    </div>
  );
}

export default App;
