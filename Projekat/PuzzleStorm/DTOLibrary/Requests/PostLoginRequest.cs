using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOLibrary.Requests
{
    public class PostLoginRequest : Request
    {
        public int RequesterId { get; set; }
    }
}
