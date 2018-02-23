using StormCommonData.Enums;

namespace DTOLibrary.Responses
{
    public class LoginResponse : Response
    {
        public string AuthToken { get; set; }
        public int PlayerId { get; set; }
    }
}
