import React, { useEffect, useState } from 'react';
import { loanApi } from '../../api/loanApi';

const LibrarianLoanManagement = () => {
    const [loanDetails, setLoanDetails] = useState<any[]>([]);
    const [loading, setLoading] = useState(true);
    const [selectedDetail, setSelectedDetail] = useState<any>(null);
    const [showModal, setShowModal] = useState(false);

    const [statusPair, setStatusPair] = useState({ loan: 2, book: 1 });

    useEffect(() => {
        loadActiveLoans();
    }, []);

    const loadActiveLoans = async () => {
        try {
            setLoading(true);
            const res = await loanApi.getAllActiveLoanDetails();
            setLoanDetails(res.data);
        } catch (error) {
            console.error("Error loading active loans:", error);
        } finally {
            setLoading(false);
        }
    };

    const handleActionChange = (actionValue: number) => {
        switch (actionValue) {
            case 2:
                setStatusPair({ loan: 2, book: 1 }); 
                break;
            case 6:
                setStatusPair({ loan: 6, book: 5 }); 
                break;
            case 4:
                setStatusPair({ loan: 4, book: 4 }); 
                break;
            default:
                setStatusPair({ loan: 2, book: 1 });
        }
    };

    const handleUpdateStatus = async () => {
        try {
            await loanApi.updateDetailStatus(
                selectedDetail.id,
                statusPair.loan,
                statusPair.book
            );

            setShowModal(false);

            setLoanDetails(prev => prev.filter(item => item.id !== selectedDetail.id));

            alert("Update system successfully!");

        } catch (err: any) {
            alert(err.response?.data?.Error || "Error occurred while updating");
        }
    };

    return (
        <div className="management-container">
            <h2>📝 Book Management</h2>

            {showModal && selectedDetail && (
                <div className="modal-overlay"> {/* Dùng class có sẵn trong hệ thống */}
                    <div className="modal-content" style={{ maxWidth: '500px', borderTop: '5px solid var(--accent)' }}>
                        <div className="modal-header">
                            <h3 style={{ color: 'var(--accent)', marginTop: 0 }}>
                                <span className="menu-icon">📥</span> Reclaim Book Copy
                            </h3>
                        </div>

                        <div className="modal-body" style={{ padding: '15px 0' }}>
                            <p style={{ fontSize: '0.95rem', marginBottom: '15px' }}>
                                Solve book copy: <strong>{selectedDetail.bookTitle}</strong>
                            </p>

                            <div className="info-badge-container" style={{ marginBottom: '20px' }}>
                                <code style={{ background: 'var(--code-bg)', padding: '5px 10px', borderRadius: '4px' }}>
                                    Barcode: {selectedDetail.barcode}
                                </code>
                            </div>

                            <div className="form-group" style={{ marginBottom: '20px' }}>
                                <label style={{ display: 'block', fontWeight: 'bold', marginBottom: '8px' }}>
                                    Reclaim Method:
                                </label>
                                <select
                                    className="data-table-select" // Giả định bạn có class cho select
                                    style={{ width: '100%', padding: '10px', borderRadius: '6px', border: '1px solid var(--border)' }}
                                    value={statusPair.loan}
                                    onChange={(e) => handleActionChange(Number(e.target.value))}
                                >
                                    <option value={2}>✅ Return</option>
                                    <option value={6}>❌ Report Lost (Lost)</option>
                                    <option value={4}>⚠️ Pending Fine (Damaged/Overdue)</option>
                                </select>
                            </div>

                            <div className="status-preview-box" style={{
                                background: 'var(--social-bg)',
                                padding: '15px',
                                borderRadius: '8px',
                                border: '1px dashed var(--accent-border)'
                            }}>
                                <p style={{ margin: '0 0 10px 0', fontSize: '0.9rem', fontWeight: 'bold' }}>System will update:</p>
                                <div style={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
                                    <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                                        <small>Loan Status:</small>
                                        <span className={`status-pill ${statusPair.loan === 2 ? 'available' : 'maintenance'}`}>
                                            {statusPair.loan === 2 ? "Returned" : statusPair.loan === 6 ? "Lost" : "Pending Fine"}
                                        </span>
                                    </div>
                                    <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                                        <small>Book Copy Status:</small>
                                        <span className={`status-pill ${statusPair.book === 1 ? 'available' : 'unavailable'}`}>
                                            {statusPair.book === 1 ? "Available" : statusPair.book === 5 ? "Lost" : "In Repair"}
                                        </span>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div className="modal-actions" style={{
                            display: 'flex',
                            gap: '10px',
                            justifyContent: 'flex-end',
                            marginTop: '20px',
                            paddingTop: '15px',
                            borderTop: '1px solid var(--border)'
                        }}>
                            <button className="btn-cancel" onClick={() => setShowModal(false)}>
                                Hủy bỏ
                            </button>
                            <button className="btn-add" onClick={handleUpdateStatus}>
                                Xác nhận xử lý
                            </button>
                        </div>
                    </div>
                </div>
            )}
            <table className="data-table">
                <thead>
                    <tr>
                        <th>Độc giả</th>
                        <th>Tên sách</th>
                        <th>Ngày mượn</th>
                        <th>Hạn trả</th>
                        <th>Thao tác</th>
                    </tr>
                </thead>
                <tbody>
                    {!loading && loanDetails.map((detail) => (
                        <tr key={detail.id}>
                            <td>{detail.readerName}</td>
                            <td>
                                <strong>{detail.bookTitle}</strong><br />
                                <small>Barcode: {detail.barcode}</small>
                            </td>
                            <td>{new Date(detail.loanDate).toLocaleDateString('vi-VN')}</td>
                            <td>{new Date(detail.dueDate).toLocaleDateString('vi-VN')}</td>
                            <td>
                                <button className="btn-add" onClick={() => {
                                    setSelectedDetail(detail);
                                    setStatusPair({ loan: 2, book: 1 }); 
                                    setShowModal(true);
                                }}>
                                    📥 Thu hồi
                                </button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
};

export default LibrarianLoanManagement;