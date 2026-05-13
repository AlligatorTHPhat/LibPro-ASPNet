import axios from 'axios';
const baseURL = import.meta.env.VITE_API_URL

const api = axios.create({
    baseURL: `${baseURL}/api/Categories`,
    headers: { 'Content-Type': 'application/json' }
});

// Thêm interceptor để đính kèm token vào mỗi request
api.interceptors.request.use((config) => {
    const token = localStorage.getItem('token'); // Hoặc lấy từ AuthContext
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

export const categoryApi = {
    getAll: () => api.get('/'),
    create: (data: { name: string; description: string; parentCategoryId?: string | null }) =>
        api.post('/', data),
    update: (id: string, data: { name: string; description?: string; parentCategoryId?: string | null }) =>
        api.put(`/${id}`, data)
};