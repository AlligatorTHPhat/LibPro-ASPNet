import React, { useState, useEffect } from 'react';

const StaffFormModal = ({ isOpen, onClose, onSave, initialData }: any) => {
    const [formData, setFormData] = useState({
        username: '',
        password: '',
        email: '',
        fullName: '',
        gender: 1,
        dateOfBirth: '',
        street: '',
        ward: '',
        district: '',
        city: '',
        phoneNumber: ''
    });

    useEffect(() => {
        if (isOpen) {
            if (initialData) {
                setFormData({
                    ...formData,
                    fullName: initialData.fullName || '',
                    phoneNumber: initialData.phoneNumber || '',
                    dateOfBirth: initialData.dateOfBirth || '',
                });
            } else {
                setFormData({
                    username: '', password: '', email: '', fullName: '',
                    gender: 1, dateOfBirth: '', street: '', ward: '',
                    district: '', city: '', phoneNumber: ''
                });
            }
        }
    }, [initialData, isOpen]);

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        onSave(formData);
    };

    if (!isOpen) return null;

    return (
        <div className="modal-overlay">
            <div className="modal-content staff-modal">
                <h3>{initialData ? `Update: ${initialData.fullName}` : "Register New Librarian"}</h3>
                <form onSubmit={handleSubmit} className="staff-form">
                    {!initialData && (
                        <fieldset>
                            <legend>Librarian Profile</legend>
                            <div className="form-row">
                                <div className="form-group">
                                    <label>Username:</label>
                                    <input type="text" required value={formData.username} onChange={e => setFormData({ ...formData, username: e.target.value })} />
                                </div>
                                <div className="form-group">
                                    <label>Email:</label>
                                    <input type="email" required value={formData.email} onChange={e => setFormData({ ...formData, email: e.target.value })} />
                                </div>
                            </div>
                            <div className="form-group">
                                <label>Password:</label>
                                <input type="password" required value={formData.password} onChange={e => setFormData({ ...formData, password: e.target.value })} />
                            </div>
                        </fieldset>
                    )}

                    <fieldset>
                        <legend>Personal Information</legend>
                        <div className="form-row">
                            <div className="form-group">
                                <label>Full Name:</label>
                                <input type="text" required value={formData.fullName} onChange={e => setFormData({ ...formData, fullName: e.target.value })} />
                            </div>
                            <div className="form-group">
                                <label>Phone Number:</label>
                                <input type="text" required value={formData.phoneNumber} onChange={e => setFormData({ ...formData, phoneNumber: e.target.value })} />
                            </div>
                        </div>
                        <div className="form-row">
                            <div className="form-group">
                                <label>Date of Birth:</label>
                                <input type="date" required value={formData.dateOfBirth} onChange={e => setFormData({ ...formData, dateOfBirth: e.target.value })} />
                            </div>
                            <div className="form-group">
                                <label>Gender:</label>
                                <select value={formData.gender} onChange={e => setFormData({ ...formData, gender: parseInt(e.target.value) })}>
                                    <option value={1}>Male</option>
                                    <option value={0}>Female</option>
                                </select>
                            </div>
                        </div>
                    </fieldset>

                    <fieldset>
                        <legend>Address</legend>
                        <div className="form-row">
                            <input type="text" placeholder="Street Number/Name" value={formData.street} onChange={e => setFormData({ ...formData, street: e.target.value })} />
                            <input type="text" placeholder="Ward/Commune" value={formData.ward} onChange={e => setFormData({ ...formData, ward: e.target.value })} />
                        </div>
                        <div className="form-row">
                            <input type="text" placeholder="District/County" value={formData.district} onChange={e => setFormData({ ...formData, district: e.target.value })} />
                            <input type="text" placeholder="Province/City" value={formData.city} onChange={e => setFormData({ ...formData, city: e.target.value })} />
                        </div>
                    </fieldset>

                    <div className="modal-actions">
                        <button type="button" onClick={onClose} className="btn-cancel">Cancel</button>
                        <button type="submit" className="btn-save">
                            {initialData ? "Update" : "Register Staff"}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default StaffFormModal;