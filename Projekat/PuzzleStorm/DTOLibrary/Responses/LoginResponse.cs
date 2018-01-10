using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOLibrary.Enums;

namespace DTOLibrary.Responses {
    public class LoginResponse
    {
        public string Username { get; set; }
        public OperationStatus Status { get; set; }
    }
}
