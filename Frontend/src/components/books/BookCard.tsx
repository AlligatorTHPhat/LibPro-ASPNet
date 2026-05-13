import React from 'react';
import { bookApi } from '../../api/bookApi';
import { BookResponse } from '../../types/book';

interface Props {
    book: BookResponse;
    onBorrow: (book: BookResponse) => void;
}

const BookCard: React.FC<Props> = ({ book, onBorrow }) => {
    if (book.isDeleted) return null;

    const getImageUrl = (url: string | undefined) => {
        if (!url) return 'https://placehold.co/40x60?text=No+Cover';
        if (url.startsWith('http')) return url;

        const baseUrl = import.meta.env.VITE_API_URL.replace(/\/$/, '');
        const cleanPath = url.startsWith('/') ? url : `/${url}`;

        return `${baseUrl}${cleanPath}`;
    };

    return (
        <div className="book-card">
            <img
                src={getImageUrl(book.coverImageUrl)}
                style={{ width: '40px', height: '60px', objectFit: 'cover', borderRadius: '4px' }}
                alt={book.title}
                onError={(e) => {
                    (e.target as HTMLImageElement).src = 'https://placehold.co/40x60?text=Error';
                }}
            />
            <h3>{book.title}</h3>
            <p>Category: {book.categoryName}</p>


            <div className="status-tag">
                {book.isDigital ? (
                    <span className="text-green-500">E-Book ready</span>
                ) : (
                    <span>Remaining: {book.availableCopies}/{book.totalCopies}</span>
                )}
            </div>

            <button
                disabled={!book.isDigital && book.availableCopies === 0}
                onClick={() => onBorrow(book)}
                className="borrow-button"
            >
                {book.isDigital || book.availableCopies > 0 ? "Borrow now" : "Book runout"}
            </button>
        </div>
    );
};

export default BookCard;