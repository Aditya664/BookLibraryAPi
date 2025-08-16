using BookLibraryAPi.DTOs;
using BookLibraryAPi.Model;

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
        Task<List<BookResponseDto>> GetBooksByGenreAsync(int genreId);
        Task<FavoriteResponseDto?> AddBookToFavoritesAsync(FavoriteRequestDto request);
        Task<List<FavoriteResponseDto>> GetUserFavoritesAsync(Guid userId);
        Task<ReadingProgressResponseDto?> UpdateReadingProgressAsync(Guid userId, ReadingProgressRequestDto request);
        Task<ReadingProgressResponseDto> GetReadingProgressAsync(Guid userId, int bookId);
        Task<ReadingProgressResponseDto> GetLastReadingProgressAsync(Guid userId);
        Task<bool> DeleteBookAsync(int bookId);
        Task<BookResponseDto?> UpdateBookAsync(int bookId, BookUploadDto bookDto, IFormFile? pdfFile);
        Task<GenreResponseDto> AddGenreAsync(GenreRequestDto genreDto);
        Task<GenreResponseDto?> UpdateGenreAsync(int id, GenreRequestDto genreDto);
        Task<bool> DeleteGenreAsync(int id);
        Task<ReadingProgress> StartReadingAsync(Guid userId, int bookId);
        Task<bool> EndReadingAsync(Guid userId, int bookId, int sessionMinutes);
        Task<bool> HasReadingTimeLeftAsync(Guid userId);

    }
}
