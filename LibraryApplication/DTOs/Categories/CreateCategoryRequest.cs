using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryApplication.DTOs.Categories
{
    public class CreateCategoryRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid? ParentCategoryId { get; set; } = null;
    }
}
