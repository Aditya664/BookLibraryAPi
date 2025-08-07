using BookLibraryAPi.Model;

namespace BookLibraryAPi.DTOs
{
    public class BookResponseDto
    {
        public int Id { get; set; }

        public string Image { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public double Rating { get; set; }
        public string Description { get; set; }
        public List<GenreResponseDto> Genres { get; set; }
        public List<ReviewResponseDto> Reviews { get; set; }
        public byte[]? PdfFile { get; set; }
        public string? PdfFileName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
