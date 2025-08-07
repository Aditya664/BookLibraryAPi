using BookLibraryAPi.Model;

namespace BookLibraryAPi.DTOs
{
    public class GenreResponseWithBooksDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string IconName { get; set; }
        public List<GenreBookResponseDto> Books { get; set; }

    }
}
