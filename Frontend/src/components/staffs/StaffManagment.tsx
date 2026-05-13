import React, { useEffect, useState } from 'react';
import { adminApi } from '../../api/adminApi';
import StaffFormModal from './StaffFormModal';

const StaffManagement = () => {
    const [staffs, setStaffs] = useState<any[]>([]);
    const [loading, setLoading] = useState(true);
    const [showForm, setShowForm] = useState(false);
    const [editingStaff, setEditingStaff] = useState<any>(null);

    useEffect(() => {
        loadStaffs();
    }, []);

    const loadStaffs = async () => {
        try {
            const res = await adminApi.getAllStaff();
            setStaffs(res.data);
        } catch (error) {
            console.error("Error loading staffs:", error);
        } finally {
            setLoading(false);
        }
    };

    const handleSave = async (data: any) => {
        try {
            if (editingStaff) {
                await adminApi.updateStaff(editingStaff.id, {
                    ...data,
                    isDeleted: editingStaff.isDeleted
                });
                alert("Update successful!");
            } else {
                await adminApi.registerLibrarian(data);
                alert("New librarian registered successfully!");
            }
            setShowForm(false);
            setEditingStaff(null);
            loadStaffs();
        } catch (error: any) {
            alert(error.response?.data || "Error processing staff data");
        }
    };

    const handleToggleStatus = async (staff: any) => {
        const action = staff.isDeleted ? "restore" : "delete";
        if (window.confirm(`Are you sure you want to ${action} staff member ${staff.fullName}?`)) {
            try {
                if (staff.isDeleted) {
                    await adminApi.updateStaff(staff.id, {
                        fullName: staff.fullName,
                        phoneNumber: staff.phoneNumber,
                        isDeleted: false
                    });
                } else {
                    // Xóa logic (Deactivate)[cite: 25, 28]
                    await adminApi.deleteStaff(staff.id);
                }
                loadStaffs();
            } catch (error: any) {
                alert(error.response?.data || `Cannot ${action} staff member!`);
            }
        }
    };

    return (
        <div className="management-container">
            <header className="page-header">
                <h2>👔 Management System Staff</h2>
                <button className="btn-add" onClick={() => setShowForm(true)}>
                    ➕ Add New Librarian
                </button>
            </header>

            {loading ? <p>Loading data...</p> : (
                <table className="data-table">
                    <thead>
                        <tr>
                            <th>Full Name</th>
                            <th>Phone Number</th>
                            <th>Date of Birth</th>
                            <th>Status</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {staffs.map((s) => (
                            <tr key={s.id} className={s.isDeleted ? "row-deleted row-faded" : ""}>
                                <td>
                                    <strong>{s.fullName}</strong><br />
                                    <small style={{ color: '#888' }}>ID: {s.id.substring(0, 8)}...</small>
                                </td>
                                <td>{s.phoneNumber}</td>
                                <td>{s.dateOfBirth}</td>
                                <td>
                                    <span className={`status-pill ${s.isDeleted ? 'unavailable' : 'available'}`}>
                                        {s.isDeleted ? "🔴 Retired" : "🟢 Active"}
                                    </span>
                                </td>
                                <td>
                                    {/* Nút sửa bị vô hiệu hóa nếu nhân viên đã nghỉ việc */}
                                    <button
                                        className="btn-edit"
                                        onClick={() => { setEditingStaff(s); setShowForm(true); }}
                                        disabled={s.isDeleted}
                                    >
                                        Edit
                                    </button>

                                    {!s.isDeleted ? (
                                        <button className="btn-delete" onClick={() => handleToggleStatus(s)}>Delete</button>
                                    ) : (
                                        <button className="btn-restore" onClick={() => handleToggleStatus(s)}>Restore</button>
                                    )}
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}

            <StaffFormModal
                isOpen={showForm}
                onClose={() => { setShowForm(false); setEditingStaff(null); }}
                onSave={handleSave}
                initialData={editingStaff}
            />
        </div>
    );
};

export default StaffManagement;