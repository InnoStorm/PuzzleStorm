﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOLibrary.Responses
{
    public class ContinueRoomResponse : Response
    {
        public ContinueGameResponse CreatedGame { get; set; }
    }
}
