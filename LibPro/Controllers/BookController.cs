using Azure.Core;
using LibraryApplication.DTOs;
using LibraryApplication.DTOs.Books;
using LibraryApplication.Interfaces;
using LibraryDomain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibPro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var books = await _bookService.GetAllBooksAsync();
            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null) return NotFound("Cannot find Book.");
            return Ok(book);
        }

        [HttpGet("title/{title}")]
        public async Task<IActionResult> GetbyTitle(string title)
        {
            var books = await _bookService.SearchBookByTitleAsync(title);
            return Ok(books);
        }

        [HttpGet("author/{author}")]
        public async Task<IActionResult> GetbyAuthor(string author)
        {
            var books = await _bookService.SearchBookByAuthorAsync(author);
            return Ok(books);
        }

        [HttpPost]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> CreateBook([FromForm] CreateBookRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _bookService.CreateBookAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> UpdateBook(Guid id, [FromForm] UpdateBookRequest request)
        {
            await _bookService.UpdateBookAsync(id, request);
            return Ok("Update Book Successfully.");
        }

        [HttpGet("{id}/details-librarian")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> GetDetailsForLibrarian(Guid id)
        {
            var book = await _bookService.GetBookDetailsWithDeletedItemsAsync(id);
            if (book == null) return NotFound();
            return Ok(book);
        }

        [HttpPost("items")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> AddBookItem([FromBody] BookItemRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _bookService.AddBookItemAsync(request);
            return Ok("Book Item added successfully.");
        }

        [Authorize(Roles = "Librarian")]
        [HttpPut("items/{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateBookStatusRequest request)
        {
            await _bookService.UpdateBookItemStatusAsync(id, (LoanStatus)request.Status, (BookStatus)request.Status);
            return Ok(new { Message = "Book status updated successfully." });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> DeleteBookItem(Guid id)
        {
            await _bookService.DeleteBookItemAsync(id);
            return Ok("Delete Book Item Successfully.");
        }

        [HttpPut("restore/{id}")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> RestoreBookItem(Guid id)
        {
            await _bookService.RestoreBookItemAsync(id);
            return Ok("Restored Book Item Successfully.");
        }
    }
}