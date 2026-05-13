import React, { useEffect, useState } from 'react';
import { PieChart, Pie, Cell, Tooltip, ResponsiveContainer, Legend, BarChart, Bar, XAxis, YAxis, CartesianGrid } from 'recharts';
import { reportApi } from '../../api/reportApi';
import { DashboardResponse } from '../../types/report';

const AdminDashboard = () => {
    const [data, setData] = useState<DashboardResponse | null>(null);
    const [topBooks, setTopBooks] = useState<any[]>([]);
    const [riskReaders, setRiskReaders] = useState<any[]>([]);
    const [circulationRate, setCirculationRate] = useState<number>(0);

    const LOAN_COLORS: { [key: string]: string } = {
        'Borrowed': '#007bff',    // Xanh dương
        'Overdue': '#dc3545',     // Đỏ
        'Returned': '#28a745',    // Xanh lá
        'Lost': '#6c757d',        // Xám
        'FinePayment': '#fd7e14', // Cam
        'Closed': '#343a40'       // Đen/Xám đậm
    };

    const BOOK_COLORS: { [key: string]: string } = {
        'Available': '#28a745',
        'Reserved': '#ffc107',
        'Loaned': '#007bff',
        'InRepair': '#17a2b8',
        'Lost': '#dc3545',
        'Discarded': '#6c757d'
    };

    useEffect(() => {
        const fetchDashboardData = async () => {
            try {
                const [summary, books, readers, rate] = await Promise.all([
                    reportApi.getDashboardSummary(),
                    reportApi.getTopBooks(5),
                    reportApi.getHighRiskReaders(5),
                    reportApi.getCirculationRate()
                ]);
                setData(summary);
                setTopBooks(books);
                setRiskReaders(readers);
                setCirculationRate(rate);
            } catch (error) {
                console.error("Lỗi khi tải dữ liệu báo cáo:", error);
            }
        };
        fetchDashboardData();
    }, []);

    if (!data) return <div style={{ padding: '20px' }}>Đang khởi tạo dữ liệu hệ thống...</div>;

    return (
        <div className="admin-dashboard-container" style={{ padding: '24px', backgroundColor: '#f4f6f9', minHeight: '100vh' }}>

            {/* Row 1: Stat Cards */}
            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(180px, 1fr))', gap: '20px', marginBottom: '25px' }}>
                <StatCard title="Total Books" value={data.totalBooks.toLocaleString()} color="#212529" />
                <StatCard title="Readers" value={data.totalReaders} color="#212529" />
                <StatCard title="Circulation Rate" value={`${circulationRate}%`} color="#673ab7" />
                <StatCard title="Currently Borrowed" value={data.activeLoans} color="#007bff" />
                <StatCard title="Overdue" value={data.overdueLoans} color="#dc3545" isAlert={data.overdueLoans > 0} />
            </div>

            <div style={{ display: 'grid', gridTemplateColumns: '1.5fr 1fr', gap: '25px', alignItems: 'start' }}>

                {/* Cột trái: Biểu đồ */}
                <div style={{ display: 'flex', flexDirection: 'column', gap: '25px' }}>

                    {/* Bar Chart: Top Books */}
                    <div style={{ background: '#fff', padding: '24px', borderRadius: '12px', boxShadow: '0 4px 12px rgba(0,0,0,0.05)' }}>
                        <h3 style={{ marginBottom: '20px', fontSize: '18px', fontWeight: 600 }}>Top 5 Most Borrowed Books</h3>
                        <div style={{ width: '100%', height: 320 }}>
                            <ResponsiveContainer width="100%" height="100%">
                                <BarChart data={topBooks} layout="vertical" margin={{ left: 40, right: 30 }}>
                                    <CartesianGrid strokeDasharray="3 3" horizontal={false} stroke="#eee" />
                                    <XAxis type="number" hide />
                                    <YAxis dataKey="title" type="category" width={140} tick={{ fontSize: 12 }} />
                                    <Tooltip cursor={{ fill: '#f8f9fa' }} />
                                    <Bar dataKey="borrowCount" fill="#4e73df" radius={[0, 4, 4, 0]} barSize={25} />
                                </BarChart>
                            </ResponsiveContainer>
                        </div>
                    </div>

                    {/* Hai biểu đồ tròn nằm ngang */}
                    <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '20px' }}>

                        <ChartBox title="Phân bổ kho sách">
                            <PieChart>
                                <Pie
                                    data={data.bookDistribution}
                                    dataKey="count"
                                    nameKey="status"
                                    cx="50%" cy="45%"
                                    innerRadius={65}
                                    outerRadius={90}
                                    paddingAngle={5}
                                >
                                    {data.bookDistribution.map((entry, index) => (
                                        <Cell key={index} fill={BOOK_COLORS[entry.status] || '#cbd5e0'} />
                                    ))}
                                </Pie>
                                <Tooltip />
                                <Legend iconType="circle" wrapperStyle={{ fontSize: '12px' }} />
                            </PieChart>
                        </ChartBox>

                        <ChartBox title="Trạng thái mượn trả">
                            <PieChart>
                                <Pie
                                    data={data.loanDistribution}
                                    dataKey="count"
                                    nameKey="status"
                                    cx="50%" cy="45%"
                                    innerRadius={65}
                                    outerRadius={90}
                                    paddingAngle={5}
                                >
                                    {data.loanDistribution.map((entry, index) => (
                                        <Cell key={index} fill={LOAN_COLORS[entry.status] || '#cbd5e0'} />
                                    ))}
                                </Pie>
                                <Tooltip />
                                <Legend iconType="circle" wrapperStyle={{ fontSize: '12px' }} />
                            </PieChart>
                        </ChartBox>
                    </div>
                </div>

                {/* Cột phải: Tài chính & Readers */}
                <div style={{ display: 'flex', flexDirection: 'column', gap: '25px' }}>
                    <div style={{ background: 'linear-gradient(135deg, #1e3a8a 0%, #3b82f6 100%)', padding: '30px', borderRadius: '12px', color: '#fff', boxShadow: '0 4px 15px rgba(59, 130, 246, 0.3)' }}>
                        <h3 style={{ fontSize: '15px', textTransform: 'uppercase', letterSpacing: '1px', opacity: 0.8, marginBottom: '8px' }}>Tổng doanh thu phạt</h3>
                        <p style={{ fontSize: '32px', fontWeight: 'bold', margin: 0 }}>
                            {data.totalRevenue.toLocaleString()} <span style={{ fontSize: '16px', fontWeight: 400 }}>VND</span>
                        </p>
                    </div>

                    <div style={{ background: '#fff', padding: '24px', borderRadius: '12px', boxShadow: '0 4px 12px rgba(0,0,0,0.05)', minHeight: '400px' }}>
                        <h3 style={{ marginBottom: '15px', fontSize: '18px', color: '#dc3545', fontWeight: 600 }}>High-Risk Readers </h3>
                        <div style={{ overflowX: 'auto' }}>
                            <table style={{ width: '100%', borderCollapse: 'collapse' }}>
                                <thead>
                                    <tr style={{ borderBottom: '1px solid #edf2f7', textAlign: 'left', color: '#718096', fontSize: '13px' }}>
                                        <th style={{ padding: '12px 8px' }}>FULL NAME</th>
                                        <th style={{ textAlign: 'center' }}>OVERDUE</th>
                                        <th style={{ textAlign: 'right' }}>FINE</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {riskReaders.map((reader, index) => (
                                        <tr key={index} style={{ borderBottom: '1px solid #f7fafc', fontSize: '14px' }}>
                                            <td style={{ padding: '12px 8px', fontWeight: 500 }}>{reader.fullName}</td>
                                            <td style={{ textAlign: 'center', color: '#dc3545', fontWeight: 'bold' }}>{reader.currentlyOverdueCount}</td>
                                            <td style={{ textAlign: 'right' }}>{reader.totalUnpaidFine.toLocaleString()}</td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                        {riskReaders.length === 0 && <p style={{ textAlign: 'center', color: '#a0aec0', marginTop: '30px' }}>No violation data available</p>}
                    </div>
                </div>
            </div>
        </div>
    );
};

// Sub-components
const StatCard = ({ title, value, color, isAlert }: any) => (
    <div style={{
        background: '#fff', padding: '20px', borderRadius: '12px',
        boxShadow: '0 2px 4px rgba(0,0,0,0.04)',
        borderLeft: isAlert ? '5px solid #dc3545' : `5px solid ${color === '#212529' ? '#e2e8f0' : color}`
    }}>
        <h4 style={{ margin: 0, color: '#718096', fontSize: '13px', textTransform: 'uppercase', marginBottom: '8px' }}>{title}</h4>
        <p style={{ margin: 0, fontSize: '24px', fontWeight: 700, color: color }}>{value}</p>
    </div>
);

const ChartBox = ({ title, children }: any) => (
    <div style={{
        background: '#fff', padding: '24px', borderRadius: '12px',
        boxShadow: '0 4px 12px rgba(0,0,0,0.05)',
        display: 'flex', flexDirection: 'column', height: '420px'
    }}>
        <h4 style={{ textAlign: 'center', marginBottom: '15px', fontSize: '16px', fontWeight: 600, color: '#2d3748' }}>{title}</h4>
        <div style={{ width: '100%', height: '100%', minHeight: 0 }}>
            <ResponsiveContainer width="100%" height="100%">
                {children}
            </ResponsiveContainer>
        </div>
    </div>
);

export default AdminDashboard;