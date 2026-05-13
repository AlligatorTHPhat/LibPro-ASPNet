export interface BookStatusDto {
    status: string;
    count: number;
}

export interface LoanStatusDto {
    status: string;
    count: number;
}

export interface DashboardResponse {
    totalBooks: number;
    totalReaders: number;
    activeLoans: number;
    overdueLoans: number;
    totalRevenue: number;
    bookDistribution: BookStatusDto[];
    loanDistribution: LoanStatusDto[];
}