import React, { useState } from 'react';
import { BookResponse } from '../../types/book';
import { LoanItemRequest, OnlineLoanRequest } from '../../types/loan';

interface Props {
    book: BookResponse;
    onClose: () => void;
    onConfirm: (request: OnlineLoanRequest) => void;
}

const BorrowModal: React.FC<Props> = ({ book, onClose, onConfirm }) => {
    const [loanDays, setLoanDays] = useState(14);

    const activeItems = book.bookItems?.filter((i: any) => !i.isDeleted) || [];

    const [selectedBarcode, setSelectedBarcode] = useState<string | undefined>(
        book.isDigital ? undefined : activeItems.find(i => String(i.status) === "Available")?.barcode
    );

    const handleConfirm = () => {
        if (!book.isDigital && !selectedBarcode) {
            alert("Choose a book instead!");
            return;
        }

        const selectedItem: LoanItemRequest = {
            bookId: book.id,
            isDigital: book.isDigital,
            barcode: selectedBarcode
        };

        const request: OnlineLoanRequest = {
            bookItemsId: [selectedItem],
            loanDays: loanDays
        };

        onConfirm(request);
    };

    return (
        <div className="modal-overlay">
            <div className="modal-content">
                <h3>Borrow book: {book.title}</h3>

                <div className="form-group">
                    <label>Borrow number:</label>
                    <input
                        type="number"
                        value={loanDays}
                        onChange={(e) => setLoanDays(parseInt(e.target.value) || 0)}
                        min={1}
                        max={30}
                        className="form-control"
                    />
                </div>

                <div className="form-group" style={{ marginTop: '15px' }}>
                    <label>Choose a book item:</label>
                    {book.isDigital ? (
                        <p className="status-available">E-Book (Always available)</p>
                    ) : (
                        <select
                            value={selectedBarcode}
                            onChange={(e) => setSelectedBarcode(e.target.value)}
                            className="form-control"
                        >
                            {!selectedBarcode && <option value="">-- Choose a copy --</option>}
                            {activeItems.map((item: any) => (
                                <option
                                    key={item.id}
                                    value={item.barcode}
                                    // Kiểm tra status bằng chuỗi "Available"
                                    disabled={String(item.status) !== "Available"}
                                >
                                    {item.barcode}
                                    {String(item.status) === "Available" ? ' (Available)' : ' (Borrowed/Broken)'}
                                    {/* Dùng .location vì AutoMapper đã đổi tên ShelfLocation -> Location */}
                                    {item.location ? ` - Shelf: ${item.location}` : ''}
                                </option>
                            ))}
                        </select>
                    )}
                </div>

                <div className="modal-actions" style={{ marginTop: '20px' }}>
                    <button onClick={onClose} className="btn-cancel">Cancel</button>
                    <button
                        onClick={handleConfirm}
                        className="btn-confirm"
                        disabled={!book.isDigital && !selectedBarcode}
                    >
                        Reserve book
                    </button>
                </div>
            </div>
        </div>
    );
};

export default BorrowModal;