namespace LibraryApplication.DTOs
{
    public class BookItemRequest
    {
        public Guid BookId { get; set; }
        public string ShelfLocation { get; set; } = "???";

        // Optional: If you want to add multiple copies at once
        public int Quantity { get; set; } = 1;
    }
}