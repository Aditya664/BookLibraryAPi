using BookLibraryAPi.Entities;

namespace BookLibraryAPi.Model
{
    public class Favorite
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; }
    }
}
