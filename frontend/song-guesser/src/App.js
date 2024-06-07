import React from 'react';
import './App.css';
import FileUpload from './FileUpload';
import YouTubeConverter from './YouTubeConverter';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import HomePage from './HomePage';
import ConvertPage from './ConvertPage';

function App() {
  // return (
  //   <div className="App">
  //     <header className="App-header">
  //       <h1>Enter YouTube URL</h1>
  //       <YouTubeConverter />
  //       <br />
  //       {/* <h1>Upload</h1> */}
  //       {/* <FileUpload /> */}
  //     </header>
  //   </div>
  // );
  return (
    <Router>
        <Routes>
            <Route path="/" element={<HomePage />} />
            <Route path="/convert" element={<ConvertPage />} />
        </Routes>
    </Router>
);
}

export default App;