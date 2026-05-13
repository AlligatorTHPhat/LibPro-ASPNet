using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryApplication.DTOs.Reservevations
{
    public class ReserveBookRequest
    {
        public Guid? ReaderId { get; set; }
        public Guid BookId { get; set; }
        public string Barcode { get; set; } = string.Empty;
    }
}
