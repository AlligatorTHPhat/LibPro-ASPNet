import React, { useEffect, useState } from 'react';
import { bookApi } from '../../api/bookApi';
import { loanApi } from '../../api/loanApi';
import { BookResponse } from '../../types/book';
import BookCard from '././BookCard';
import BorrowModal from '././BorrowModal';
import { reservationApi } from '../../api/reservationApi';

const BookGallery = () => {
    const [books, setBooks] = useState<BookResponse[]>([]);
    const [selectedBook, setSelectedBook] = useState<BookResponse | null>(null);

    useEffect(() => {
        loadBooks();
    }, []);

    const loadBooks = async () => {
        const res = await bookApi.getAll();
        const visibleBooks = res.data.filter((b: BookResponse) => !b.isDeleted);
        setBooks(visibleBooks);
    };

    const handleBorrowSubmit = async (request: any) => {
        console.log("Data load:", request.bookItemsId[0]);
        try {
            //await loanApi.createOnlineLoan(request);
            await reservationApi.reserveBook(
                {
                    bookId: request.bookItemsId[0].bookId,
                    barcode: request.bookItemsId[0].barcode
                }
            )
            alert("Borrow book successfully! Get it in right shelf-location");
            setSelectedBook(null);
            loadBooks();
        } catch (error: any) {
            alert(error.response?.data || "Error while borrow book!");
        }
    };

    return (
        <div className="book-gallery-container">
            <div className="grid-layout">
                {books.map(book => (
                    <BookCard
                        key={book.id}
                        book={book}
                        onBorrow={(b) => setSelectedBook(b)}
                    />
                ))}
            </div>

            {selectedBook && (
                <BorrowModal
                    book={selectedBook}
                    onClose={() => setSelectedBook(null)}
                    onConfirm={handleBorrowSubmit}
                />
            )}
        </div>
    );
};

export default BookGallery;