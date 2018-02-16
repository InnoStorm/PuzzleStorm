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
    public static class ClientUtils
    {
        #region Requests

        public static async Task<TResponse> PerformRequestAsync<TRequest, TResponse>(Func<TRequest, Task<TResponse>> RequestFunc, TRequest request)
            where TRequest : Request, new()
            where TResponse : Response, new()
        {
            var popup = new LoadingPopup()
            {
                Message = { Text = "Just a moment.." }
            };

            TResponse response = null;

            await DialogHost.Show(popup, async delegate (object sender, DialogOpenedEventArgs args)
            {
                response = await RequestFunc(request);
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

        #endregion

        #region Subscribe

        private static string SubscriptionId
        {
            get
            {
                if (Player.Instance.Id <= 0)
                    throw new Exception("ClientUtils: SubscriptionId is invalid! PlayerId iz not initialized");

                return Player.Instance.Id.ToString();
            }
        }
        
        #endregion
        
    }
}
