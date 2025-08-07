using System.Text.Json.Serialization;

namespace BookLibraryAPi.Model
{
    public class Review
    {
        public int Id { get; set; }

        public string User { get; set; }

        public string Comment { get; set; }

        public int BookId { get; set; }

        [JsonIgnore]
        public Book Book { get; set; }
    }
}