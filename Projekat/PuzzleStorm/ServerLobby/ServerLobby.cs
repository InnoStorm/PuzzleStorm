using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server;

namespace ServerLobby
{
    class ServerLobby : StormServer
    {
        static void Main(string[] args)
        {
            ServerInstance = new ServerLobby();
            ServerInstance.Start();
        }

        protected override void StartupInit()
        {
            InitWorkerPool();
            BindWorkerMethods();
        }

        private void InitWorkerPool()
        {
            throw new NotImplementedException();
        }

        private void BindWorkerMethods()
        {
            throw new NotImplementedException();
        }

        

        protected override void ShutdownCleanUp()
        {
            throw new NotImplementedException();
        }
    }
}
