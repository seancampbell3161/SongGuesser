import React, { useContext, useEffect } from 'react';
import { BrowserRouter as Router, Route, Routes, Navigate } from 'react-router-dom';
import HomePage from './HomePage';
import ConvertPage from './ConvertPage';
import { AuthContext, AuthProvider } from './AuthContext';
import LoginPage from './LoginPage';
import api from './api';

const App = () => {
  useEffect(() => {
    const token = localStorage.getItem('token');
    if (token) {
        api.defaults.headers.common['Authorization'] = `Bearer ${token}`;
    }
  }, []);

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