import React, { useEffect, useState } from 'react';
import { supplierApi } from '../../api/supplierApi';

const SupplierManagement = () => {
    const [suppliers, setSuppliers] = useState<any[]>([]);
    const [loading, setLoading] = useState(true);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [editingSup, setEditingSup] = useState<any>(null);
    const [formData, setFormData] = useState({
        name: '',
        taxCode: '',
        contactPerson: '',
        email: '',
        phoneNumber: '',
        officeAddress: ''
    });

    useEffect(() => {
        loadSuppliers();
    }, []);

    const loadSuppliers = async () => {
        try {
            const res = await supplierApi.getAll();
            setSuppliers(res.data);
        } catch (error) {
            console.error("Error loading suppliers:", error);
        } finally {
            setLoading(false);
        }
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            if (editingSup) {
                await supplierApi.update(editingSup.id, formData);
            } else {
                await supplierApi.create(formData);
            }
            setIsModalOpen(false);
            setEditingSup(null);
            loadSuppliers();
            alert("Operation successful!");
        } catch (error) {
            alert("Error saving supplier.");
        }
    };

    const handleEdit = (s: any) => {
        setEditingSup(s);
        setFormData({ ...s });
        setIsModalOpen(true);
    };

    return (
        <div className="main-content">
            <div className="header-actions" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }}>
                <h2 style={{ color: 'var(--accent)' }}>Supplier Management</h2>
                <button className="btn-add" onClick={() => { setEditingSup(null); setIsModalOpen(true); }}>
                    + Register Supplier
                </button>
            </div>

            {loading ? <p>Loading...</p> : (
                <table className="data-table">
                    <thead>
                        <tr>
                            <th>Company Name</th>
                            <th>Tax Code</th>
                            <th>Contact Person</th>
                            <th>Email</th>
                            <th>Phone</th>
                            <th>Address</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {suppliers.map(s => (
                            <tr key={s.id}>
                                <td>{s.name}</td>
                                <td>{s.taxCode}</td>
                                <td>{s.contactPerson}</td>
                                <td>{s.email}</td>
                                <td>{s.phoneNumber}</td>
                                <td>{s.officeAddress}</td>
                                <td>
                                    <button className="btn-restore" onClick={() => handleEdit(s)}>Edit</button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}

            {isModalOpen && (
                <div className="modal-overlay">
                    <div className="modal-content">
                        <h3>{editingSup ? 'Edit Supplier' : 'Register New Supplier'}</h3>
                        <form onSubmit={handleSubmit} className="book-form">
                            <div className="form-group">
                                <label>Company Name:</label>
                                <input required value={formData.name} onChange={e => setFormData({ ...formData, name: e.target.value })} />
                            </div>
                            <div className="form-row">
                                <div className="form-group">
                                    <label>Tax Code:</label>
                                    <input value={formData.taxCode} onChange={e => setFormData({ ...formData, taxCode: e.target.value })} />
                                </div>
                                <div className="form-group">
                                    <label>Contact Person:</label>
                                    <input value={formData.contactPerson} onChange={e => setFormData({ ...formData, contactPerson: e.target.value })} />
                                </div>
                            </div>
                            <div className="form-row">
                                <div className="form-group">
                                    <label>Email:</label>
                                    <input type="email" value={formData.email} onChange={e => setFormData({ ...formData, email: e.target.value })} />
                                </div>
                                <div className="form-group">
                                    <label>Phone Number:</label>
                                    <input value={formData.phoneNumber} onChange={e => setFormData({ ...formData, phoneNumber: e.target.value })} />
                                </div>
                            </div>
                            <div className="form-group">
                                <label>Office Address:</label>
                                <input value={formData.officeAddress} onChange={e => setFormData({ ...formData, officeAddress: e.target.value })} />
                            </div>
                            <div className="modal-actions">
                                <button type="button" className="btn-cancel" onClick={() => setIsModalOpen(false)}>Close</button>
                                <button type="submit" className="btn-save">Submit</button>
                            </div>
                        </form>
                    </div>
                </div>
            )}
        </div>
    );
};

export default SupplierManagement;