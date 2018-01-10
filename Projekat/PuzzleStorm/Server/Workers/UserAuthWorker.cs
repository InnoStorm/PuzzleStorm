using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Core.Domain;
using DataLayer.Persistence;
using DTOLibrary.Enums;
using DTOLibrary.Requests;
using DTOLibrary.Responses;

namespace Server.Workers
{
    class UserAuthWorker
    {
        public int Id { get; set; }

        public RegistrationResponse Register(RegistrationRequest request)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(new StormContext()))
                {
                    if (unitOfWork.Users.UsernameExists(request.Username))
                        throw new Exception("Username is already in use!");
                    if (request.Password == string.Empty)
                        throw new Exception("Password can not be empty!");

                    var newUser = new User
                    {
                        Username = request.Username,
                        Password = request.Password,
                        Email = request.Email
                    };

                    unitOfWork.Users.Add(newUser);
                    unitOfWork.Complete();


                    Console.WriteLine($"[WORKER {Id}] Successfull registration for username: {request.Username}");
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
                Console.WriteLine($"[WORKER {Id}] Failed registration for username: {request.Username}; Reason: {ex.Message}");
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
                using (var unitOfWork = new UnitOfWork(new StormContext()))
                {
                    if (!unitOfWork.Users.UsernameExists(request.Username))
                        throw new Exception("Username not found!");

                    Console.WriteLine($"[WORKER {Id}] Successfull login for username: {request.Username};");
                    return new LoginResponse()
                    {
                        Username = request.Username,
                        Status = OperationStatus.Successfull,
                        Details = "Successfull login"
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WORKER {Id}] Failed login for username: {request.Username}; Reason: {ex.Message}");
                return new LoginResponse()
                {
                    Username = request.Username,
                    Status = OperationStatus.Failed,
                    Details = ex.Message
                };
            }
        }
    }
}
