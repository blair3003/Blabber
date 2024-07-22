namespace Blabber.Api.Models
{
    public class AuthorProfile
    {
        public int Id { get; set; }
        public string? Handle { get; set; }
        public string? DisplayName { get; set; }
        public string? DisplayPic { get; set; }
        public ICollection<AuthorView> Following { get; set; } = [];
        public ICollection<AuthorView> Followers { get; set; } = [];

        public int FollowingCount => Following.Count;
        public int FollowersCount => Followers.Count;
    }
}