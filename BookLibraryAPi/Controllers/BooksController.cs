using BookLibraryAPi.DB;
using BookLibraryAPi.DTOs;
using BookLibraryAPi.Model;
using BookLibraryAPi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookLibraryAPi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpPost("bookupload")]
        public async Task<IActionResult> AddBookWithPdf(
        [FromForm] BookUploadDto bookDto,
        IFormFile? pdfFile)
        {
            var result = await _bookService.AddBookWithPdfAsync(bookDto, pdfFile);
            return CreatedAtAction(nameof(GetBookById), new { id = result.Id },ApiResponse<BookResponseDto>.SuccessResponse( result, ""));
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(int id)
        {
            var result = await _bookService.GetBookByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(ApiResponse<BookResponseDto>.SuccessResponse(result,""));
        }

        [HttpGet("getAllBooks")]
        public async Task<IActionResult> GetAllBooks()
        {
            var result = await _bookService.GetAllBooksAsync();
            return Ok(ApiResponse<List<BookResponseDto>>.SuccessResponse(result, ""));
        }

        [HttpGet("getAllReview")]
        public async Task<IActionResult> GetAllReviews()
        {
            var result = await _bookService.GetAllReviewsAsync();
            return Ok(ApiResponse<List<ReviewResponseDto>>.SuccessResponse(result, ""));
        }

        [HttpPost("{bookId}/reviews")]
        public async Task<IActionResult> AddReview(int bookId, [FromBody] ReviewRequestDto reviewDto)
        {
            var result = await _bookService.AddReviewAsync(bookId, reviewDto);
            if (result == null) return NotFound(ApiResponse<string>.ErrorResponse("No Review Found!"));
            return Ok(ApiResponse<ReviewResponseDto>.SuccessResponse(result, ""));
        }

        [HttpGet("getAllGenres")]
        public async Task<IActionResult> GetGenres()
        {
            var result = await _bookService.GetAllGenres();
            return Ok(ApiResponse<List<GenreResponseWithBooksDto>>.SuccessResponse(result, ""));
        }

    }
}
