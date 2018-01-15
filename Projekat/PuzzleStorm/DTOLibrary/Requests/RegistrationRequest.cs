namespace DTOLibrary.Requests
{
    public class RegistrationRequest : PreLoginRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
