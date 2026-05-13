import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { authApi } from '../../api/authApi';

const RegisterReaderForm = () => {
    const navigate = useNavigate();
    const [formData, setFormData] = useState({
        username: '', password: '', email: '',
        fullName: '', gender: 1, dateOfBirth: '',
        street: '', ward: '', district: '', city: '',
        phoneNumber: '', type: 1 
    });

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            await authApi.registerReader(formData);
            alert("Đăng ký thành công!");
            navigate('/login');
        } catch (error: any) {
            alert(error.response?.data?.message || "Lỗi đăng ký");
        }
    };

    return (
        <div className="auth-container">
            <div className="auth-card">
                <h2>Register</h2>
                <form onSubmit={handleSubmit} className="auth-form">
                    <section>
                        <h4>Account Information</h4>
                        <input type="text" placeholder="Username" required onChange={e => setFormData({ ...formData, username: e.target.value })} />
                        <div className="form-row">
                            <input type="password" placeholder="Password" required onChange={e => setFormData({ ...formData, password: e.target.value })} />
                            <input type="email" placeholder="Email" required onChange={e => setFormData({ ...formData, email: e.target.value })} />
                        </div>
                    </section>

                    <section>
                        <h4>Personal Information</h4>
                        <input type="text" placeholder="Full Name" required onChange={e => setFormData({ ...formData, fullName: e.target.value })} />
                        <div className="form-row">
                            <select value={formData.gender} onChange={e => setFormData({ ...formData, gender: parseInt(e.target.value) })}>
                                <option value={1}>Male</option>
                                <option value={2}>Female</option>
                            </select>
                            <input type="date" required onChange={e => setFormData({ ...formData, dateOfBirth: e.target.value })} />
                        </div>
                    </section>

                    <section>
                        <h4>Address & Contact</h4>
                        <input type="text" placeholder="Street Address" required onChange={e => setFormData({ ...formData, street: e.target.value })} />
                        <div className="form-row">
                            <input type="text" placeholder="Ward" required onChange={e => setFormData({ ...formData, ward: e.target.value })} />
                            <input type="text" placeholder="District" required onChange={e => setFormData({ ...formData, district: e.target.value })} />
                        </div>
                        <div className="form-row">
                            <input type="text" placeholder="City" required onChange={e => setFormData({ ...formData, city: e.target.value })} />
                            <input type="text" placeholder="Phone Number" required onChange={e => setFormData({ ...formData, phoneNumber: e.target.value })} />
                        </div>
                    </section>

                    <div className="form-row">
                        <div style={{ flex: 1 }}>
                            <label><small>Reader Type</small></label>
                            <select
                                className="form-control"
                                value={formData.type}
                                onChange={e => setFormData({ ...formData, type: parseInt(e.target.value) })}
                            >
                                <option value={1}>Student</option>
                                <option value={2}>Teacher</option>
                                <option value={3}>General Public</option>
                            </select>
                        </div>
                    </div>

                    <button type="submit" className="btn-primary">Complete Registration</button>

                    <div className="auth-footer">
                        <span>Already have an account? </span>
                        <Link to="/login" className="auth-link">Login here</Link>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default RegisterReaderForm;