namespace BookLibraryAPi.DTOs
{
    public class ReadingProgressRequestDto
    {
        public int BookId { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
