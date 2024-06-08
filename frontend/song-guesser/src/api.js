import axios from 'axios';

const api = axios.create({
    baseURL: 'http://localhost:5244/api',
    headers: {
        'Content-Type': 'application/json',
    }
});

// Add a request interceptor to include the JWT token
api.interceptors.request.use(
    config => {
        const token = localStorage.getItem('authToken');
        if (token) {
            config.headers['Authorization'] = `Bearer ${token}`;
        }
        return config;
    },
    error => {
        return Promise.reject(error);
    }
);

export default api;