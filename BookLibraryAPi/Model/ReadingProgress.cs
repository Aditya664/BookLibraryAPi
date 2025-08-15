using BookLibraryAPi.Entities;

namespace BookLibraryAPi.Model
{
    public class ReadingProgress
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; } 
        public Guid UserId { get; set; }  
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
