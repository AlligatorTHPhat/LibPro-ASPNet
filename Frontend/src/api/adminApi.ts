import axios from 'axios';

const api = axios.create({
    baseURL: import.meta.env.VITE_API_URL + '/api',
    headers: { 'Content-Type': 'application/json' }
});

api.interceptors.request.use(config => {
    const token = localStorage.getItem('token');
    if (token) config.headers.Authorization = `Bearer ${token}`;
    return config;
});

export const adminApi = {
    getAllStaff: () => api.get('/Staffs'), 
    registerLibrarian: (data: any) => api.post('Auth/register-librarian', data),
    updateStaff: (id: string, data: any) => api.put(`/Staffs/${id}`, data),
    deleteStaff: (id: string) => api.delete(`/Staffs/${id}`),
    restoreStaff: (id: string, currentData: any) => api.put(`/Staffs/${id}`, { ...currentData, isDeleted: false }),

    getAllLogs: () => api.get('/Audit'),
    getHistory: (entity: string, id: string) => api.get(`/Audit/history/${entity}/${id}`)
};