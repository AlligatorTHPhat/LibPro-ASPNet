import axios from 'axios';
const baseURL = import.meta.env.VITE_API_URL

const api = axios.create({
    baseURL: `${baseURL}/api/Auth`
});

api.interceptors.response.use(
    (response) => response,
    (error) => {
        const message = error.response?.data?.message || "Đã có lỗi xảy ra";

        if (error.response?.status === 401) {
            // Nếu bị 401 (Hết hạn hoặc bị xóa khi đang dùng), logout luôn
            localStorage.removeItem('token');
            window.location.href = '/login';
        }

        // Trả lỗi về để catch ở component (như alert)
        return Promise.reject(error);
    }
);

export const authApi = {
    login: (credentials: any) => api.post('/login', credentials),

    registerReader: (data: any) => api.post('/register-reader', data),

    getMyInfo: (token: string) => api.get('/my-info', {
        headers: { Authorization: `Bearer ${token}` }
    }),

    getProfile: (id: string, token: string) => axios.get(`${baseURL}/api/Auth/${id}`, {
        headers: { Authorization: `Bearer ${token}` }
    })
};