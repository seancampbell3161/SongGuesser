import React from 'react';
import './App.css';
import FileUpload from './FileUpload';
import YouTubeConverter from './YouTubeConverter';

function App() {
  return (
    <div className="App">
      <header className="App-header">
        <h1>YouTube Converter</h1>
        <YouTubeConverter />
        <br />
        <h1>Song Guesser</h1>
        <FileUpload />
      </header>
    </div>
  );
}

export default App;