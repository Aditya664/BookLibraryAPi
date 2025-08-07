using BookLibraryAPi.DTOs;

namespace BookLibraryAPi.Services
{
    public interface IBookService
    {
        Task<BookResponseDto> AddBookWithPdfAsync(BookUploadDto bookDto, IFormFile? pdfFile);
        Task<BookResponseDto?> GetBookByIdAsync(int id);
        Task<List<BookResponseDto>> GetAllBooksAsync();
        Task<List<GenreResponseWithBooksDto>> GetAllGenres();
        Task<List<ReviewResponseDto>> GetAllReviewsAsync();
        Task<ReviewResponseDto?> AddReviewAsync(int bookId, ReviewRequestDto reviewDto);
    }
}
