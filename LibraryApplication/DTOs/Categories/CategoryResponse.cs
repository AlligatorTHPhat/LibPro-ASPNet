using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryApplication.DTOs.Categories
{
    public class CategoryResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int BookCount { get; set; }

        public CategoryResponse() { }

        public CategoryResponse(Guid id, string name, string description, int bookCount)
        {
            Id = id;
            Name = name;
            Description = description;
            BookCount = bookCount;
        }
    }
}
