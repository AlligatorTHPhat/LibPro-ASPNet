using LibraryDomain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryApplication.DTOs.Books
{
    public class UpdateBookStatusRequest
    {
        public BookStatus Status { get; set; }
    }
}
