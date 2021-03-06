﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataLayer.Core.Domain;
using DataLayer.Persistence;
using StormCommonData.Enums;
using DTOLibrary.Requests;
using DTOLibrary.Responses;
using EasyNetQ;
using Server;
using Server.Workers;
using StormCommonData;

namespace ServerAuth.Workers
{
    class AuthWorker : Worker
    {
        public AuthWorker(IBus communicator) : base(communicator)
        {
        }

        public RegistrationResponse Register(RegistrationRequest request)
        {
            try
            {
                StormValidator.ValidateRequest(request);

                using (UnitOfWork data = WorkersUnitOfWork)
                {
                    if (!data.Players.IsUsernameAvailable(request.Username))
                        throw new Exception("Username is not available!");

                    var newPlayer = new Player()
                    {
                        Username = request.Username,
                        Password = request.Password,
                        Email = request.Email,

                    };

                    data.Players.Add(newPlayer);
                    data.Complete();

                    Log($"[SUCCESS] Registration for: {request.Username}");

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
                Log($"[FAILED] Registration for: {request.Username}; Reason: {StormUtils.FlattenException(ex)}",
                    LogMessageType.Error);

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
                StormValidator.ValidateRequest(request);

                using (var data = new UnitOfWork(new StormContext()))
                {
                    var player = data.Players.Get(request.Username);

                    if (player == null)
                        throw new Exception($"Player with username {request.Username} not found!");
                    
                    if (player.Password != request.Password)
                        throw new Exception("Wrong password");

                    //if (player.IsLogged)
                    //    throw new Exception($"Player with username {request.Username} is already logged!");

                    player.IsLogged = true;
                    player.AuthToken = Guid.NewGuid().ToString();
                    player.CurrentRoom = null;
                    player.IsReady = false;
                    player.Score = 0;

                    data.Complete();

                    Log($"[SUCCESS] Login for username: {request.Username}");

                    return new LoginResponse()
                    {
                        PlayerId = player.Id,
                        AuthToken = player.AuthToken,
                        Status = OperationStatus.Successfull,
                        Details = "Successfull login"
                    };
                }
            }
            catch (Exception ex)
            {
                Log($"[FAILED] Login for username: {request.Username}; Reason: {StormUtils.FlattenException(ex)}", LogMessageType.Error);
                return new LoginResponse()
                {
                    AuthToken = "",
                    Status = OperationStatus.Failed,
                    Details = ex.Message
                };
            }
        }

        public SignOutResponse SignOut(SignOutRequest request)
        {
            try
            {
                StormValidator.ValidateRequest(request);

                using (UnitOfWork data = WorkersUnitOfWork)
                {
                    Player player = data.Players.Get(request.RequesterId);

                    if (player == null)
                        throw new Exception("User not found! Can not signout!");

                    player.AuthToken = "";
                    player.CurrentRoom = null;
                    player.IsLogged = false;
                    player.IsReady = false;
                    player.Score = 0;

                    data.Complete();

                    Log($"[SUCCESS] Signout for username: { player.Username}");

                    return new SignOutResponse()
                    {
                        Status = OperationStatus.Successfull,
                        Details = "Successful Signout."
                    };
                }
            }
            catch (Exception ex)
            {
                Log($"[FAILED] Signout. Reason: {StormUtils.FlattenException(ex)}.", LogMessageType.Error);

                return new SignOutResponse()
                {
                    Status = OperationStatus.Failed,
                    Details = ex.Message
                };
            }
        }
    }
}
