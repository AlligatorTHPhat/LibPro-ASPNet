using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryApplication.DTOs.Categories
{
    public record UpdateCategoryRequest(
        string Name,
        string? Description,
        Guid? ParentCategoryId
    );
}