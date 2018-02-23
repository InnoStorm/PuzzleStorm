using System;
using System.Collections.Generic;
using System.Text;
using StormCommonData.Enums;

namespace StormCommonData.EventArgs
{
    public class LogMessageArgs : System.EventArgs
    {
        public readonly LogMessageType Type;
        public readonly string Message;

        public LogMessageArgs(string message, LogMessageType type = LogMessageType.Info)
        {
            Type = type;
            Message = message;
        }
    }
}
