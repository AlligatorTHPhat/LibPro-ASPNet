using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace LibraryApplication.DTOs
{
    public class BookResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Isbn { get; set; } = string.Empty;
        //public IFormFile? ImageFile { get; set; }
        public string? CoverImageUrl { get; set; } // Thêm dòng này để chứa link ảnh[cite: 12]
        public string CategoryName { get; set; } = string.Empty;
        public string PublisherName { get; set; } = string.Empty;

        //public string Author { get; set; } = string.Empty;
        //public int PublishYear { get; set; }
        public string Language { get; set; } = string.Empty;

        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public bool IsDigital { get; set; }

        public bool IsDeleted { get; set; }
        public string StatusDescription => IsDigital ? "E-Book (Sẵn sàng)" : $"{AvailableCopies}/{TotalCopies} bản vật lý";

        // Danh sách các ID của BookItems thuộc đầu sách này (nếu cần truy xuất chi tiết)
        public List<BookItemResponse> BookItems { get; set; } = new List<BookItemResponse>();
    }
}