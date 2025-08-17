using BookLibraryAPi.DB;
using BookLibraryAPi.DTOs;
using BookLibraryAPi.Entities;
using BookLibraryAPi.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using static System.Reflection.Metadata.BlobBuilder;

namespace BookLibraryAPi.Services
{
    public class BookService : IBookService
    {
        private readonly AppDbContext _context;
        private readonly Int32 MaxAllowedMinutes = 400;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookService(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
                    User = g.User,
                }).ToList(),
                CreatedAt = book.CreatedAt,
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

        public async Task<GenreResponseDto> AddGenreAsync(GenreRequestDto genreDto)
        {
            var genre = new Genre
            {
                Name = genreDto.Name,
                IconName = genreDto.IconName
            };

            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            return new GenreResponseDto
            {
                Id = genre.Id,
                Name = genre.Name,
                IconName = genre.IconName
            };
        }

        public async Task<GenreResponseDto?> UpdateGenreAsync(int id, GenreRequestDto genreDto)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null) return null;

            genre.Name = genreDto.Name;
            genre.IconName = genreDto.IconName;

            _context.Genres.Update(genre);
            await _context.SaveChangesAsync();

            return new GenreResponseDto
            {
                Id = genre.Id,
                Name = genre.Name,
                IconName = genre.IconName
            };
        }

        public async Task<bool> DeleteGenreAsync(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null) return false;

            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<List<BookResponseDto>> GetAllBooksAsync()
        {
            return await _context.Books
                .Select(b => new BookResponseDto
                {
                    Id = b.Id,
                    Image = b.Image,
                    Title = b.Title,
                    Language = b.Language,
                    Author = b.Author,
                    Rating = b.Rating,
                    Description = b.Description,
                    Genres = b.Genres
                        .Select(g => new GenreResponseDto
                        {
                            Id = g.Id,
                            Name = g.Name
                        }).ToList(),
                    Reviews = b.Reviews
                        .Select(r => new ReviewResponseDto
                        {
                            User = r.User,
                            Comment = r.Comment
                        }).ToList(),
                    CreatedAt = b.CreatedAt,
                    PdfFileName = b.PdfFileName
                })
                .ToListAsync();
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

        public async Task<bool?> CheckFavoriteOrNot(FavoriteRequestDto request)
        {
            // Check if favorite already exists
            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == request.UserId && f.BookId == request.BookId);

            if (favorite != null)
            {
               
                return true;
            }
            else
            {
                return false;

            }

        }

        public async Task<List<FavoriteResponseDto>> GetUserFavoritesAsync(Guid userId)
        {
            var favorites = await _context.Favorites
                .Where(f => f.UserId == userId) // ✅ filter by user
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


        public async Task<FavoriteResponseDto?> ToggleFavoriteAsync(FavoriteRequestDto request)
        {
            var favorite = await _context.Favorites
                .AsTracking()
                .SingleOrDefaultAsync(f => f.UserId == request.UserId && f.BookId == request.BookId);

            if (favorite != null)
            {
                _context.Favorites.Remove(favorite);
                await _context.SaveChangesAsync();
                return null;
            }

            var newFavorite = new Favorite
            {
                UserId = request.UserId,
                BookId = request.BookId
            };

            _context.Favorites.Add(newFavorite);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Unique constraint will prevent duplicate inserts
                return await GetFavoriteAsync(request.UserId, request.BookId);
            }

            var book = await _context.Books
                .Include(b => b.Genres)
                .Include(b => b.Reviews)
                .FirstOrDefaultAsync(b => b.Id == request.BookId);

            return new FavoriteResponseDto
            {
                Id = newFavorite.Id,
                UserId = request.UserId,
                Book = MapBookToDto(book)
            };
        }

        private async Task<FavoriteResponseDto?> GetFavoriteAsync(Guid userId, int bookId)
        {
            var favorite = await _context.Favorites
                .Include(f => f.Book)
                .ThenInclude(b => b.Genres)
                .Include(f => f.Book.Reviews)
                .SingleOrDefaultAsync(f => f.UserId == userId && f.BookId == bookId);

            return favorite == null ? null : new FavoriteResponseDto
            {
                Id = favorite.Id,
                UserId = userId,
                Book = MapBookToDto(favorite.Book)
            };
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

        public async Task<List<ReadingProgressResponseDto>> GetReadingHistoryAsync(Guid userId)
        {
            var progresses = await _context.ReadingProgress
                .Where(r => r.UserId == userId)
                .ToListAsync();

            if (progresses == null || !progresses.Any())
                return new List<ReadingProgressResponseDto>();

            var result = new List<ReadingProgressResponseDto>();

            foreach (var progress in progresses)
            {
                var book = await _context.Books
                    .Include(b => b.Reviews)
                    .Include(b => b.Genres)
                    .FirstOrDefaultAsync(b => b.Id == progress.BookId);

                result.Add(new ReadingProgressResponseDto
                {
                    BookId = progress.BookId,
                    UserId = progress.UserId,
                    CurrentPage = progress.CurrentPage,
                    TotalPages = progress.TotalPages,
                    Percentage = progress.TotalPages > 0
                        ? Math.Round((double)progress.CurrentPage / progress.TotalPages * 100, 2)
                        : 0,
                    LastUpdated = progress.LastUpdated,
                    Book = MapBookToDto(book)
                });
            }

            return result;
        }


        public async Task<BookResponseDto?> UpdateBookAsync(int bookId, BookUploadDto bookDto, IFormFile? pdfFile)
        {
            var book = await _context.Books
                .Include(b => b.Genres)
                .Include(b => b.Reviews)
                .FirstOrDefaultAsync(b => b.Id == bookId);

            if (book == null) return null;

            // Update basic fields
            book.Title = bookDto.Title;
            book.Author = bookDto.Author;
            book.Description = bookDto.Description;
            book.Language = bookDto.Language;
            book.Image = bookDto.Image;
            book.Rating = bookDto.Rating;

            // Update genres
            var existingGenres = await _context.Genres
                .Where(g => bookDto.GenreIds.Contains(g.Id))
                .ToListAsync();
            book.Genres = existingGenres;

            // Update PDF if provided
            if (pdfFile != null && pdfFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await pdfFile.CopyToAsync(ms);
                book.PdfFile = ms.ToArray();
                book.PdfFileName = pdfFile.FileName;
            }

            await _context.SaveChangesAsync();

            return MapBookToDto(book);
        }

        public async Task<bool> DeleteBookAsync(int bookId)
        {
            var book = await _context.Books
                .Include(b => b.Reviews)
                .FirstOrDefaultAsync(b => b.Id == bookId);

            if (book == null) return false;

            // Optionally remove related reviews
            if (book.Reviews.Any())
            {
                _context.Reviews.RemoveRange(book.Reviews);
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return true;
        }


        public async Task<ReadingProgress> GetProgressAsync(Guid userId, int bookId)
        {
            return await _context.ReadingProgress
                .FirstOrDefaultAsync(r => r.UserId == userId && r.BookId == bookId);
        }

        public async Task<ReadingProgress> StartReadingAsync(Guid userId, int bookId)
        {
            var progress = await GetProgressAsync(userId, bookId);

            if (progress == null)
            {
                progress = new ReadingProgress
                {
                    UserId = userId,
                    BookId = bookId,
                    CurrentPage = 0,
                    TotalPages = 0,
                    TotalReadingMinutes = 0
                };
                _context.ReadingProgress.Add(progress);
            }

            progress.LastUpdated = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return progress;
        }

        public async Task<bool> EndReadingAsync(Guid userId, int bookId, int sessionMinutes)
        {
            var progress = await GetProgressAsync(userId, bookId);

            if (progress == null) return false;

            progress.TotalReadingMinutes += sessionMinutes;
            progress.LastUpdated = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user.HasSubscription) return true;
            return progress.TotalReadingMinutes < MaxAllowedMinutes;
        }

        public async Task<bool> HasReadingTimeLeftAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user.HasSubscription) return true;
            var totalMinutes = await _context.ReadingProgress
                .Where(r => r.UserId == userId)
                .SumAsync(r => r.TotalReadingMinutes);

            return totalMinutes < MaxAllowedMinutes;
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
