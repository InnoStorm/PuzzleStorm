using System;
using System.Collections.Generic;
using System.Text;
using StormCommonData.EventArgs;

namespace StormCommonData.Interfaces
{
    public interface IStormServer : IDisposable
    {
        void Start();
        void Stop();
        bool IsRunning { get; }

        event EventHandler<LogMessageArgs> NewLogMessage;
    }
}
