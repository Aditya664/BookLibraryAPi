using System.ComponentModel.DataAnnotations;

namespace BookLibraryAPi.Model
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        public string Image { get; set; }

        [Required]
        public string Title { get; set; }

        public string Author { get; set; }

        public double Rating { get; set; }

        public string Description { get; set; }

        public List<Genre> Genres { get; set; } = new ();
        public List<Review> Reviews { get; set; } = new();

        public byte[]? PdfFile { get; set; }
        public string? PdfFileName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
