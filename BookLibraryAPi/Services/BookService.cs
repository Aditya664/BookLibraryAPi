using BookLibraryAPi.DB;
using BookLibraryAPi.DTOs;
using BookLibraryAPi.Model;
using Microsoft.EntityFrameworkCore;
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
    }

}
