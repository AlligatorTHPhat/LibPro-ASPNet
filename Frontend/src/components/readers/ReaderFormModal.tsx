import React, { useState, useEffect } from 'react';
import { useAuth } from '../../context/AuthContext';

const ReaderFormModal = ({ isOpen, onClose, onSave, initialData }: any) => {
    const { user } = useAuth();

    const [formData, setFormData] = useState({
        accountId: user?.userId || '',
        libraryCardNumber: '',
        fullName: '',
        gender: 0,
        dateOfBirth: '',
        street: '',
        ward: '',
        district: '',
        city: '',
        phoneNumber: '',
        readerTypeName: 'Guest',
        isDeleted: false
    });

    useEffect(() => {
        if (initialData) {
            // Mapping for Update (Some fields from backend address string might need manual re-entry)
            setFormData(prev => ({
                ...prev,
                fullName: initialData.fullName || '',
                phoneNumber: initialData.phoneNumber || '',
                readerTypeName: initialData.readerTypeName || 'Guest',
                gender: initialData.gender || 0,
                dateOfBirth: initialData.dateOfBirth?.split('T')[0] || '',
                isDeleted: initialData.isDeleted || false
            }));
        }
    }, [initialData]);

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        onSave(formData);
    };

    if (!isOpen) return null;

    return (
        <div className="modal-overlay">
            <div className="modal-content" style={{ maxWidth: '700px', width: '90%' }}>
                <div className="modal-header">
                    <h3 style={{ color: 'var(--accent)' }}>
                        {initialData ? "Update Reader Profile" : "Register New Reader"}
                    </h3>
                </div>

                <form onSubmit={handleSubmit} className="management-form">
                    {/* Basic Information Section */}
                    <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '15px', marginBottom: '15px' }}>
                        <div className="form-group">
                            <label>Full Name</label>
                            <input type="text" value={formData.fullName} required
                                onChange={e => setFormData({ ...formData, fullName: e.target.value })} />
                        </div>
                        <div className="form-group">
                            <label>Phone Number</label>
                            <input type="text" value={formData.phoneNumber} required
                                onChange={e => setFormData({ ...formData, phoneNumber: e.target.value })} />
                        </div>
                    </div>

                    <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr 1fr', gap: '15px', marginBottom: '15px' }}>
                        <div className="form-group">
                            <label>Gender</label>
                            <select value={formData.gender} onChange={e => setFormData({ ...formData, gender: parseInt(e.target.value) })}>
                                <option value={0}>Female</option>
                                <option value={1}>Male</option>
                            </select>
                        </div>
                        <div className="form-group">
                            <label>Date of Birth</label>
                            <input type="date" value={formData.dateOfBirth} required
                                onChange={e => setFormData({ ...formData, dateOfBirth: e.target.value })} />
                        </div>
                        <div className="form-group">
                            <label>Reader Type</label>
                            <select value={formData.readerTypeName} onChange={e => setFormData({ ...formData, readerTypeName: e.target.value })}>
                                <option value="Guest">Guest</option>
                                <option value="Student">Student</option>
                                <option value="Teacher">Teacher</option>
                            </select>
                        </div>
                    </div>

                    {/* Address Section */}
                    <label style={{ fontWeight: 'bold', fontSize: '0.9rem', marginBottom: '10px', display: 'block' }}>Address Information</label>
                    <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '10px', backgroundColor: 'var(--code-bg)', padding: '15px', borderRadius: '8px' }}>
                        <div className="form-group">
                            <input placeholder="Street Name" value={formData.street} onChange={e => setFormData({ ...formData, street: e.target.value })} />
                        </div>
                        <div className="form-group">
                            <input placeholder="Ward" value={formData.ward} onChange={e => setFormData({ ...formData, ward: e.target.value })} />
                        </div>
                        <div className="form-group">
                            <input placeholder="District" value={formData.district} onChange={e => setFormData({ ...formData, district: e.target.value })} />
                        </div>
                        <div className="form-group">
                            <input placeholder="City" value={formData.city} onChange={e => setFormData({ ...formData, city: e.target.value })} />
                        </div>
                    </div>

                    <div className="modal-actions" style={{ marginTop: '25px', borderTop: '1px solid var(--border)', paddingTop: '15px' }}>
                        <button type="button" onClick={onClose} className="btn-cancel">Cancel</button>
                        <button type="submit" className="btn-save" style={{ backgroundColor: 'var(--accent)' }}>
                            {initialData ? "Save Changes" : "Create Account"}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default ReaderFormModal;