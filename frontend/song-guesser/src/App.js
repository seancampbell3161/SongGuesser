// import React from 'react';
// import './App.css';
// import FileUpload from './FileUpload';
// import YouTubeConverter from './YouTubeConverter';
// import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
// import HomePage from './HomePage';
// import ConvertPage from './ConvertPage';

// function App() {
//   // return (
//   //   <div className="App">
//   //     <header className="App-header">
//   //       <h1>Enter YouTube URL</h1>
//   //       <YouTubeConverter />
//   //       <br />
//   //       {/* <h1>Upload</h1> */}
//   //       {/* <FileUpload /> */}
//   //     </header>
//   //   </div>
//   // );
//   return (
//     <Router>
//         <Routes>
//             <Route path="/" element={<HomePage />} />
//             <Route path="/convert" element={<ConvertPage />} />
//         </Routes>
//     </Router>
// );
// }

// export default App;


import React, { useContext, useEffect } from 'react';
import { BrowserRouter as Router, Route, Routes, Navigate } from 'react-router-dom';
import HomePage from './HomePage';
import ConvertPage from './ConvertPage';
import { AuthContext, AuthProvider } from './AuthContext';
import LoginPage from './LoginPage';

const App = () => {
    return (
        <AuthProvider>
            <Router>
                <Routes>
                    <Route path="/" element={<ProtectedRoute><HomePage /></ProtectedRoute>} />
                    <Route path="/convert" element={<ProtectedRoute><ConvertPage /></ProtectedRoute>} />
                    <Route path="/login" element={<LoginPage />} />
                </Routes>
            </Router>
        </AuthProvider>
    );
};

const ProtectedRoute = ({ children }) => {
    const { authToken } = useContext(AuthContext);
    return authToken ? children : <Navigate to="/login" />;
};

export default App;