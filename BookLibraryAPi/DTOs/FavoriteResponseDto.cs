namespace BookLibraryAPi.DTOs
{
    public class FavoriteResponseDto
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public BookResponseDto Book { get; set; } 
    }
}
