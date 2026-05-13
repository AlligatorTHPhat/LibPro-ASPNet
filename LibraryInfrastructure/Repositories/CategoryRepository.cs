using LibraryDomain.Entities;
using LibraryDomain.Interfaces;
using LibraryInfrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LibraryInfrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly LibraryDbContext _context;

        public CategoryRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<Category?> GetByIdAsync(Guid id) => await _context.Categories.FindAsync(id);

        public async Task<Category?> GetByNameAsync(string name) => await _context.Categories.FindAsync($"{name}");

        public async Task<IEnumerable<Category>> GetAllAsync() => await _context.Categories.ToListAsync();

        public async Task<Category?> FindAsync(Expression<Func<Category, bool>> predicate)
        {
            return await _context.Categories.FirstOrDefaultAsync(predicate);
        }

        public async Task AddAsync(Category category) => await _context.Categories.AddAsync(category);

        public void Update(Category category) => _context.Categories.Update(category);

        public async Task<int> CountBooksInCategoryAsync(Guid categoryId)
        {
            return await _context.Books.CountAsync(b => b.CategoryId == categoryId);
        }

        public async Task<bool> ExistsByNameAsync(string name) => await _context.Categories.AnyAsync(c => c.Name == name);
    }
}