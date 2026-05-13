using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using LibraryDomain.Enums;
using LibraryDomain.ValueObjects;

namespace LibraryDomain.Entities
{
    public class Book
    {
        public Guid Id { get; private set; }
        public required string Title { get; set; }
        public Isbn Isbn { get; set; }
        //new
        public string? CoverImageUrl { get; set; }
        public string? Description { get; set; }
        public string? Author { get; set; }

        public bool IsDigital { get; set; }
        public string? DigitalFilePath { get; private set; }
        public DateTime? LicenseExpiryDate { get; private set; }

        public Guid? PublisherId { get; private set; }
        public virtual Publisher Publisher { get; private set; } = null!;

        public Guid? SupplierId { get; private set; }
        public virtual Supplier Supplier { get; private set; } = null!;

        public Guid CategoryId { get; private set; }
        public virtual Category Category { get; private set; } = null!;
        public BookStatus Status { get; set; }

        public bool IsDeleted { get; private set; }

        private readonly List<BookItem> _bookItems = new();
        public virtual ICollection<BookItem> BookItems => _bookItems.AsReadOnly();

        private Book() { }

        [SetsRequiredMembers]
        public Book(string title, Isbn isbn, Publisher publisher, Supplier supplier, Category category, bool isDeleted = false)
        {
            Id = Guid.NewGuid();
            Title = title;
            Isbn = isbn;
            Publisher = publisher;
            PublisherId = publisher.Id;
            Supplier = supplier;
            SupplierId = supplier.Id;
            Category = category;
            CategoryId = category.Id;
            IsDeleted = isDeleted;
            Status = BookStatus.Available;
        }

        public void MarkAsDigital(string filePath, DateTime expiry)
        {
            IsDigital = true;
            DigitalFilePath = filePath;
            LicenseExpiryDate = expiry;
            //new
            if (!_bookItems.Any())
            {
                AddBookItem($"DIGITAL-{Isbn.Value}", "Cloud Storage");
            }
        }

        public void AddBookItem(string barcode, string shelfLocation)
        {
            var newItem = new BookItem(barcode, shelfLocation, this.Id, false);
            _bookItems.Add(newItem);
        }

        public void SoftDelete()
        {
            IsDeleted = true;

            foreach (var item in _bookItems)
            {
                item.MarkAsUnavaiable();
            }
        }

        public void Restore()
        {
            IsDeleted = false;

            foreach (var item in _bookItems)
            {
                item.MarkAsAvailable();
            }
        }
    }
}
