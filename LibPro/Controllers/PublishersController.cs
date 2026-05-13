using AutoMapper;
using LibraryApplication.DTOs;
using LibraryDomain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibPro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Librarian")]
    public class PublishersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPublisherService  _publisherService;
        public PublishersController(IUnitOfWork unitOfWork, IPublisherService publisherService, IMapper mapper)
        { 
            _unitOfWork = unitOfWork;
            _publisherService = publisherService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var publishers = await _unitOfWork.Publishers.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<PublisherUpdateDto>>(publishers));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PublisherCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _publisherService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] PublisherUpdateDto dto)
        {
            if (id != dto.Id) return BadRequest("ID not found!");

            try
            {
                var result = await _publisherService.UpdateAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
