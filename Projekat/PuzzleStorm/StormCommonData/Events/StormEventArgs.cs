using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StormCommonData.Events
{
    public class StormEventArgs<TEventPayload> : System.EventArgs
    {
        public TEventPayload Data { get; }

        public StormEventArgs(TEventPayload payload)
        {
            Data = payload;
        }
    }
}
