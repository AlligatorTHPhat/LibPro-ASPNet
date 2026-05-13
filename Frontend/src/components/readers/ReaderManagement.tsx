import React, { useCallback, useEffect, useState } from 'react';
import { readerApi } from '../../api/readerApi';
import ReaderFormModal from './ReaderFormModal';

const ReaderManagement = () => {
    const [readers, setReaders] = useState<any[]>([]);
    const [loading, setLoading] = useState(true);
    const [showModal, setShowModal] = useState(false);
    const [editingReader, setEditingReader] = useState<any>(null);

    const handleDelete = async (id: string) => {
        if (window.confirm("Are you sure you want to DEACTIVATE this reader and lock their account?")) {
            try {
                await readerApi.delete(id);
                alert("Reader deactivated!");
                loadReaders();
            } catch (error: any) {
                alert(error.response?.data?.message || "Error deactivating");
            }
        }
    };

    const handleRestore = async (id: string) => {
        try {
            await readerApi.restore(id);
            alert("Reader restored!");
            loadReaders();
        } catch (error: any) {
            alert("Error restoring reader");
        }
    };

    const loadReaders = useCallback( async () => {
        try {
            const res = await readerApi.getAll();
            setReaders(res.data);
        } catch (error) {
            console.error("Error loading readers:", error);
        } finally {
            setLoading(false);
        }
    }, []);

    const handleSave = async (formData: any) => {
        try {
            if (editingReader) {
                await readerApi.update(editingReader.id, formData);
                alert("Update reader successfully!");
            }
            setShowModal(false);
            setEditingReader(null);
            loadReaders();
        } catch (error: any) {
            alert(error.response?.data?.error || "Error saving reader");
        }
    };

    useEffect(() => {
        loadReaders();
    }, [loadReaders]);
    return (
        <div className="management-container">
            <div className="header-actions" style={{ marginBottom: '20px' }}>
                <div>
                    <h2 style={{ color: 'var(--accent)' }}>Reader Management</h2>
                    <p style={{ fontSize: '0.9rem', color: 'var(--text)' }}>Manage library members and their profiles</p>
                </div>
            </div>

            <table className="management-table">
                <thead>
                    <tr>
                        <th>Full Name</th>
                        <th>Card Number</th>
                        <th>Contact</th>
                        <th>Address</th>
                        <th>Type</th>
                        <th>Status</th>
                        <th>Expiry Date</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {readers.map(reader => (
                        <tr key={reader.id}>
                            <td style={{ fontWeight: '500', color: 'var(--text-h)' }}>{reader.fullName}</td>
                            <td><code>{reader.libraryCardNumber}</code></td>
                            <td>
                                <div style={{ fontSize: '0.85rem' }}>{reader.phoneNumber}</div>
                            </td>
                            <td title={reader.address}>
                                <div style={{ maxWidth: '180px', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                                    {reader.address}
                                </div>
                            </td>
                            <td>
                                <span className={`status-badge status-${reader.readerTypeName.toLowerCase()}`}>
                                    {reader.readerTypeName}
                                </span>
                            </td>
                            <td>
                                {reader.isDeleted ?
                                    <span style={{ color: '#f44336' }}>● Inactive</span> :
                                    <span style={{ color: '#28a745' }}>● Active</span>
                                }
                            </td>
                            <td>{new Date(reader.expiryDate).toLocaleDateString('en-GB')}</td>
                            <td>
                                <button className="btn-edit" onClick={() => { setEditingReader(reader); setShowModal(true); }}>
                                    Edit
                                </button>
                                {reader.isDeleted ? (
                                    <button className="btn-restore" onClick={() => handleRestore(reader.id)} style={{ marginLeft: '5px' }}>
                                        Restore
                                    </button>
                                ) : (
                                    <button className="btn-delete" onClick={() => handleDelete(reader.id)} style={{ marginLeft: '5px' }}>
                                        Deactivate
                                    </button>
                                )}
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>

            {showModal && (
                <ReaderFormModal
                    isOpen={showModal}
                    onClose={() => setShowModal(false)}
                    onSave={handleSave}
                    initialData={editingReader}
                />
            )}
        </div>
    );
};

export default ReaderManagement;