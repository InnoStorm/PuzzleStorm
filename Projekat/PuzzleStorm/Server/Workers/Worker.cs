using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataLayer.Persistence;
using EasyNetQ;
using StormCommonData.Enums;
using StormCommonData.EventArgs;


namespace Server.Workers
{
    public abstract class Worker
    {
        #region Constructors

        protected Worker(IBus communicator)
        {
            Communicator = communicator;
        }
        
        #endregion
        
        #region Events

        public EventHandler<LogMessageArgs> NewWorkerLogMessage;

        protected virtual void OnNewLogMessage(LogMessageArgs msg)
            => NewWorkerLogMessage?.Invoke(this, msg);

        #endregion

        #region Properties

        public int Id { get; set; }

        protected readonly IBus Communicator;
        
        protected static UnitOfWork WorkersUnitOfWork => new UnitOfWork(new StormContext());

        #endregion

        protected void Log(string message, LogMessageType type = LogMessageType.Info)
        {
            OnNewLogMessage(new LogMessageArgs(
                message: $@"[{DateTime.Now}][WORKER {Id}] {message}",
                type: type
                ));
        }
    }
}
