import React, { useEffect, useState } from 'react';
import { adminApi } from '../../api/adminApi';

const SystemAuditLog = () => {
    const [logs, setLogs] = useState([]);

    useEffect(() => {
        adminApi.getAllLogs().then(res => setLogs(res.data));
    }, []);

    return (
        <div className="management-card">
            <h2>⚙️ Audit Log</h2>
            <div className="log-container">
                <table className="data-table">
                    <thead>
                        <tr>
                            <th>Time</th> <th>User</th> <th>Action</th> <th>Details</th>
                        </tr>
                    </thead>
                    <tbody>
                        {logs.map((log: any) => (
                            <tr key={log.id}>
                                <td>{new Date(log.timestamp).toLocaleString()}</td>
                                <td><strong>{log.userName}</strong></td>
                                <td><span className="badge-action">{log.action}</span></td>
                                <td style={{ fontSize: '0.85rem' }}>{log.details}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
};

export default SystemAuditLog;