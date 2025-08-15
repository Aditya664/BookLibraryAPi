namespace BookLibraryAPi.DTOs
{
    public class FavoriteRequestDto
    {
        public Guid UserId { get; set; }
        public int BookId { get; set; }
    }
}
