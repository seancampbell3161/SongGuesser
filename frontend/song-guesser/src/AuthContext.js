import React, { createContext, useState } from 'react';

export const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
    const [authToken, setAuthToken] = useState(null);

    const login = (token) => {
        setAuthToken(token);
        localStorage.setItem('authToken', token);
    };

    const logout = () => {
        setAuthToken(null);
        localStorage.removeItem('authToken');
    };

    return (
        <AuthContext.Provider value={{ authToken, login, logout }}>
            {children}
        </AuthContext.Provider>
    );
};