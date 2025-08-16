namespace BookLibraryAPi.DTOs
{
    public class UserListDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; } = new();
        public bool HasSubscription { get; set; } = false;
    }

}
