import React, { useEffect, useState } from 'react';
import { bookApi } from '../../api/bookApi';
import BookFormModal from '../books/BookFormModal';
import { useNavigate } from 'react-router-dom';

const BookManagement = () => {
    const navigate = useNavigate();

    const [books, setBooks] = useState<any[]>([]);
    const [loading, setLoading] = useState(true);
    const [showForm, setShowForm] = useState(false);
    const [editingBook, setEditingBook] = useState<any>(null);
    const [selectedBookItems, setSelectedBookItems] = useState<any[] | null>(null);
    const [selectedBookId, setSelectedBookId] = useState<string | null>(null);

    useEffect(() => { loadBooks(); }, []);

    const handleLogout = () => {
        localStorage.removeItem('token');
        navigate('/login');
    };

    const loadBooks = async () => {
        try {
            const res = await bookApi.getAll();
            setBooks(res.data);
        } catch (error) { console.error("Error:", error); }
        finally { setLoading(false); }
    };

    const handleSave = async (formData: FormData) => {
        try {
            if (editingBook) {
                await bookApi.update(editingBook.id, formData);
                alert("Update book successfully!");
            } else {
                await bookApi.create(formData);
                alert("Add book successfully!");
            }
            setShowForm(false);
            loadBooks();
        } catch (error: any) { alert("Error while saving!"); }
    };

    const handleViewCopies = async (bookId: string) => {
        try {
            const res = await bookApi.getAdminDetails(bookId);
            if (res.data && res.data.bookItems) {
                setSelectedBookItems(res.data.bookItems);
                setSelectedBookId(bookId);
        }
            else
                console.error("", res.data);
        } catch (error) {
            alert("Cant load book item list.");
        }
    };

    const handleAddCopy = async () => {
        if (!selectedBookId) return;

        const location = window.prompt("Enter Shelf Location:", "???");
        if (location === null) return;

        try {
            await bookApi.addBookItem({
                bookId: selectedBookId,
                shelfLocation: location,
                quantity: 1
            });

            alert("Added new copy successfully!");
            handleViewCopies(selectedBookId);
            loadBooks();
        } catch (error) {
            alert("Error adding new copy");
        }
    };

    const handleUpdateStatus = async (itemId: string, newStatus: number) => {
        // 1. Kiểm tra ID ngay lập tức
        if (!itemId || itemId === "undefined") {
            console.error("Error: BookItemID not exists!", itemId);
            alert("No book item found.");
            return;
        }

        try {
            await bookApi.updateItemStatus({
                bookItemId: itemId,
                status: newStatus
            });
            alert("Update successful!");
            handleViewCopies(selectedBookId!);
        } catch (error) {
            console.error("Update Status Error:", error);
            alert("Error while updating book item status.");
        }
    };

    const handleDeleteItem = async (itemId: string) => {
        if (window.confirm("Delete this copy?")) {
            try {
                await bookApi.deleteBookItem(itemId);

                if (selectedBookItems) {
                    const updatedItems = selectedBookItems.map(item =>
                        item.id === itemId ? { ...item, isDeleted: true, status: 'Discarded' } : item
                    );
                    setSelectedBookItems(updatedItems);
                }

                loadBooks();
                alert("Delete book success!");
            } catch (error) {
                alert("Error while deleting book item");
            }
        }
    };

    const handleRestoreItem = async (itemId: string) => {
        try {
            await bookApi.restoreBookItem(itemId);

            if (selectedBookItems) {
                const updatedItems = selectedBookItems.map(item =>
                    item.id === itemId ? { ...item, isDeleted: false } : item
                );
                setSelectedBookItems(updatedItems);
            }

            loadBooks();

            alert("Restore successful!");
        } catch (error) {
            console.error("Restore error:", error);
            alert("Error while restore!");
        }
    };

    const convertStatusNameToNumber = (statusName: string) => {
        switch (statusName) {
            case 'Available': return 1;
            case 'Reserved': return 2;
            case 'Loaned': return 3;
            case 'InRepair': return 4;
            case 'Lost': return 5;
            case 'Discarded': return 6;
            default: return 1;
        }
    };

    const getStatusClass = (status: any) => {
        const s = typeof status === 'string' ? status : String(status);
        if (s === '1' || s === 'Available') return 'available';
        if (s === '2' || s === 'Reserved' || s === '3' || s === 'Loaned') return 'unavailable';
        return 'maintenance'; // Cho các trạng thái InRepair, Lost...
    };

    const getImageUrl = (url: string) => {
        if (!url) return 'https://placehold.co/40x60?text=No+Cover';
        const baseUrl = import.meta.env.VITE_API_URL.replace(/\/$/, '');
        return url.startsWith('http') ? url : `${baseUrl}${url.startsWith('/') ? '' : '/'}${url}`;
    };

    return (
        <div className="main-content">
            <header className="header-actions" style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '20px' }}>
                <h2 style={{ color: 'var(--accent)' }}>Book Management</h2>
                <button className="btn-add" onClick={() => { setEditingBook(null); setShowForm(true); }}>
                    ➕ Add New Book
                </button>
            </header>

            <table className="data-table">
                <thead>
                    <tr>
                        <th>Book Information</th>
                        <th>ISBN</th>
                        <th>Quantity</th>
                        <th>Type</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {books.map((book) => (
                        <tr key={book.id}>
                            <td>
                                <div style={{ display: 'flex', gap: '15px', alignItems: 'center' }}>
                                    <img src={getImageUrl(book.coverImageUrl)} className="book-cover-small" style={{ width: '45px', borderRadius: '4px' }} alt="" />
                                    <div>
                                        <div style={{ fontWeight: 'bold' }}>{book.title}</div>
                                        <small>{book.author}</small>
                                    </div>
                                </div>
                            </td>
                            <td>{book.isbn}</td>
                            <td style={{ textAlign: 'center' }}>
                                <span style={{ fontWeight: 'bold', color: book.availableCopies > 0 ? '#28a745' : '#dc3545' }}>
                                    {book.availableCopies}
                                </span> / {book.totalCopies}
                            </td>
                            <td>{book.isDigital ? <span className="badge-digital">E-Book</span> : "Physical book"}</td>
                            <td>
                                <div style={{ display: 'flex', gap: '5px' }}>
                                    <button className="btn-restore" onClick={() => { setEditingBook(book); setShowForm(true); }}>Edit</button>
                                    <button className="btn-add" onClick={() => handleViewCopies(book.id)}>
                                        Copied
                                    </button>
                                </div>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>

            {/* Modal hiển thị danh sách bản sao (BookItems) */}
            {selectedBookItems && (
                <div className="modal-overlay">
                    <div className="modal-content" style={{ maxWidth: '800px' }}>
                        <h3 style={{ color: 'var(--accent)', marginBottom: '15px' }}>Detailed Copy List</h3>
                        <div style={{ maxHeight: '400px', overflowY: 'auto' }}>
                            <table className="data-table">
                                <thead>
                                    <tr>
                                        <th>Barcode</th>
                                        <th>Location</th>
                                        <th>Status</th>
                                        <th>Actions</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {selectedBookItems.map((item: any) => (
                                        <tr
                                            key={item.id}
                                            className={item.isDeleted ? "row-deleted" : ""}
                                            style={item.isDeleted ? { opacity: 0.5, backgroundColor: '#f9f9f9' } : {}}
                                        >
                                            <td><code>{item.barcode}</code></td>
                                            <td>{item.location || 'Undefined'}</td>
                                            <td>
                                                <td>
                                                    <span className={`status-pill ${item.isDeleted ? 'deleted' : getStatusClass(item.status)}`}>
                                                        {item.isDeleted ? 'Deleted' : (item.status)}
                                                    </span>
                                                </td>
                                            </td>
                                            <td>
                                                <select
                                                    value={typeof item.status === 'string' ? convertStatusNameToNumber(item.status) : item.status}
                                                    onChange={(e) => handleUpdateStatus(item.id, parseInt(e.target.value))}
                                                    // Khóa nếu: Đã xóa, Đang đặt (2/Reserved), Đang mượn (3/Loaned)
                                                    disabled={
                                                        item.isDeleted ||
                                                        item.status === 2 || item.status === 'Reserved' ||
                                                        item.status === 3 || item.status === 'Loaned' ||
                                                        item.status === 6 || item.status === 'Discarded'
                                                    }
                                                >
                                                    <option value={1}>Available</option>
                                                    <option value={4}>In Repair</option>
                                                    <option value={5}>Lost</option>
                                                    <option value={6}>Discarded</option>
                                                </select>
                                            </td>
                                            <td>
                                                {/* Logic hiển thị Nút bấm */}
                                                {!item.isDeleted ? (
                                                    <button
                                                        className="btn-delete"
                                                        onClick={() => handleDeleteItem(item.id)}
                                                        disabled={
                                                            item.status === 2 || item.status === 'Reserved' ||
                                                            item.status === 3 || item.status === 'Loaned'
                                                        }
                                                    >
                                                        Delete
                                                    </button>
                                                ) : (
                                                    <button
                                                        className="btn-restore"
                                                        style={{ backgroundColor: '#28a745', color: 'white', padding: '4px 12px', border: 'none', borderRadius: '4px', cursor: 'pointer' }}
                                                        onClick={() => handleRestoreItem(item.id)}
                                                    >
                                                        Restore
                                                    </button>
                                                )}
                                            </td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                        <div className="modal-actions" style={{ marginTop: '20px' }}>
                            <button className="btn-cancel" onClick={() => setSelectedBookItems(null)}>Close</button>
                        </div>
                    </div>
                </div>
            )}

            <BookFormModal
                isOpen={showForm}
                onClose={() => { setShowForm(false); setEditingBook(null); }}
                onSave={handleSave}
                initialData={editingBook}
            />
        </div>
    );
};

export default BookManagement;