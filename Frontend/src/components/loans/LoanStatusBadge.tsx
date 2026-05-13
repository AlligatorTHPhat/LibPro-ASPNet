import React from 'react';

interface Props {
    status: number;
    description: string;
}

const LoanStatusBadge: React.FC<Props> = ({ status, description }) => {
    const getStatusClass = (status: number) => {
        switch (status) {
            case 1: return 'status-active';    // Active
            case 2: return 'status-returned';  // Returned
            case 3: return 'status-overdue';   // Overdue
            case 4: return 'status-pending';   // PendingFine
            case 6: return 'status-lost';      // Lost
            default: return 'status-default';
        }
    };

    return (
        <span className={`loan-status-badge ${getStatusClass(status)}`}>
            {status === 0 && "⏳ "}
            {status === 1 && "📖 "}
            {status === 3 && "✅ "}
            {status === 4 && "⚠️ "}
            {description}
        </span>
    );
};

export default LoanStatusBadge;