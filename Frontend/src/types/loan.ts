export interface LoanItemRequest {
    bookId: string;
    barcode?: string;
    isDigital: boolean;
}

export interface OnlineLoanRequest {
    bookItemsId: LoanItemRequest[];
    loanDays: number;
}