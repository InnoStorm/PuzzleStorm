using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOLibrary.Requests;

namespace Server
{
    public static class StormValidator
    {
        private static void ValidateUsernameFormat(string username)
        {
            if (username == string.Empty)
                throw new Exception("Username can not be empty!");
            if (username.Contains(" "))
                throw new Exception("Username can not contain spaces!");
        }

        private static void ValidateEmailFormat(string email)
        {
            //TODO: Implement
        }

        private static void ValidatePasswordFormat(string password)
        {
            if (password == string.Empty)
                throw new Exception("Password can not be empty!");
        }

        private static void ValidateRequesterIdFormat(PostLoginRequest request)
        {
            if (request.RequesterId <= 0)
                throw new Exception("RequesterID not valid: ID <= 0!");
        }

        private static void ValidateIDFormat(int Id)
        {
            if (Id <= 0)
                throw new Exception("ID is not valid. ID <= 0!");
        }
        //Prelogin

        public static void ValidateRequest(LoadGameRequest request)
        {
            //TODO
        }

        public static void ValidateRequest(RegistrationRequest request)
        {
            ValidateUsernameFormat(request.Username);
            ValidateEmailFormat(request.Email);
            ValidatePasswordFormat(request.Password);
        }

        public static void ValidateRequest(LoginRequest request)
        {
            ValidateUsernameFormat(request.Username);
            ValidatePasswordFormat(request.Password);
        }

        public static void ValidateRequest(SignOutRequest request)
        {
            ValidateRequesterIdFormat(request);
        }

        //Postlogin
        public static void ValidateRequest(CreateRoomRequest request)
        {
            ValidateRequesterIdFormat(request);
            
            if (request.MaxPlayers < 2)
                throw new Exception("MaxPlayer must be >= 2!");

            if (request.NumberOfRounds < 1)
                throw new Exception("Number of rounds must be >= 2!");
        }

        public static void ValidateRequest(GetAllRoomsRequest request)
        {
            ValidateRequesterIdFormat(request);
        }

        public static void ValidateRequest(CancelRoomRequest request)
        {
            ValidateRequesterIdFormat(request);
            ValidateIDFormat(request.RoomId);
        }

        public static void ValidateRequest(RoomCurrentStateRequest request)
        {
            ValidateRequesterIdFormat(request);
            ValidateIDFormat(request.RoomId);
        }

        public static void ValidateRequest(JoinRoomRequest request)
        {
            ValidateRequesterIdFormat(request);
            ValidateIDFormat(request.RoomId);
        }

        public static void ValidateRequest(ChangeRoomPropertiesRequest request)
        {
            ValidateRequesterIdFormat(request);
            ValidateIDFormat(request.RoomId);
            
            if (request.MaxPlayers < 2)
                throw new Exception("Max player value is not valid. Must be >= 2");

            if (request.NumberOfRounds < 1)
                throw new Exception("Number of rounds must be >= 2!");
        }

        public static void ValidateRequest(ChangeStatusRequest request)
        {
            ValidateRequesterIdFormat(request);
        }

        public static void ValidateRequest(LeaveRoomRequest request)
        {
            ValidateRequesterIdFormat(request);
        }

        public static void ValidateRequest(StartGameRequest request)
        {
            
        }

        public static void ValidateRequest(StartRoomRequest request)
        {
            
        }

        //public static void ValidateRequest(ContinueGameRequest request)
        //{
            
        //}

        //public static void ValidateRequest(ContinueRoomRequest request)
        //{
            
        //}
    }
}
