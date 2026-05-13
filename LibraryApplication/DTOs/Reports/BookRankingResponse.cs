using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryApplication.DTOs.Reports
{
    public class BookRankingResponse
    {
        public Guid BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int BorrowCount { get; set; }
        public int CurrentAvailableCopies { get; set; }
    }
}
