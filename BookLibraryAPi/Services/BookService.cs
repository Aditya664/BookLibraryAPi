using BookLibraryAPi.DB;
using BookLibraryAPi.DTOs;
using BookLibraryAPi.Model;
using Microsoft.EntityFrameworkCore;
using System.Net;
using static System.Reflection.Metadata.BlobBuilder;

namespace BookLibraryAPi.Services
{
    public class BookService : IBookService
    {
        private readonly AppDbContext _context;

        public BookService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<BookResponseDto> AddBookWithPdfAsync(BookUploadDto bookDto, IFormFile? pdfFile)
        {
            var existingGenres = await _context.Genres
                .Where(g => bookDto.GenreIds.Contains(g.Id))
                .ToListAsync();

            byte[]? pdfBytes = null;
            if (pdfFile != null && pdfFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await pdfFile.CopyToAsync(ms);
                pdfBytes = ms.ToArray();
            }

            var book = new Book
            {
                Title = bookDto.Title,
                Author = bookDto.Author,
                Description = bookDto.Description,
                Language = bookDto.Language,
                Image = bookDto.Image,
                Rating = (bookDto.Rating),
                Genres = existingGenres,
                CreatedAt = DateTime.UtcNow,
                PdfFile = pdfBytes,
                PdfFileName = pdfFile?.FileName
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return new BookResponseDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Rating = book.Rating,
                Description = book.Description,
                Language = bookDto.Language,
                Image = book.Image,
                Genres = book.Genres.Select(g => new GenreResponseDto
                {
                    Id = g.Id,
                    Name = g.Name,
                }).ToList(),
                Reviews = book.Reviews.Select(g => new ReviewResponseDto
                {
                    Id = g.Id,
                    BookId = g.BookId,
                    Comment = g.Comment,
                    User    = g.User,
                }).ToList(),
                CreatedAt= book.CreatedAt,
                PdfFile = book.PdfFile,
                PdfFileName = book.PdfFileName
            };
        }
        public async Task<BookResponseDto?> GetBookByIdAsync(int id)
        {
            var book = await _context.Books
                .Include(b => b.Reviews)
                .Include(b => b.Genres)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null) return null;

            return new BookResponseDto
            {
                Id = book.Id,
                Image = book.Image,
                Title = book.Title,
                Author = book.Author,
                Rating = book.Rating,
                Language = book.Language,
                Description = book.Description,
                Genres = book.Genres.Select(g => new GenreResponseDto
                {
                    Id = g.Id,
                    Name = g.Name,
                }).ToList(),
                Reviews = book.Reviews.Select(r => new ReviewResponseDto
                {
                    User = r.User,
                    Comment = r.Comment
                }).ToList(),
                CreatedAt = book.CreatedAt,
                PdfFile = book.PdfFile,
                PdfFileName = book.PdfFileName
            };
        }

        public async Task<List<GenreResponseWithBooksDto>> GetAllGenres()
        {
            var genres = await _context.Genres
                .Include(g => g.Books) 
                .ToListAsync();

            return genres.Select(g => new GenreResponseWithBooksDto
            {
                Id = g.Id,
                Name = g.Name,
                IconName = g.IconName,
                Books = g.Books.Select(b => new GenreBookResponseDto
                {
                    Id = b.Id,
                    BookName = b.Title
                }).ToList()
            }).ToList();
        }

        public async Task<List<BookResponseDto>> GetBooksByGenreAsync(int genreId)
        {
            var books = await _context.Books
                .Include(b => b.Genres)
                .Include(b => b.Reviews)
                    .ThenInclude(r => r.User)
                .Where(b => b.Genres.Any(g => g.Id == genreId))
                .ToListAsync();

            return books.Select(b => MapBookToDto(b)).ToList();
        }


        public async Task<List<BookResponseDto>> GetAllBooksAsync()
        {
            var books = await _context.Books
                .Include(b => b.Genres) 
                .Include(b => b.Reviews)
                .ToListAsync();
            return books.Select(b => new BookResponseDto
            {
                Id = b.Id,
                Image = b.Image,
                Title = b.Title,
                Language = b.Language,
                Author = b.Author,
                Rating = b.Rating,
                Description = b.Description,
                Genres = b.Genres.Select(g => new GenreResponseDto
                {
                    Id=g.Id,
                    Name = g.Name,
                }).ToList(),
                Reviews = b.Reviews.Select(r => new ReviewResponseDto
                {
                    User = r.User,
                    Comment = r.Comment
                }).ToList(),
                CreatedAt = b.CreatedAt,
                PdfFile = b.PdfFile,
                PdfFileName = b.PdfFileName
            }).ToList();
        }

        public async Task<List<ReviewResponseDto>> GetAllReviewsAsync()
        {
            return await _context.Reviews
                .Select(r => new ReviewResponseDto
                {
                    Id = r.Id,
                    User = r.User,
                    Comment = r.Comment,
                    BookId = r.BookId
                })
                .ToListAsync();
        }

