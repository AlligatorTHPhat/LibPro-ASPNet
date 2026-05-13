import axios from 'axios';
import { ReaderResponse, CreateReaderRequest, UpdateReaderRequest } from '../types/reader';

const baseURL = import.meta.env.VITE_API_URL;

const api = axios.create({
    baseURL: `${baseURL}/api/Readers`,
    headers: { 'Content-Type': 'application/json' }
});

api.interceptors.request.use((config) => {
    const token = localStorage.getItem('token');
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

export const readerApi = {
    getAll: () => api.get<ReaderResponse[]>(''),
    update: (id: string, data: UpdateReaderRequest) => api.put(`/${id}`, data),
    delete: (id: string) => api.delete(`/${id}`),
    restore: (id: string) => api.put(`/${id}/restore`),
};