using ExitGames.Client.Photon;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;
using Games;
using System.Linq;
using System;

namespace MyPhotons
{
    public static class RoomPropertiesExtension
    {
        public const string RandomTurnKey = "RandomTurn";
        public const string TurnKey = "Turn";

        private static readonly Hashtable propsToSet = new Hashtable();

        public static bool GetRandomTurn(this Room room)
        {
            return (room.CustomProperties[RandomTurnKey] is bool isRandom) ? isRandom : true;
        }

        public static Players[] GetTurnArray(this Room room)
        {
            return (room.CustomProperties[TurnKey] is int[] turnArray) ? turnArray.Select(x=>(Players)Enum.ToObject(typeof(Players),x)).ToArray() : null;
        }

        public static void SetRandomTurn(this Room room, bool isRandom)
        {
            propsToSet[RandomTurnKey] = isRandom;
            room.SetCustomProperties(propsToSet);
            propsToSet.Clear();
        }

        public static void SetTurnArray(this Room room, int[] turnArray)
        {
            propsToSet[TurnKey] = turnArray;
            room.SetCustomProperties(propsToSet);
            propsToSet.Clear();
        }
    }
}

