using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Core.Domain;
using DataLayer.Persistence;
using DTOLibrary.Enums;
using DTOLibrary.Requests;
using DTOLibrary.Responses;
using Server.Workers;

namespace ServerAuth.Workers
{
    class AuthWorker : Worker
    {
        public RegistrationResponse Register(RegistrationRequest request)
        {
            try
            {
                using (UnitOfWork data = CreateUnitOfWork())
                {
                    if (data.Users.UsernameExists(request.Username))
                        throw new Exception("Username is already in use!");
                    if (request.Password == string.Empty)
                        throw new Exception("Password can not be empty!");

                    var newUser = new User
                    {
                        Username = request.Username,
                        Password = request.Password,
                        Email = request.Email
                    };

                    data.Users.Add(newUser);
                    data.Complete();

                    WorkerLog($"Successfull registration for username: { request.Username}");

                    return new RegistrationResponse()
                    {
                        Status = OperationStatus.Successfull,
                        Username = request.Username,
                        Details = "Successful registration!"
                    };
                }
            }
            catch (Exception ex)
            {
                WorkerLog($"Failed registration for username: {request.Username}; Reason: {ex.Message}");

                return new RegistrationResponse()
                {
                    Status = OperationStatus.Failed,
                    Username = request.Username,
                    Details = ex.Message
                };
            }
        }

        public LoginResponse Login(LoginRequest request)
        {
            try
            {
                using (var data = new UnitOfWork(new StormContext()))
                {
                    if (!data.Users.UsernameExists(request.Username))
                        throw new Exception("Username not found!");


                    User user = data.Users.FindByUsername(request.Username);
                    if (user.Password != request.Password)
                        throw new Exception("Password does not match!");

                    WorkerLog($"Successfull login for username: {request.Username};");
                    return new LoginResponse()
                    {
                        AuthToken = request.Username,
                        Status = OperationStatus.Successfull,
                        Details = "Successfull login"
                    };
                }
            }
            catch (Exception ex)
            {
                WorkerLog($"Failed login for username: {request.Username}; Reason: {ex.Message}");
                return new LoginResponse()
                {
                    AuthToken = "",
                    Status = OperationStatus.Failed,
                    Details = ex.Message
                };
            }
        }

        //logout function
    }
}
