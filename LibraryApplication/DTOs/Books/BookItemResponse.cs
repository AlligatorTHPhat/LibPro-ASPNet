using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryApplication.DTOs
{
    public class BookItemResponse
    {
        public Guid Id { get; set; }
        public string Barcode { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;

        public bool IsDeleted { get; set; }
    }
}
