using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOLibrary.SubDTOs
{
    public class Player
    {
        public int PlayerId { get; set; }
        public string Username { get; set; }
        public bool IsReady { get; set; }
    }
}
