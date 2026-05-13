import React, { useState, useEffect } from 'react';
import { categoryApi } from '../../api/categoryApi';
import { publisherApi } from '../../api/publisherApi';
import { supplierApi } from '../../api/supplierApi';

const BookFormModal = ({ isOpen, onClose, onSave, initialData }: any) => {
    const [formData, setFormData] = useState({
        title: '',
        isbn: '',
        categoryId: '',
        publisherId: '',
        supplierId: '',
        author: '',
        isDigital: false, 
        digitalUrl: '',   
        initialCopies: 1,
        defaultShelf: '???',
        description: '',
        coverImage: null as File | null
    });

    const [previewUrl, setPreviewUrl] = useState<string | null>(null);
    const [dataSources, setDataSources] = useState({ cats: [], pubs: [], sups: [] });

    useEffect(() => {
        if (isOpen) {
            loadData();
        }
    }, [isOpen]);

    useEffect(() => {
        return () => {
            if (previewUrl && !initialData?.coverImageUrl) {
                URL.revokeObjectURL(previewUrl);
            }
        };
    }, [previewUrl]);

    useEffect(() => {
        if (isOpen) {
            if (initialData) {
                setFormData({
                    title: initialData.title || '',
                    isbn: initialData.isbn || '',
                    categoryId: initialData.categoryId || '',
                    publisherId: initialData.publisherId || '',
                    supplierId: initialData.supplierId || '',
                    author: initialData.author || '',
                    isDigital: initialData.isDigital || false,
                    digitalUrl: initialData.digitalUrl || '',
                    initialCopies: initialData.initialCopies || 0,
                    defaultShelf: initialData.defaultShelf || 'Khu vực chờ',
                    description: initialData.description || '',
                    coverImage: null
                });
                if (initialData.coverImageUrl) {
                    setPreviewUrl(`${import.meta.env.VITE_API_URL}${initialData.coverImageUrl}`);
                }
            } else {
                setFormData({
                    title: '', isbn: '', categoryId: '', publisherId: '', supplierId: '',
                    author: '', description: '', isDigital: false, digitalUrl: '',
                    initialCopies: 1, defaultShelf: '???', coverImage: null
                });
                setPreviewUrl(null);
            }
        }
    }, [initialData, isOpen]);

    const loadData = async () => {
        try {
            const [c, p, s] = await Promise.all([
                categoryApi.getAll(),
                publisherApi.getAll(),
                supplierApi.getAll()
            ]);
            setDataSources({ cats: c.data, pubs: p.data, sups: s.data });
        } catch (error) {
            console.error("Error loading data:", error);
        }
    };

    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        if (e.target.files && e.target.files[0]) {
            const file = e.target.files[0];
            setFormData({ ...formData, coverImage: file });
            setPreviewUrl(URL.createObjectURL(file));
        }
    };

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        const submitData = new FormData();
        Object.keys(formData).forEach(key => {
            if (key === 'coverImage') {
                if (formData.coverImage) submitData.append('ImageFile', formData.coverImage);
            } else if ((formData as any)[key] !== null) {
                submitData.append(key, (formData as any)[key]);
            }

            if (formData.isDigital) {
                submitData.append('defaultShelf', 'E-Library');
                submitData.append('initialCopies', '1');
            }
        });
        onSave(submitData);
    };

    if (!isOpen) return null;

    return (
        <div className="modal-overlay">
            <div className="modal-content">
                <h3>{initialData ? `Edit: ${initialData.title}` : "Add New Book"}</h3>

                <form onSubmit={handleSubmit} className="book-form">

                    <div className="form-group image-upload-section">
                        <label>Book Cover Image (Upload File):</label>
                        <div className="upload-container">
                            {previewUrl && <img src={previewUrl} alt="Preview" className="img-preview" />}
                            <input type="file" accept="image/*" onChange={handleFileChange} />
                        </div>
                    </div>

                    <div className="form-row">
                        <div className="form-group">
                            <label>Book Title:</label>
                            <input type="text" required value={formData.title} onChange={e => setFormData({ ...formData, title: e.target.value })} />
                        </div>
                        <div className="form-group">
                            <label>Author:</label>
                            <input type="text" required value={formData.author} onChange={e => setFormData({ ...formData, author: e.target.value })} />
                        </div>
                    </div>

                    <div className="form-row">
                        <div className="form-group">
                            <label>ISBN:</label>
                            <input type="text" required value={formData.isbn} onChange={e => setFormData({ ...formData, isbn: e.target.value })} />
                        </div>
                        <div className="form-group">
                            <label>Category:</label>
                            <select required value={formData.categoryId} onChange={e => setFormData({ ...formData, categoryId: e.target.value })}>
                                <option value="">Select Category</option>
                                {dataSources.cats.map((item: any) => <option key={item.id} value={item.id}>{item.name}</option>)}
                            </select>
                        </div>
                    </div>

                    <div className="form-row">
                        <div className="form-group">
                            <label>Publisher:</label>
                            <select required value={formData.publisherId} onChange={e => setFormData({ ...formData, publisherId: e.target.value })}>
                                <option value="">Select Publisher</option>
                                {dataSources.pubs.map((item: any) => <option key={item.id} value={item.id}>{item.name}</option>)}
                            </select>
                        </div>
                        <div className="form-group">
                            <label>Supplier:</label>
                            <select required value={formData.supplierId} onChange={e => setFormData({ ...formData, supplierId: e.target.value })}>
                                <option value="">Select Supplier</option>
                                {dataSources.sups.map((item: any) => <option key={item.id} value={item.id}>{item.name}</option>)}
                            </select>
                        </div>
                    </div> 

                    <div className="form-group">
                        <div className="form-group">
                            <label>Book Description:</label>
                            <textarea required value={formData.description} onChange={e => setFormData({ ...formData, description: e.target.value })} />
                        </div>
                        <div className="radio-group">
                            <label>
                                <input type="radio" checked={!formData.isDigital} disabled={!!initialData} onChange={() => setFormData({ ...formData, isDigital: false })} />
                                Physical Book   
                            </label>
                            <label>
                                <input type="radio" checked={formData.isDigital} disabled={!!initialData} onChange={() => setFormData({ ...formData, isDigital: true })} />
                                E-Book
                            </label>
                        </div>
                    </div>

                    {formData.isDigital ? (
                        <div className="form-group">
                            <label>Link Book (URL E-book):</label>
                            <input type="text" value={formData.digitalUrl} onChange={e => setFormData({ ...formData, digitalUrl: e.target.value })} />
                        </div>
                    ) : (
                        <div className="form-row">
                                <div className="form-group">
                                    <label style={{ color: initialData ? '#999' : 'inherit' }}>Copies number:</label>
                                    <input
                                        type="number"
                                        name="initialCopies"
                                        value={formData.initialCopies}
                                        onChange={e => {
                                            const val = parseInt(e.target.value);
                                            setFormData({ ...formData, initialCopies: isNaN(val) ? 0 : val });
                                        }}
                                        min={0}
                                        className={initialData ? "input-disabled" : "input-standard"}
                                        disabled={!!initialData}
                                        readOnly={!!initialData}
                                    />
                                </div>
                            <div className="form-group">
                                <label>Default Shelf Location:</label>
                                <input type="text" value={formData.defaultShelf} onChange={e => setFormData({ ...formData, defaultShelf: e.target.value })} />
                            </div>
                        </div>
                    )}

                    <div className="modal-actions">
                        <button type="button" onClick={onClose} className="btn-cancel">Cancel</button>
                        <button type="submit" className="btn-save">
                            {initialData ? "Update Changes" : "Save Book"}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
};

                    

export default BookFormModal;