namespace BookLibraryAPi.DTOs
{
    public class BookUploadDto
    { 
        public string Title { get; set; }
        public string Author { get; set; }
        public double Rating { get; set; }
        public string Description { get; set; }
        public string? Image { get; set; }

        public List<int> GenreIds { get; set; } = new();
    }

}
