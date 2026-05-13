export interface BookItemResponse {
    id: string;
    barcode: string;
    status: number; // Mapping từ Enum BookStatus
    currentShelf: string;
}

export interface BookResponse {
    id: string;
    title: string;
    isbn: string;
    coverImageUrl?: string;
    categoryName: string;
    publisherName: string;
    language: string;
    totalCopies: number;
    availableCopies: number;
    isDigital: boolean;
    isDeleted: boolean;
    statusDescription: string;
    bookItems: BookItemResponse[];
}