namespace DataLayer.Core.Domain
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public virtual Player PlayerForUser { get; set; }
        public string IsLogged { get; set; }
        public string AuthToken { get; set; }
    }
}
