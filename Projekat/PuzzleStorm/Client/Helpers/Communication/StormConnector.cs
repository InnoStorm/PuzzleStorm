using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Communicator;
using DTOLibrary.Enums;
using DTOLibrary.Requests;
using DTOLibrary.Responses;
using MaterialDesignThemes.Wpf;

namespace Client.Helpers.Communication
{
    public sealed class StormConnector
    {
        private static readonly Lazy<StormConnector> ConnectorInstance
            = new Lazy<StormConnector>(() => new StormConnector());

        public static StormConnector Instance => ConnectorInstance.Value;
        public static API Api => API.Instance;

        private StormConnector()
        {
        }

        public async Task<TResponse> PerformRequestAsync<TRequest, TResponse>
            (
                Func<TRequest, Task<TResponse>> RequestFunc, 
                TRequest request,
                String loadingMessage
            )
            where TRequest : Request, new()
            where TResponse : Response, new()
        {
            var popup = new LoadingPopup()
            {
                Message = { Text = loadingMessage }
            };

            TResponse response = null;

            await DialogHost.Show(popup, async delegate (object sender, DialogOpenedEventArgs args)
            {
                var task = RequestFunc(request);
                //task.Start();
                response = await task;
                args.Session.Close(false);
            });


            if (response?.Status == OperationStatus.Successfull)
                return response;
            

            await DialogHost.Show(new SampleMessageDialog
                {
                    Message = { Text = "Failed: " + response.Details }
                }
            );

            return null;
        }
    }
}
