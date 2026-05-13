using LibraryApplication.DTOs.Categories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryApplication.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync();
        Task<CategoryResponse?> GetCategoryByIdAsync(Guid id);
        Task<CategoryResponse> CreateCategoryAsync(CreateCategoryRequest request);
        Task UpdateCategoryAsync(Guid id, UpdateCategoryRequest request);

        Task<int> GetBookCountAsync(Guid categoryId);
    }
}