using System;
using EasyNetQ;

namespace Client {

    public sealed class RabbitBus
    {
        private static readonly Lazy<RabbitBus> BusInstance
                            = new Lazy<RabbitBus>(() => new RabbitBus("amqp://ygunknwy:pAncRrH8Gxk3ULDyy-Wju7NIqdBThwCJ@sheep.rmq.cloudamqp.com/ygunknwy"));
        public IBus Bus { get; set; } 

        public static RabbitBus Instance => BusInstance.Value;

        private RabbitBus() {
        }

        private RabbitBus(string bus)
        {
            this.Bus = RabbitHutch.CreateBus(bus);
        }
    }
}
