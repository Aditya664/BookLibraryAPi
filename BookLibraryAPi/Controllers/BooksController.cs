using BookLibraryAPi.DB;
using BookLibraryAPi.DTOs;
using BookLibraryAPi.Model;
using BookLibraryAPi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BookLibraryAPi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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

        [HttpPost("toggleFavorites")]
        public async Task<IActionResult> AddBookToFavorites([FromBody] FavoriteRequestDto request)
        {
            var result = await _bookService.ToggleFavoriteAsync(request);
            if (result == null)
                return Ok(ApiResponse<FavoriteResponseDto>.SuccessResponse(result, "Book already in favorites"));

            return Ok(ApiResponse<FavoriteResponseDto>.SuccessResponse(result, "Book added to favorites"));
        }

        [HttpPost("checkFavorites")]
        public async Task<IActionResult> GetBookFavorite([FromBody] FavoriteRequestDto request)
        {
            var result = await _bookService.CheckFavoriteOrNot(request);
            if (result == null)
                return BadRequest(ApiResponse<string>.ErrorResponse("Book not in favorites"));

            return Ok(ApiResponse<bool?>.SuccessResponse(result, "Book is favorite"));
        }


        [HttpGet("user/{userId}/favorites")]
        public async Task<IActionResult> GetUserFavorites(Guid userId)
        {
            var result = await _bookService.GetUserFavoritesAsync(userId);
            return Ok(ApiResponse<List<FavoriteResponseDto>>.SuccessResponse(result, ""));
        }



        [HttpPut("user/{userId}/reading-progress")]
        public async Task<IActionResult> UpdateReadingProgress(Guid userId, [FromBody] ReadingProgressRequestDto request)
        {
            var result = await _bookService.UpdateReadingProgressAsync(userId, request);
            return Ok(ApiResponse<ReadingProgressResponseDto>.SuccessResponse(result, "Reading progress updated"));
        }

        [HttpGet("user/{userId}/reading-progress")]
        public async Task<IActionResult> GetLastReadingProgressAsync(Guid userId)
        {
            var result = await _bookService.GetLastReadingProgressAsync(userId);
            if (result == null)
                return NotFound(ApiResponse<string>.ErrorResponse("No reading progress found for this book"));

            return Ok(ApiResponse<ReadingProgressResponseDto>.SuccessResponse(result, ""));
        }

        [HttpGet("user/{userId}/reading-progress/{bookId}")]
        public async Task<IActionResult> GetReadingProgress(Guid userId, int bookId)
        {
            var result = await _bookService.GetReadingProgressAsync(userId, bookId);
            if (result == null)
                return NotFound(ApiResponse<string>.ErrorResponse("No reading progress found for this book"));

            return Ok(ApiResponse<ReadingProgressResponseDto>.SuccessResponse(result, ""));
        }

        [HttpPut("updateBook/{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromForm] BookUploadDto bookDto, IFormFile? pdfFile)
        {
            var updatedBook = await _bookService.UpdateBookAsync(id, bookDto, pdfFile);
            if (updatedBook == null) return NotFound();
            return Ok(ApiResponse<BookResponseDto>.SuccessResponse(updatedBook, ""));
        }

        // Delete a book
        [HttpDelete("deleteBook/{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var deleted = await _bookService.DeleteBookAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> AddGenre([FromBody] GenreRequestDto dto) =>
       Ok(ApiResponse<GenreResponseDto>.SuccessResponse(await _bookService.AddGenreAsync(dto), ""));

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGenre(int id, [FromBody] GenreRequestDto dto)
        {
            var updated = await _bookService.UpdateGenreAsync(id, dto);
            return updated == null ? NotFound(ApiResponse<string>.ErrorResponse("No Genre Found!")) : Ok(ApiResponse<GenreResponseDto>.SuccessResponse(updated, ""));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            var deleted = await _bookService.DeleteGenreAsync(id);
            return deleted ? NoContent() : NotFound(ApiResponse<string>.ErrorResponse("No Genre Found!"));
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartReading(Guid userId, int bookId)
        {
            var progress = await _bookService.StartReadingAsync(userId, bookId);
            return Ok(ApiResponse<ReadingProgress>.SuccessResponse(progress, "Reading Started"));
        }

        // On book close/destroy (pass session duration)
        [HttpPost("end")]
        public async Task<IActionResult> EndReading(Guid userId, int bookId, int sessionMinutes)
        {
            var hasTimeLeft = await _bookService.EndReadingAsync(userId, bookId, sessionMinutes);

            if (!hasTimeLeft)
                return Forbid("Your free 50 hours have expired. Please subscribe.");

            return Ok(ApiResponse<bool>.SuccessResponse(hasTimeLeft, "Progress saved"));
        }

        [HttpGet("user/{userId}/reading-history")]
        public async Task<IActionResult> GetReadingHistory(Guid userId)
        {
            var history = await _bookService.GetReadingHistoryAsync(userId);
            return Ok(ApiResponse<List<ReadingProgressResponseDto>>.SuccessResponse(history, ""));
        }

        // To check if user still has time left
        [HttpGet("check")]
        public async Task<IActionResult> CheckTimeLeft(Guid userId)
        {
            var hasTimeLeft = await _bookService.HasReadingTimeLeftAsync(userId);
            return Ok(new { hasTimeLeft });
        }
    }
}
