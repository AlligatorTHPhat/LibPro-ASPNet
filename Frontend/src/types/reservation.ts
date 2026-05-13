export interface ReserveBookRequest {
    bookId: string;
    barcode: string; 
}

export interface ReservationResponse {
    id: string;
    reservedDate: string;
    readyDate?: string;
    expiryDate?: string;
    status: string;
    readerId: string;
    readerName: string;
    bookId: string;
    bookTitle: string;
    barcode: string;
    shelfLocation?: string;
}

export enum ReservationStatus {
    Pending = 1,
    Ready = 2,
    Completed = 3,
    Canceled = 4,
    Expired = 5
}

export const ReservationStatusLabels: Record<ReservationStatus, string> = {
    [ReservationStatus.Pending]: "Pending",
    [ReservationStatus.Ready]: "Ready to Pickup",
    [ReservationStatus.Completed]: "Completed",
    [ReservationStatus.Canceled]: "Canceled",
    [ReservationStatus.Expired]: "Expired"
};