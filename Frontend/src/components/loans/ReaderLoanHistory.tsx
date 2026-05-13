import React, { useEffect, useState } from 'react';
import { loanApi } from '../../api/loanApi';
import LoanStatusBadge from './LoanStatusBadge';

const ReaderLoanHistory = () => {
    const [loans, setLoans] = useState<any[]>([]);

    

    useEffect(() => {
        const fetchHistory = async () => {
            try {
                const res = await loanApi.getMyHistory();
                console.log("History Loading:", res.data);
                setLoans(res.data);
            } catch (err) {
                console.error("Error while loading history:", err);
            }
        };
        fetchHistory();
    }, []);

    const handleExtend = async (loanDetailId: string) => {
        if (!window.confirm("Are you sure you want to extend the loan by 7 days?")) return;

        try {
            await loanApi.extendLoan(loanDetailId, 7);
            alert("Extend successful!");
            const res = await loanApi.getMyHistory();
            setLoans(res.data);
        } catch (err: any) {
            const errorMsg = err.response?.data?.Error || "Cannot extend loan (Someone has reserved it or it's overdue)";
            alert(errorMsg);
        }
    };

    return (
        <div className="loan-history-container">
            <h2>Your Loan History</h2>
            <table className="management-table">
                <thead>
                    <tr>
                        <th style={{ width: '35%' }}>Title / Ticket Number</th>
                        <th style={{ width: '15%' }}>Loan Date</th>
                        <th style={{ width: '15%' }}>Due Date</th>
                        <th style={{ width: '15%' }}>Status</th>
                        <th style={{ width: '20%' }}>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {loans && loans.length > 0 ? (
                        loans.map((loan) => (
                            <React.Fragment key={loan.id}>                                
                                {/* Các dòng chi tiết sách trong đơn đó */}
                                {loan.details && loan.details.length > 0 ? (
                                    loan.details.map((detail: any) => {
                                        const isLoanActive = detail.status === 'Active' || detail.status === 1;
                                        const isNotOverdue = new Date(detail.dueDate) > new Date();
                                        const hasNotRenewed = (detail.renewalCount || 0) < 1;
                                        const canExtend = isLoanActive && isNotOverdue && hasNotRenewed;
                                        return (
                                            <tr key={detail.id || detail.Id}>
                                                <td style={{ paddingLeft: '30px' }}>
                                                    {/* Mapping chuẩn: dùng bookTitle (viết thường chữ đầu) */}
                                                    <strong>{detail.bookTitle || detail.BookTitle || "N/A"}</strong>
                                                    <div style={{ fontSize: '0.8em', color: '#999' }}>Barcode: {detail.barcode || "N/A"}</div>
                                                </td>
                                                <td>{new Date(loan.loanDate).toLocaleDateString('vi-VN')}</td>
                                                <td>{detail.dueDate ? new Date(detail.dueDate).toLocaleDateString('vi-VN') : '---'}</td>
                                                <td>
                                                    <LoanStatusBadge
                                                        status={detail.status || detail.Status}
                                                        description={detail.status || detail.Status}
                                                    />
                                                </td>
                                                <td>
                                                    {detail.accessToken ? (
                                                        <a href={`/read/${detail.accessToken}`} className="btn-read">📖 Read E-Book</a>
                                                    ) : (
                                                        <div style={{ display: 'flex', flexDirection: 'column', gap: '5px' }}>
                                                            <span className="status-physical">At Counter</span>

                                                            {/* Sử dụng biến canExtend đã tính toán ở trên */}
                                                            {canExtend && (
                                                                <button
                                                                    onClick={() => handleExtend(detail.id)}
                                                                    className="btn-extend-action"
                                                                >
                                                                    ➕ Extend 7 days
                                                                </button>
                                                            )}

                                                            {detail.renewalCount > 0 && (
                                                                <small style={{ color: '#27ae60' }}>✓ Extended</small>
                                                            )}
                                                        </div>
                                                    )}
                                                </td>
                                            </tr>
                                        )
                                    })
                                ) : (
                                    <tr>
                                        <td colSpan={5} style={{ textAlign: 'center', fontStyle: 'italic', color: '#999' }}>
                                                Have no details for this loan.
                                        </td>
                                    </tr>
                                )}
                            </React.Fragment>
                        ))
                    ) : (
                        <tr>
                            <td colSpan={5} style={{ textAlign: 'center', padding: '40px' }}>
                                Loading data or you have no loan history.
                            </td>
                        </tr>
                    )}
                </tbody>
            </table>
        </div>
    );
};

export default ReaderLoanHistory;