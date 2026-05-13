/****
Tran Hoang Phat - 49.01.104.107
****/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace LibraryDomain.Entities
{
    public class Supplier
    {
        public Guid Id { get; private set; }
        public required string Name { get; set; }
        public string? TaxCode { get; set; }
        public string? ContactPerson { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? OfficeAddress { get; set; }
        private readonly List<Book> _suppliedBooks = new();
        public virtual ICollection<Book> SuppliedBooks => _suppliedBooks.AsReadOnly();
        private Supplier() { }

        [SetsRequiredMembers]
        public Supplier(string name, string? contactPerson = null, string? email = null)
        {
            Id = Guid.NewGuid();
            Name = name;
            ContactPerson = contactPerson;
            Email = email;
        }

        public void UpdateSupplierDetails(string name, string address, string phone)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name of Supplier cannot be empty!");

            Name = name;
            OfficeAddress = address;
            PhoneNumber = phone;
        }
    }
}
