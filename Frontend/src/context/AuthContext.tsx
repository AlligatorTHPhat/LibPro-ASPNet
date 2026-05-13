import React, { createContext, useContext, useState, useEffect } from 'react';
import { User, AuthResponse } from '../types/auth';
import { authApi } from '../api/authApi';

interface AuthContextType {
    user: User | null;
    profile: any;
    login: (data: AuthResponse) => void;
    logout: () => void;
    isAuthenticated: boolean;
}
interface AuthProviderProps {
    children: React.ReactNode;
}
const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
    const [user, setUser] = useState<User | null>(null);
    const [profile, setProfile] = useState<any>(null);

    useEffect(() => {
        const loadInitialData = async () => {
            const savedUser = localStorage.getItem('user');
            const token = localStorage.getItem('token');

            if (savedUser && token) {
                const parsedUser = JSON.parse(savedUser);

                if (parsedUser.id && parsedUser.id !== '00000000-0000-0000-0000-000000000000') {
                    setUser(parsedUser);
                    try {
                        const res = await authApi.getProfile(parsedUser.id, token);
                        setProfile(res.data);
                    } catch (err) {
                        console.error("Lỗi fetch profile khi refresh:", err);
                    }
                } else {
                    console.warn("ID trong localStorage không hợp lệ, yêu cầu đăng nhập lại.");
                    logout();
                }
            }
        };
        loadInitialData();
    }, []);

    const login = async (response: any) => {
        const data = response.data || response;

        console.log("Cấu trúc thực tế nhận được:", data);

        const userId = data.id;
        const token = data.token;

        if (!userId) {
            console.error("ID bị trống!");
            return;
        }

        if (userId !== '00000000-0000-0000-0000-000000000000') {
            try {
                const res = await authApi.getProfile(userId, token);
                setProfile(res.data);
            } catch (err) {
                console.error("Lỗi lấy profile:", err);
            }
        } else {
            setProfile({ fullName: "Quản trị viên", role: "Director" });
        }

        const userData = {
            id: userId,
            username: data.username,
            role: data.role,
            token: data.token
        };

        setUser(userData);
        localStorage.setItem('user', JSON.stringify(userData));
        localStorage.setItem('token', token);

        try {
            const res = await authApi.getProfile(userId, token);
            setProfile(res.data);
            console.log("Profile đã tải thành công:", res.data);
        } catch (err) {
            console.error("Lỗi lấy profile sau login:", err);
        }
    };

    const logout = () => {
        setUser(null);
        localStorage.clear();
    };

    return (
        <AuthContext.Provider value={{ user, profile, login, logout, isAuthenticated: !!user }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => {
    const context = useContext(AuthContext);
    if (!context) throw new Error("useAuth must be used within an AuthProvider");
    return context;
};