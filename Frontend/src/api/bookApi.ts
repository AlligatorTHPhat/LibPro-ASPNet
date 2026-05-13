import axios from 'axios';
const baseURL = import.meta.env.VITE_API_URL


const api = axios.create({
    baseURL: `${baseURL}/api/Books`,
    headers: { 'Content-Type': 'application/json' }
});

api.interceptors.request.use((config) => {
    const token = localStorage.getItem('token');
    if (token) config.headers.Authorization = `Bearer ${token}`;
    return config;
});

export const bookApi = {
    getAll: () => api.get('/'),
    getById: (id: string) => api.get(`/${id}`),
    create: (formData: FormData) => api.post('', formData, {
        headers: { 'Content-Type': 'multipart/form-data' }
    }),
    update: (id: string, formData: FormData) => api.put(`/${id}`, formData, {
        headers: { 'Content-Type': 'multipart/form-data' }
    }),
    getAdminDetails: (id: string) => api.get(`/${id}/details-librarian`),

    addBookItem: (data: { bookId: string; shelfLocation: string; quantity: number }) => api.post('/items', data),

    updateItemStatus: (data: { bookItemId: string; status: number }) => api.put(`/items/${data.bookItemId}/status`, { status: data.status }),

    deleteBookItem: (itemId: string) => api.delete(`/${itemId}`),

    restoreBookItem: (itemId: string) => api.put(`/restore/${itemId}`),

    searchByTitle: (title: string) => api.get(`/title/${title}`),

    searchByAuthor: (author: string) => api.get(`/author/${author}`)
};