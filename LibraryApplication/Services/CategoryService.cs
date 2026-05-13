using AutoMapper;
using LibraryApplication.DTOs.Categories;
using LibraryApplication.Interfaces;
using LibraryDomain.Entities;
using LibraryDomain.Exceptions;
using LibraryDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryApplication.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync() {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            var responseList = new List<CategoryResponse>();

            foreach (var category in categories)
            {
                var bookCount = await _unitOfWork.Categories.CountBooksInCategoryAsync(category.Id);

                var dto = new CategoryResponse(category.Id, category.Name, category.Description,bookCount);
                responseList.Add(dto);
            }

            return responseList;
        }

        public async Task<CategoryResponse?> GetCategoryByIdAsync(Guid id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null) throw new EntityNotFoundException("Category", id);

            var bookCount = await _unitOfWork.Categories.CountBooksInCategoryAsync(id);

            return new CategoryResponse(
                category.Id,
                category.Name,
                category.Description,
                bookCount
            );
        }

        public async Task<CategoryResponse> CreateCategoryAsync(CreateCategoryRequest request)
        {
            var existing = await _unitOfWork.Categories.FindAsync(c => c.Name == request.Name);
            if (existing != null) throw new EntityAlreadyExistsException("Category", request.Name);

            var category = new Category(request.Name, request.Description);

            if (request.ParentCategoryId is not null && request.ParentCategoryId != Guid.Empty)
            {
                category.ParentCategoryId = request.ParentCategoryId;
            }

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<CategoryResponse>(category);
            response.BookCount = 0;
            return response;
        }

        public async Task UpdateCategoryAsync(Guid id, UpdateCategoryRequest request)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("Category", id);

            var duplicate = await _unitOfWork.Categories.FindAsync(c => c.Name == request.Name && c.Id != id);
            if (duplicate != null) throw new EntityAlreadyExistsException("Category", request.Name);

            category.UpdateCategory(request.Name, request.Description);

            _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> GetBookCountAsync(Guid categoryId)
        {
            var exists = await _unitOfWork.Categories.GetByIdAsync(categoryId);
            if (exists == null) throw new EntityNotFoundException("Category", categoryId);

            return await _unitOfWork.Categories.CountBooksInCategoryAsync(categoryId);
        }
    }
}