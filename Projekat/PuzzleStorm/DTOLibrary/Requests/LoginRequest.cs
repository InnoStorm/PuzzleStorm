namespace DTOLibrary.Requests
{
    public class LoginRequest : PreLoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}