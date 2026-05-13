namespace LibraryApplication.DTOs.Reservation
{
    public class ReservationResponse
    {
        public Guid Id { get; set; }
        public DateTime ReservedDate { get; set; }
        public DateTime? ReadyDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Status { get; set; } = string.Empty;

        public Guid ReaderId { get; set; }
        public string ReaderName { get; set; } = string.Empty;

        public Guid BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public string? ShelfLocation { get; set; }
    }
}