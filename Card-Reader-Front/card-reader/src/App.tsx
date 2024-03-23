import React from 'react';
import ImageUploader from './component/ImportImage';
import Chatbot from './component/Chatbot'
import { ImageResponseProvider } from './context/ImageResponseContext';
import DisplayResponse from './component/DisplayResult';
import './App.css';

function App() {
  return (
    <div className="App">
      <ImageResponseProvider>
        <ImageUploader />
        <DisplayResponse/>
        <Chatbot/>
      </ImageResponseProvider>
    </div>
  );
}

export default App;
