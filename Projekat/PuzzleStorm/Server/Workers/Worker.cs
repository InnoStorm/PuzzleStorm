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
        public int Id { get; set; }

        protected UnitOfWork CreateUnitOfWork()
        {
            return new UnitOfWork(new StormContext());
        }

        protected void WorkerLog(string message)
        {
            Console.WriteLine($"[{DateTime.Now}][WORKER {Id}] {message}");
        }
    }
}
