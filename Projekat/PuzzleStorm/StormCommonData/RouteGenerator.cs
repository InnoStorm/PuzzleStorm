using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StormCommonData.Enums;

namespace StormCommonData
{
    public static class RouteGenerator
    {
        public static class RoomUpdates
        {
            public static string ExchangeName => "RoomUpdates";

            public static class Room
            {
                public static string BaseRoute => "Room";

                public static class Filter
                {
                    public static string All()                   => $"{BaseRoute}.#";
                    public static string All(int roomId)         => $"{BaseRoute}.*.{roomId}";

                    public static string Created()               => $"{BaseRoute}.Created.#";
                    public static string Created(int id)         => $"{BaseRoute}.Created.{id}";

                    public static string BecameAvailable()       => $"{BaseRoute}.BecameAvailable.#";
                    public static string BecameAvailable(int id) => $"{BaseRoute}.BecameAvailable.{id}";

                    public static string Modified()              => $"{BaseRoute}.Modified.#";
                    public static string Modified(int id)        => $"{BaseRoute}.Modified.{id}";

                    public static string Started()               => $"{BaseRoute}.Started.#";
                    public static string Started(int id)         => $"{BaseRoute}.Started.{id}";

                    public static string Filled()                => $"{BaseRoute}.Filled.#";
                    public static string Filled(int id)          => $"{BaseRoute}.Filled.{id}";

                    public static string Deleted()               => $"{BaseRoute}.Deleted.#";
                    public static string Deleted(int id)         => $"{BaseRoute}.Deleted.{id}";

                    public static string FromEnum(RoomUpdateType updateType)             => $"{BaseRoute}.{updateType.ToString()}.#";
                    public static string FromEnum(RoomUpdateType updateType, int roomId) => $"{BaseRoute}.{updateType.ToString()}.{roomId}";
                }

                public static class Set
                {
                    public static string Created(int id) => $"{BaseRoute}.Created.{id}";
                    public static string BecameAvailable(int id) => $"{BaseRoute}.BecameAvailable.{id}";
                    public static string Modified(int id) => $"{BaseRoute}.Modified.{id}";
                    public static string Started(int id) => $"{BaseRoute}.Started.{id}";
                    public static string Filled(int id) => $"{BaseRoute}.Filled.{id}";
                    public static string Deleted(int id) => $"{BaseRoute}.Deleted.{id}";
                    public static string FromEnum(RoomUpdateType updateType, int roomId) => $"{BaseRoute}.{updateType.ToString()}.{roomId}";
                }
            }

            public static class InRoom
            {
                public static string BaseRoute => "InRoom";

                public static class Filter
                {
                    public static string All()            => $"{BaseRoute}.#";
                    public static string All(int roomId)  => $"{BaseRoute}.{roomId}.#";

                    public static string Joined(int roomId)        => $"{BaseRoute}.{roomId}.Joined";
                    public static string ChangedStatus(int roomId) => $"{BaseRoute}.{roomId}.ChangeStatus";
                    public static string LeftRoom(int roomId)      => $"{BaseRoute}.{roomId}.LeftRoom";

                    public static string FromEnum(RoomPlayerUpdateType updateType, int id) => $"{BaseRoute}.{id}.{updateType.ToString()}";

                }

                public static class Set
                {
                    public static string Joined(int roomId)        => $"{BaseRoute}.{roomId}.Joined";
                    public static string ChangedStatus(int roomId) => $"{BaseRoute}.{roomId}.ChangeStatus";
                    public static string LeftRoom(int roomId)      => $"{BaseRoute}.{roomId}.LeftRoom";

                    public static string FromEnum(RoomPlayerUpdateType updateType, int id) => $"{BaseRoute}.{id}.{updateType.ToString()}";
                }
            }
        }
    }
}
