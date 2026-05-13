import React, { useState } from 'react';
import { useAuth } from '../../context/AuthContext';
import { authApi } from '../../api/authApi';
import { useNavigate } from 'react-router-dom';
import { Link } from 'react-router-dom';

const LoginForm = () => {
    const [credentials, setCredentials] = useState({ username: '', password: '' });
    const { login } = useAuth();
    const navigate = useNavigate();

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            const res = await authApi.login(credentials);
            console.log("Loading data:", res.data);

            const responseData = res.data.data || res.data;
            const token = responseData.token || res.data.token;

            if (token) {
                login(responseData);
                alert("Login Successful");

                const role = responseData.role;
                console.log("Role received:", role);

                switch (role) {
                    case 'Director':
                        navigate('/admin/dashboard');
                        break;
                    case 'Librarian':
                        navigate('/librarian/categories');
                        break;
                    case 'Reader':
                        navigate('/reader/books');
                        break;
                    default:
                        console.warn("Role not recognized:", role);
                        navigate('/unauthorized');
                }
            } else {
                alert("Login failed: No Token found in response");
            }
        } catch (error: any) {
            console.error("Full Error Object:", error);

            if (error.response) {
                alert(`Server Error: ${error.response.data?.message || "Lỗi không xác định"}`);
            } else if (error.request) {
                alert("No response received from Server. Please check the Backend!");
            } else {
                alert(`Client Logic Error: ${error.message}`);
            }
        }
    };

    return (
        <div className="login-container">
            <h2>Login</h2>
            <form className="login-form" onSubmit={handleSubmit}>
                <input
                    type="text"
                    placeholder="Username"
                    required
                    onChange={(e) => setCredentials({ ...credentials, username: e.target.value })}
                />
                <input
                    type="password"
                    placeholder="Password"
                    required
                    onChange={(e) => setCredentials({ ...credentials, password: e.target.value })}
                />
                <button type="submit" className="login-button">Login</button>
                <div className="register-link" style={{ marginTop: '15px', textAlign: 'center' }}>
                    <p>Don't have an account? <Link to="/register" style={{ color: '#007bff', fontWeight: 'bold' }}>Register now    !</Link></p>
                </div>
            </form>
        </div>
    );
};

export default LoginForm;