import React, { useEffect, useState } from 'react';
import { reservationApi } from '../../api/reservationApi';

interface Reservation {
    id: string;
    bookTitle: string;
    barcode: string;
    reservedDate: string;
    expiryDate?: string;
    status: string;
}

const ReaderReservationHistory: React.FC = () => {
    const [reservations, setReservations] = useState<Reservation[]>([]);
    const [loading, setLoading] = useState(false);

    const fetchMyReservations = async () => {
        setLoading(true);
        try {
            const response = await reservationApi.getMyActive();
            setReservations(response.data);
        } catch (error) {
            console.error("Error fetching my reservations:", error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchMyReservations();
    }, []);

    const handleCancel = async (id: string) => {
        if (!window.confirm("Are you sure you want to cancel this reservation?")) return;
        try {
            await reservationApi.cancel(id);
            alert("Reservation canceled successfully.");
            fetchMyReservations();
        } catch (error) {
            alert("Failed to cancel reservation at this time.");
        }
    };

    // Hàm helper để hiển thị màu sắc trạng thái
    const getStatusStyle = (status: string) => {
        switch (status) {
            case 'Ready': return { color: '#52c41a', fontWeight: 'bold' }; // Sách đã có sẵn
            case 'Pending': return { color: '#faad14', fontWeight: 'bold' }; // Đang chờ trả sách
            default: return { color: '#999' };
        }
    };

    return (
        <div className="loan-history-container">
            <h3>My Reservations</h3>
            <p style={{ fontSize: '0.9rem', color: '#666', marginBottom: '20px' }}>
                Note: When the status changes to <b>"Ready"</b>, please come to the library to pick up the book before the expiration date.
            </p>

            <table className="management-table">
                <thead>
                    <tr>
                        <th>Book Title</th>
                        <th>Barcode</th>
                        <th>Reservation Date</th>
                        <th>Expiration Date</th>
                        <th>Status</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {loading ? (
                        <tr><td colSpan={6} style={{ textAlign: 'center' }}>Loading...</td></tr>
                    ) : reservations.length === 0 ? (
                        <tr><td colSpan={6} style={{ textAlign: 'center' }}>You have no reservation requests.</td></tr>
                    ) : (
                        reservations.map((item) => (
                            <tr key={item.id}>
                                <td style={{ fontWeight: 500 }}>{item.bookTitle}</td>
                                <td><code>{item.barcode}</code></td>
                                <td>{new Date(item.reservedDate).toLocaleDateString('vi-VN')}</td>
                                <td>{item.expiryDate ? new Date(item.expiryDate).toLocaleDateString('vi-VN') : '---'}</td>
                                <td style={getStatusStyle(item.status)}>
                                    {item.status === 'Ready' ? '● Ready to pick up' : '○ Pending'}
                                </td>
                                <td>
                                    <button
                                        onClick={() => handleCancel(item.id)}
                                        style={{
                                            padding: '4px 8px',
                                            backgroundColor: 'transparent',
                                            color: '#ff4d4f',
                                            border: '1px solid #ff4d4f',
                                            borderRadius: '4px',
                                            cursor: 'pointer'
                                        }}
                                    >
                                        Cancel Reservation
                                    </button>
                                </td>
                            </tr>
                        ))
                    )}
                </tbody>
            </table>
        </div>
    );
};

export default ReaderReservationHistory;