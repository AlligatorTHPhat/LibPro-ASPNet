export interface User {
    username: string;
    role: string;
    token: string;
    userId?: string;
}

export interface AuthResponse {
    message: string;
    token: string;
    data: {
        username: string;
        role: string;
        token: string;
    };
}