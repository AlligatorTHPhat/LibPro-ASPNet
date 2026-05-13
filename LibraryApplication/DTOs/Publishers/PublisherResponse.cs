namespace LibraryApplication.DTOs
{
    public class PublisherCreateDto
    {
        public required string Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? OfficeAddress { get; set; }
    }

    public class PublisherUpdateDto : PublisherCreateDto
    {
        public Guid Id { get; set; }
    }
}