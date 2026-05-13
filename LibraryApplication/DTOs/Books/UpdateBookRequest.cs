using Microsoft.AspNetCore.Http;
using System;

namespace LibraryApplication.DTOs
{
    public class UpdateBookRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public IFormFile? ImageFile { get; set; }
        public string? Description { get; set; }
        //public string Language { get; set; } = string.Empty;
        //public int PublishYear { get; set; }

        public Guid CategoryId { get; set; }
        public Guid PublisherId { get; set; }
        public Guid SupplierId { get; set; }

        public bool IsDigital { get; set; }
        public string? DigitalUrl { get; set; }
    }
}
