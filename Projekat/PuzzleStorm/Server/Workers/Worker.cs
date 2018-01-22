using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Persistence;


namespace Server.Workers
{
    public abstract class Worker
    {
        protected enum LogMessageType
        {
            Normal,
            Warning,
            Error
        }

        public int Id { get; set; }

        protected UnitOfWork CreateUnitOfWork()
        {
            return new UnitOfWork(new StormContext());
        }

        protected void WorkerLog(string message, LogMessageType type = LogMessageType.Normal)
        {
            switch (type)
            {
                case LogMessageType.Normal:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case LogMessageType.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogMessageType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            Console.WriteLine($"[{DateTime.Now}][WORKER {Id}] {message}");

            Console.ResetColor();
        }

        protected string ExceptionStack(Exception ex)
        {
            string result = string.Empty;

            Exception e = ex;
            while (e != null)
            {
                result += e.Message + Environment.NewLine;
                e = e.InnerException;
            }

            return result;
        }
    }
}
