namespace BookLibraryAPi.DTOs
{
    public class ReviewRequestDto
    {
        public string User { get; set; }
        public string Comment { get; set; }
        public string BookId { get; set; }
    }
}