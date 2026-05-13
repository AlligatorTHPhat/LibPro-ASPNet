using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace LibraryDomain.Entities
{
    public class Publisher
    {
        public Guid Id { get; private set; }

        public required string Name { get; set; }

        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        public string? OfficeAddress { get; set; } 

        private readonly List<Book> _books = new();
        public virtual ICollection<Book> Books => _books.AsReadOnly();

        private Publisher() { }

        [SetsRequiredMembers]
        public Publisher(string name, string? email = null)
        {
            Id = Guid.NewGuid();
            Name = name;
            Email = email;
        }

        public void UpdateContactInfo(string email, string phone)
        {
            Email = email;
            PhoneNumber = phone;
        }
    }
}