        public async Task<ReviewResponseDto?> AddReviewAsync(int bookId, ReviewRequestDto reviewDto)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null) return null;

            var review = new Review
            {
                User = reviewDto.User,
                Comment = reviewDto.Comment,
                BookId = bookId
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return new ReviewResponseDto
            {
                Id = review.Id,
                User = review.User,
                Comment = review.Comment,
                BookId = review.BookId
            };
        }

        public async Task<FavoriteResponseDto?> AddBookToFavoritesAsync(FavoriteRequestDto request)
        {
            var exists = await _context.Favorites
                .AnyAsync(f => f.UserId == request.UserId && f.BookId == request.BookId);

            if (exists) return null;

            var favorite = new Favorite
            {
                UserId = request.UserId,
                BookId = request.BookId
            };

            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();

            var book = await _context.Books
                .Include(b => b.Genres)
                .Include(b => b.Reviews)
                .FirstOrDefaultAsync(b => b.Id == request.BookId);

            return new FavoriteResponseDto
            {
                Id = favorite.Id,
                UserId = request.UserId,
                Book = MapBookToDto(book)
            };
        }

        public async Task<List<FavoriteResponseDto>> GetUserFavoritesAsync(Guid userId)
        {
            var favorites = await _context.Favorites
                .Include(f => f.Book)
                    .ThenInclude(b => b.Genres)
                .Include(f => f.Book.Reviews)
                .ToListAsync();

            return favorites.Select(f => new FavoriteResponseDto
            {
                Id = f.Id,
                UserId = userId,
                Book = MapBookToDto(f.Book)
            }).ToList();
        }

        public async Task<ReadingProgressResponseDto> UpdateReadingProgressAsync(Guid userId, ReadingProgressRequestDto request)
        {
            var progress = await _context.ReadingProgress
                .FirstOrDefaultAsync(r => r.BookId == request.BookId && r.UserId == userId);

            if (progress == null)
            {
                progress = new ReadingProgress
                {
                    BookId = request.BookId,
                    UserId = userId,
                    CurrentPage = request.CurrentPage,
                    TotalPages = request.TotalPages
                };
                _context.ReadingProgress.Add(progress);
            }
            else
            {
                progress.CurrentPage = request.CurrentPage;
                progress.TotalPages = request.TotalPages;
                progress.LastUpdated = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return new ReadingProgressResponseDto
            {
                BookId = progress.BookId,
                UserId = progress.UserId,
                CurrentPage = progress.CurrentPage,
                TotalPages = progress.TotalPages,
                Percentage = progress.TotalPages > 0 ? Math.Round((double)progress.CurrentPage / progress.TotalPages * 100, 2) : 0,
                LastUpdated = progress.LastUpdated,
                Book = MapBookToDto(await _context.Books
                .Include(b => b.Reviews)
                .Include(b => b.Genres)
                .FirstOrDefaultAsync(b => b.Id == progress.BookId)),
            };
        }

        public async Task<ReadingProgressResponseDto?> GetLastReadingProgressAsync(Guid userId)
        {
            var progress = await _context.ReadingProgress
         .Where(r => r.UserId == userId)
         .OrderByDescending(r => r.LastUpdated)
         .Include(r => r.Book)
             .ThenInclude(b => b.Genres)
         .Include(r => r.Book.Reviews)
         .FirstOrDefaultAsync();

            if (progress == null)
                return null;

            return new ReadingProgressResponseDto
            {
                BookId = progress.BookId,
                UserId = progress.UserId,
                CurrentPage = progress.CurrentPage,
                TotalPages = progress.TotalPages,
                Percentage = progress.TotalPages > 0
                    ? Math.Round((double)progress.CurrentPage / progress.TotalPages * 100, 2)
                    : 0,
                LastUpdated = progress.LastUpdated,
                Book = MapBookToDto(await _context.Books
                .Include(b => b.Reviews)
                .Include(b => b.Genres)
                .FirstOrDefaultAsync(b => b.Id == progress.BookId))
            };
        }

        public async Task<ReadingProgressResponseDto?> GetReadingProgressAsync(Guid userId, int bookId)
        {
            var progress = await _context.ReadingProgress
                .FirstOrDefaultAsync(r => r.BookId == bookId && r.UserId == userId);

            if (progress == null) return null;

            return new ReadingProgressResponseDto
            {
                BookId = progress.BookId,
                UserId = progress.UserId,
                CurrentPage = progress.CurrentPage,
                TotalPages = progress.TotalPages,
                Percentage = progress.TotalPages > 0 ? Math.Round((double)progress.CurrentPage / progress.TotalPages * 100, 2) : 0,
                LastUpdated = progress.LastUpdated,
                Book = MapBookToDto(await _context.Books
                .Include(b => b.Reviews)
                .Include(b => b.Genres)
                .FirstOrDefaultAsync(b => b.Id == progress.BookId)),
            };
    }

    private BookResponseDto MapBookToDto(Book book)
        {
            return new BookResponseDto
            {
                Id = book.Id,
                Image = book.Image,
                Title = book.Title,
                Author = book.Author,
                Rating = book.Rating,
                Description = book.Description,
                Language = book.Language,
                Genres = book.Genres.Select(g => new GenreResponseDto
                {
                    Id = g.Id,
                    Name = g.Name,
                }).ToList(),
                Reviews = book.Reviews.Select(r => new ReviewResponseDto
                {
                    User = r.User,
                    Comment = r.Comment
                }).ToList(),
                PdfFile = book.PdfFile,
                PdfFileName = book.PdfFileName,
                CreatedAt = book.CreatedAt
            };
        }

    }


}
