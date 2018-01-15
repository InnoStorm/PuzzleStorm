using DTOLibrary.Enums;

namespace DTOLibrary.Responses
{
    public class Response
    {
        public OperationStatus Status { get; set; }
        public string Details { get; set; }
    }
}
