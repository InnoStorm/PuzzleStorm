using DTOLibrary.Enums;

namespace DTOLibrary.Responses
{
    public class LoginResponse : Response
    {
        public string AuthToken { get; set; }
        public int UserId { get; set; }
    }
}
