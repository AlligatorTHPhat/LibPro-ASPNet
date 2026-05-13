import React from 'react';
import { ReservationStatus, ReservationStatusLabels } from '../../types/reservation';

interface Props {
    status: string | number;
}

const ReservationStatusBadge: React.FC<Props> = ({ status }) => {
    const s = status.toString();

    let statusClass = "status-pending";

    if (s === "2" || s === "Ready") {
        statusClass = "status-ready";
    } else if (s === "3" || s === "Completed") {
        statusClass = "status-completed";
    } else if (s === "4" || s === "Canceled") {
        statusClass = "status-canceled";
    } else if (s === "5" || s === "Expired") {
        statusClass = "status-overdue";
    }

    const label = ReservationStatusLabels[status as unknown as ReservationStatus] || s;

    return (
        <span className={`status-tag ${statusClass}`}>
            {label}
        </span>
    );
};

export default ReservationStatusBadge;