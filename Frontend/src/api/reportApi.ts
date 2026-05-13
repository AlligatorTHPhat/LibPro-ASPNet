import axios from 'axios';
import { DashboardResponse } from '../types/report';

const baseURL = import.meta.env.VITE_API_URL;

const api = axios.create({
    baseURL: `${baseURL}/api/Reports`
});

export const reportApi = {
    getDashboardSummary: async (): Promise<DashboardResponse> => {
        const response = await api.get('/dashboard', {
            headers: {
                Authorization: `Bearer ${localStorage.getItem('token')}`
            }
        });
        return response.data;
    },

    getTopBooks: async (top: number = 5): Promise<any[]> => {
        const response = await api.get(`/book-ranking?top=${top}`, {
            headers: { Authorization: `Bearer ${localStorage.getItem('token')}` }
        });
        return response.data;
    },
    getHighRiskReaders: async (top: number = 5): Promise<any[]> => {
        const response = await api.get(`/high-risk-readers?top=${top}`, {
            headers: { Authorization: `Bearer ${localStorage.getItem('token')}` }
        });
        return response.data;
    },
    getCirculationRate: async (): Promise<number> => {
        const response = await api.get('/circulation-rate', {
            headers: { Authorization: `Bearer ${localStorage.getItem('token')}` }
        });
        return response.data.circulationRatePercentage;
    }
};