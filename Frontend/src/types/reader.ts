export interface ReaderResponse {
    id: string;
    libraryCardNumber: string;
    fullName: string;
    gender: number;
    dateOfBirth: string;
    address: string;
    phoneNumber: string;
    expiryDate: string;
    isDeleted: boolean;
    accountId: string;
    readerTypeName: string;
}

export interface CreateReaderRequest {
    accountId: string;
    libraryCardNumber: string;
    fullName: string;
    gender: number;
    dateOfBirth: string;
    ward: string;
    street: string;
    city: string;
    district: string;
    phoneNumber: string;
    readerTypeName: string;
}

export interface UpdateReaderRequest {
    fullName: string;
    ward: string;
    street: string;
    district: string;
    city: string;
    phoneNumber: string;
    isDeleted: boolean;
}