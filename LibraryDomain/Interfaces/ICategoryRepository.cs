using LibraryDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LibraryDomain.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category?> GetByIdAsync(Guid id);

        Task<Category?> GetByNameAsync(string name);

        Task<Category?> FindAsync(Expression<Func<Category, bool>> predicate);

        Task<IEnumerable<Category>> GetAllAsync();

        Task AddAsync(Category category);
        void Update(Category category);

        //void Delete(Category category);
        Task<int> CountBooksInCategoryAsync(Guid categoryId);

        Task<bool> ExistsByNameAsync(string name);
    }
}