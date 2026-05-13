import axios from 'axios';

const baseURL = import.meta.env.VITE_API_URL;

const api = axios.create({
    baseURL: `${baseURL}/api/loans`,
    headers: { 'Content-Type': 'application/json' }
});

api.interceptors.request.use((config) => {
    const token = localStorage.getItem('token');
    if (token) config.headers.Authorization = `Bearer ${token}`;
    return config;
});

export const loanApi = {
    createOnlineLoan: (data: { bookItemsId: any[], loanDays: number }) => {
        return api.post('/online', data); 
    },
    getMyHistory: () => {
        return api.get('/my-history'); 
    },
    getAllActiveLoanDetails: () =>
        api.get('/active-details'),
    returnPhysicalBook: (loanDetailId: string) =>
        api.post(`/return/physical/${loanDetailId}`),
    extendLoan: (loanDetailId: string, extraDays: number = 7) =>
        api.post(`/extend/${loanDetailId}?extraDays=${extraDays}`),

    updateDetailStatus: (detailId: string, loanStatus: number, bookStatus: number) => {
        return api.put(`/detail/${detailId}/update-status`, {
            loanDetailId: detailId,
            newLoanStatus: loanStatus,
            newBookStatus: bookStatus
        });
    }
};