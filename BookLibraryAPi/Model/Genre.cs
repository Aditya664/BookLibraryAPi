using System.Text.Json.Serialization;

namespace BookLibraryAPi.Model
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string IconName { get; set; }
        public List<Book> Books { get; set; }

    }


}
