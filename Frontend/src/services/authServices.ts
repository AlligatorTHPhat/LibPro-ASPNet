import axios from 'axios';

const API_URL = 'http://localhost:5000/api/account'; // Thay bằng URL Backend của bạn

export const registerReader = async (data: any) => {
    // Khớp với RegisterReaderRequest ở Backend[cite: 21]
    return await axios.post(`${API_URL}/register-reader`, data);
};

export const login = async (credentials: any) => {
    // Khớp với LoginRequest ở Backend
    const response = await axios.post(`${API_URL}/login`, credentials);
    if (response.data.token) {
        localStorage.setItem('token', response.data.token); // Lưu JWT Token[cite: 23]
    }
    return response.data;
};