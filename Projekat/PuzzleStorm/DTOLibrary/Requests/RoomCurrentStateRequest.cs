﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOLibrary.Requests
{
    public class RoomCurrentStateRequest : PostLoginRequest
    {
        public int RoomId { get; set; }
    }
}
