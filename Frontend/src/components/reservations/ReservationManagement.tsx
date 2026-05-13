import React, { useEffect, useState } from 'react';
import { reservationApi } from '../../api/reservationApi';
import ReservationStatusBadge from './ReservationStatusBadge';
import { ReservationStatus, ReservationStatusLabels } from '../../types/reservation';

interface Reservation {
    id: string;
    readerName: string;
    bookTitle: string;
    barcode: string;
    reservedDate: string;
    status: string;
}

const ReservationManagement: React.FC = () => {
    const [reservations, setReservations] = useState<Reservation[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const fetchReservations = async () => {
        setLoading(true);
        try {
            const response = await reservationApi.getPending();
            setReservations(response.data);
            setError(null);
        } catch (err: any) {
            setError("Can not load reservation list.");
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchReservations();
    }, []);

    const handleConfirm = async (id: string) => {
        if (!window.confirm("Borrow this book?")) return;
        try {
            await reservationApi.confirmToLoan(id);
            alert("Borrowed successfully!");
            fetchReservations();
        } catch (err: any) {
            alert(err.response?.data?.Error || "Error occurred while confirming loan.");
        }
    };

    const handleCancel = async (id: string) => {
        if (!window.confirm("Are you sure you want to cancel this request?")) return;
        try {
            await reservationApi.cancel(id);
            alert("Request canceled successfully.");
            fetchReservations();
        } catch (err: any) {
            alert("Error occurred while canceling request.");
        }
    };

    const renderStatusTag = (status: string) => {
        const statusKey = status as unknown as ReservationStatus;
        const label = ReservationStatusLabels[statusKey] || status;

        let statusClass = "status-pending";
        if (status === "Ready" || status === "1") statusClass = "status-ready";
        if (status === "Canceled" || status === "3") statusClass = "status-canceled";

        return <span className={`status-tag ${statusClass}`}>{label}</span>;
    };

    return (
        <div className="loan-history-container">
            <h2>Reservation Management</h2>

            {error && <p style={{ color: 'red' }}>{error}</p>}

            <table className="management-table">
                <thead>
                    <tr>
                        <th>Borrow people</th>
                        <th>Book Title</th>
                        <th>Barcode</th>
                        <th>Reservation Date</th>
                        <th>Status</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {loading ? (
                        <tr><td colSpan={6} style={{ textAlign: 'center' }}>Loading...</td></tr>
                    ) : reservations.length === 0 ? (
                        <tr><td colSpan={6} style={{ textAlign: 'center' }}>No reservation requests.</td></tr>
                    ) : (
                        reservations.map((item) => (
                            <tr key={item.id}>
                                <td>{item.readerName}</td>
                                <td>{item.bookTitle}</td>
                                <td><code>{item.barcode}</code></td>
                                <td>{new Date(item.reservedDate).toLocaleDateString('vi-VN')}</td>
                                <td>
                                    {/* Gọi component Badge ở đây */}
                                    <ReservationStatusBadge status={item.status} />
                                </td>
                                <td>
                                    <div style={{ display: 'flex', gap: '8px' }}>
                                        <button
                                            className="btn-confirm"
                                            onClick={() => handleConfirm(item.id)}
                                            // Sửa logic: Chỉ cho phép mượn khi status là Ready (giá trị 2 hoặc chuỗi "Ready")
                                            disabled={item.status.toString() !== "2" && item.status !== "Ready"}
                                            style={{
                                                backgroundColor: (item.status.toString() === "2" || item.status === "Ready") ? '#aa3bff' : '#ccc',
                                                color: '#fff',
                                                border: 'none',
                                                padding: '6px 12px',
                                                borderRadius: '4px',
                                                cursor: (item.status.toString() === "2" || item.status === "Ready") ? 'pointer' : 'not-allowed'
                                            }}
                                        >
                                            Confirm Borrow
                                        </button>
                                        <button
                                            className="btn-cancel"
                                            onClick={() => handleCancel(item.id)}
                                            style={{ backgroundColor: '#f44336', color: '#fff', border: 'none', padding: '6px 12px', borderRadius: '4px', cursor: 'pointer' }}
                                        >
                                            Cancel
                                        </button>
                                    </div>
                                </td>
                            </tr>
                        ))
                    )}
                </tbody>
            </table>
        </div>
    );
};

export default ReservationManagement;