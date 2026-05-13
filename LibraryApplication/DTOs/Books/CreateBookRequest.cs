using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace LibraryApplication.DTOs
{
    public class CreateBookRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Isbn { get; set; } = string.Empty;
        public IFormFile? ImageFile { get; set; }
        public Guid CategoryId { get; set; }
        public Guid PublisherId { get; set; }
        public Guid SupplierId { get; set; }
        public string Author { get; set; } = string.Empty;
        //public int PublishYear { get; set; }
        //public string Language { get; set; } = string.Empty;

        public int InitialCopies { get; set; } = 0;
        public string DefaultShelf { get; set; } = "Khu vực chờ";
        public string? Description { get; set; }
        public bool IsDigital { get; set; }
        public string? DigitalUrl { get; set; }
    }
}