import React, { useEffect, useState } from 'react';
import { categoryApi } from '../../api/categoryApi';

const CategoryManagement = () => {
    const [categories, setCategories] = useState<any[]>([]);
    const [loading, setLoading] = useState(true);
    const [showAddForm, setShowAddForm] = useState(false);
    const [newCategory, setNewCategory] = useState({ name: '', description: '', parentCategoryId: null as string | null });

    useEffect(() => {
        loadCategories();
    }, []);

    const loadCategories = async () => {
        try {
            const res = await categoryApi.getAll();
            setCategories(res.data);
        } catch (error) {
            console.error("Error:", error);
        } finally {
            setLoading(false);
        }
    };

    const handleCreate = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            await categoryApi.create(newCategory);
            alert("Add category successful!");
            setShowAddForm(false);
            setNewCategory({ name: '', description: '', parentCategoryId: null });
            loadCategories();
        } catch (error: any) {
            alert(error.response?.data?.message || "Category name already exists");
        }
    };

    return (
        <div className="main-content">
            <div className="header-actions" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }}>
                <h2 style={{ color: 'var(--accent)' }}></h2>
                <button className="btn-add" onClick={() => setShowAddForm(true)}>
                    ➕ Add New Category
                </button>
            </div>

            {showAddForm && (
                <div className="modal-overlay">
                    <div className="modal-content">
                        <h3>Add New Category</h3>
                        <form onSubmit={handleCreate} className="book-form">
                            <div className="form-group">
                                <label>Category Name:</label>
                                <input type="text" value={newCategory.name} onChange={e => setNewCategory({ ...newCategory, name: e.target.value })} required />
                            </div>
                            <div className="form-group">
                                <label>Description:</label>
                                <input type="text" value={newCategory.description} onChange={e => setNewCategory({ ...newCategory, description: e.target.value })} />
                            </div>
                            <div className="form-group">
                                <label>Parent Category:</label>
                                <select value={newCategory.parentCategoryId || ""} onChange={e => setNewCategory({ ...newCategory, parentCategoryId: e.target.value || null })} >
                                    <option value="">-- No parent (Root category) --</option>
                                    {categories.map(cat => (
                                        <option key={cat.id} value={cat.id}>{cat.name}</option>
                                    ))}
                                </select>
                            </div>
                            <div className="modal-actions">
                                <button type="button" className="btn-cancel" onClick={() => setShowAddForm(false)}>Cancel</button>
                                <button type="submit" className="btn-save">Save Category</button>
                            </div>
                        </form>
                    </div>
                </div>
            )}

            {loading ? <p>Loading data...</p> : (
                <table className="data-table">
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Description</th>
                            <th>Book Count</th>
                        </tr>
                    </thead>
                    <tbody>
                        {categories.map((cat) => (
                            <tr key={cat.id}>
                                <td>{cat.name}</td>
                                <td>{cat.description}</td>
                                <td style={{ textAlign: 'center' }}>{cat.bookCount}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}
        </div>
    );
};

export default CategoryManagement;