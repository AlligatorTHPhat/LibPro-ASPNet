using LibraryApplication.DTOs;

namespace LibraryApplication.Interfaces
{
    public interface ISupplierService
    {
        Task <SupplierCreateDto> CreateAsync(SupplierCreateDto dto);
        Task <SupplierUpdateDto> UpdateAsync(SupplierUpdateDto dto);
    }
}