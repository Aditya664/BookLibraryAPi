namespace BookLibraryAPi.DTOs
{
    public class ReviewResponseDto
    {
        public int Id { get; set; }

        public string User { get; set; }
        public string Comment { get; set; }
        public int BookId { get; set; }
    }
}