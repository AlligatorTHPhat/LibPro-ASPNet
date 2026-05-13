import React, { useEffect, useState } from 'react';
import { publisherApi } from '../../api/publisherApi';

const PublisherManagement = () => {
    const [publishers, setPublishers] = useState<any[]>([]);
    const [loading, setLoading] = useState(true);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [editingPub, setEditingPub] = useState<any>(null);
    const [formData, setFormData] = useState({ name: '', email: '', phoneNumber: '', officeAddress: '' });

    useEffect(() => {
        loadPublishers();
    }, []);

    const loadPublishers = async () => {
        try {
            const res = await publisherApi.getAll();
            setPublishers(res.data);
        } catch (error) {
            console.error("Error loading publishers:", error);
        } finally {
            setLoading(false);
        }
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            if (editingPub) {
                await publisherApi.update(editingPub.id, formData);
                alert("Publisher updated successfully!");
            } else {
                await publisherApi.create(formData);
                alert("Publisher created successfully!");
            }
            setIsModalOpen(false);
            setEditingPub(null);
            setFormData({ name: '', email: '', phoneNumber: '', officeAddress: '' });
            loadPublishers();
        } catch (error) {
            alert("Action failed.");
        }
    };

    const handleEdit = (pub: any) => {
        setEditingPub(pub);
        setFormData({
            name: pub.name,
            email: pub.email || '',
            phoneNumber: pub.phoneNumber || '',
            officeAddress: pub.officeAddress || ''
        });
        setIsModalOpen(true);
    };

    return (
        <div className="main-content">
            <div className="header-actions" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }}>
                <h2 style={{ color: 'var(--accent)' }}>Publisher Management</h2>
                <button className="btn-add" onClick={() => { setEditingPub(null); setIsModalOpen(true); }}>
                    + Add New Publisher
                </button>
            </div>

            {loading ? <p>Loading...</p> : (
                <table className="data-table">
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Email</th>
                            <th>Phone</th>
                            <th>Address</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {publishers.map(p => (
                            <tr key={p.id}>
                                <td>{p.name}</td>
                                <td>{p.email}</td>
                                <td>{p.phoneNumber}</td>
                                <td>{p.officeAddress}</td>
                                <td>
                                    <button className="btn-restore" onClick={() => handleEdit(p)}>Edit</button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}

            {isModalOpen && (
                <div className="modal-overlay">
                    <div className="modal-content">
                        <h3>{editingPub ? 'Edit Publisher' : 'Add New Publisher'}</h3>
                        <form onSubmit={handleSubmit} className="book-form">
                            <div className="form-group">
                                <label>Publisher Name:</label>
                                <input required value={formData.name} onChange={e => setFormData({ ...formData, name: e.target.value })} />
                            </div>
                            <div className="form-group">
                                <label>Email:</label>
                                <input type="email" value={formData.email} onChange={e => setFormData({ ...formData, email: e.target.value })} />
                            </div>
                            <div className="form-group">
                                <label>Phone:</label>
                                <input value={formData.phoneNumber} onChange={e => setFormData({ ...formData, phoneNumber: e.target.value })} />
                            </div>
                            <div className="form-group">
                                <label>Office Address:</label>
                                <input value={formData.officeAddress} onChange={e => setFormData({ ...formData, officeAddress: e.target.value })} />
                            </div>
                            <div className="modal-actions">
                                <button type="button" className="btn-cancel" onClick={() => setIsModalOpen(false)}>Cancel</button>
                                <button type="submit" className="btn-save">Save Changes</button>
                            </div>
                        </form>
                    </div>
                </div>
            )}
        </div>
    );
};

export default PublisherManagement;