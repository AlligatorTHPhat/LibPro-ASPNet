using AutoMapper;
using LibraryApplication.DTOs;
using LibraryApplication.Interfaces;
using LibraryDomain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibPro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Librarian")]
    public class SuppliersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISupplierService _supplierService;
        private readonly IMapper _mapper;
        public SuppliersController(IUnitOfWork unitOfWork, ISupplierService supplierService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _supplierService = supplierService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var suppliers = await _unitOfWork.Suppliers.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<SupplierUpdateDto>>(suppliers));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SupplierCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _supplierService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] SupplierUpdateDto dto)
        {
            if (id != dto.Id) return BadRequest("ID not found!");

            try
            {
                var result = await _supplierService.UpdateAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
