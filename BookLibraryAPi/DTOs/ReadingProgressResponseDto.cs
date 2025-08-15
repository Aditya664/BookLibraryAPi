using System;

namespace BookLibraryAPi.DTOs
{
    public class ReadingProgressResponseDto
    {
        public int BookId { get; set; }
        public Guid UserId { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public double Percentage { get; set; }
        public DateTime LastUpdated { get; set; }

        public BookResponseDto Book { get; set; }  // Make sure you have a BookDto class
    }
}
