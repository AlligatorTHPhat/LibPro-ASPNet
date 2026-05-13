import axios from 'axios';
const baseURL = import.meta.env.VITE_API_URL

const api = axios.create({
    baseURL: `${baseURL}/api/Publishers`,
    headers: { 'Content-Type': 'application/json' }
});

api.interceptors.request.use((config) => {
    const token = localStorage.getItem('token');
    if (token) config.headers.Authorization = `Bearer ${token}`;
    return config;
});

export const publisherApi = {
    getAll: () => api.get('/'),
    getById: (id: string) => api.get(`/${id}`),
    create: (data: any) => api.post('/', data),
    update: (id: string, data: any) => api.put(`/${id}`, data),
    delete: (id: string) => api.delete(`/${id}`)
};