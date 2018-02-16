using System;
using EasyNetQ;

namespace Client {

    //TODO REMOVE
    public sealed class RabbitBus
    {
        private const string ConnectionString =
            "host=sheep.rmq.cloudamqp.com;" +
            "virtualHost=ygunknwy;" +
            "username=ygunknwy;" +
            "password=pAncRrH8Gxk3ULDyy-Wju7NIqdBThwCJ;" +
            "timeout=0";

        private static readonly Lazy<RabbitBus> BusInstance
                            = new Lazy<RabbitBus>(() => new RabbitBus(ConnectionString));
        public IBus Bus { get; set; }

        public static RabbitBus Instance => BusInstance.Value;

        private RabbitBus()
        {
        }

        private RabbitBus(string bus)
        {
            this.Bus = RabbitHutch.CreateBus(bus);
        }
    }
}
