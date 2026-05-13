using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace LibraryDomain.Entities
{
    public class Category
    {
        public Guid Id { get; private set; }

        public required string Name { get; set; }

        public string? Description { get; set; }

        public Guid? ParentCategoryId { get; set; } 
        public virtual Category? ParentCategory { get; set; }


        private readonly List<Book> _books = new();
        public virtual ICollection<Book> Books => _books.AsReadOnly();

        private Category() { }

        [SetsRequiredMembers]
        public Category(string name, string? description = null, Guid? parentCategoryId = null)
        {
            Id = Guid.NewGuid();
            Name = name;
            Description = description;
            ParentCategoryId = parentCategoryId;
        }

        public void UpdateCategory(string name, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name cannot be empty!");

            Name = name;
            Description = description;
        }
    }
}
