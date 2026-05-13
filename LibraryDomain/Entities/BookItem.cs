using LibraryDomain.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace LibraryDomain.Entities
{
    public class BookItem
    {
        public Guid Id { get; private set; }
        public required string Barcode { get; set; }
        public string? ShelfLocation { get; set; }
        public BookStatus Status { get; private set; }

        public Guid BookId { get; private set; }
        public bool IsDeleted { get; private set; }

        public virtual Book Book { get; private set; } = null!;

        private BookItem() { }

        [SetsRequiredMembers]
        public BookItem(string barcode, string shelfLocation, Guid bookId,bool isDeleted)
        {
            Id = Guid.NewGuid();
            Barcode = barcode;
            ShelfLocation = shelfLocation;
            BookId = bookId;
            IsDeleted = isDeleted;
            Status = BookStatus.Available; 
        }
        public void MarkAsAvailable() => Status = BookStatus.Available;
        public void MarkAsLoaned() => Status = BookStatus.Loaned;
        public void MarkAsReserved() => Status = BookStatus.Reserved;
        public void MarkAsInRepair() => Status = BookStatus.InRepair;
        public void MarkAsLost() => Status = BookStatus.Lost;
        public void MarkAsUnavaiable() => Status = BookStatus.Discarded;
        public void Create(string barcode)
        {
            Barcode = barcode;
            IsDeleted = false;
            Status = BookStatus.Available;
        }
        public void Delete()
        {
            IsDeleted = true;
            Status = BookStatus.Discarded;
        }
        public void Restore()
        {
            IsDeleted = false;
            Status = BookStatus.Available;
        }
    }
}
