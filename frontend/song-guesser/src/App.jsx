import React, { useEffect, useState } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import axios from 'axios';
import HomePage from './HomePage';
import LoginPage from './LoginPage';

const App = () => {
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        const token = localStorage.getItem('token');
        if (token) {
            axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
            setIsAuthenticated(true);
        }
        setIsLoading(false); // Mark loading as complete
    }, []);

    if (isLoading) {
        return <div>Loading...</div>; // Show a loading message or spinner
    }

    return (
        <Router>
            <Routes>
                <Route path="/login" element={<LoginPage />} />
                <Route path="/" element={isAuthenticated ? <HomePage /> : <Navigate to="/login" />} />
            </Routes>
        </Router>
    );
};

export default App;