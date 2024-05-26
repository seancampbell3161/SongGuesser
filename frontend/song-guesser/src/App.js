import React from 'react';
import './App.css';
import FileUpload from './FileUpload';
import YouTubeConverter from './YouTubeConverter';

function App() {
  return (
    <div className="App">
      <header className="App-header">
        <h1>Enter YouTube URL</h1>
        <YouTubeConverter />
        <br />
        <h1>Upload</h1>
        <FileUpload />
      </header>
    </div>
  );
}

export default App;