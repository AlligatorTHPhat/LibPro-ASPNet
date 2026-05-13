import axios from 'axios';
import { ReserveBookRequest, ReservationResponse } from '../types/reservation';

const baseURL = import.meta.env.VITE_API_URL;

const api = axios.create({
    baseURL: `${baseURL}/api/Reservation`,
    headers: { 'Content-Type': 'application/json' }
});

api.interceptors.request.use((config) => {
    const token = localStorage.getItem('token');
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

export const reservationApi = {
    reserveBook: (data: ReserveBookRequest) => api.post('/reserve', data),

    getMyActive: () =>
        api.get<ReservationResponse[]>('/my-active'),

    cancel: (id: string) =>
        api.post(`/${id}/cancel`),

    getPending: () =>
        api.get<ReservationResponse[]>('/pending'),

    confirmToLoan: (id: string) =>
        api.post(`/${id}/confirm`),

    getReadyForLibrarian: () =>
        api.get<ReservationResponse[]>('/ready-list'),
};