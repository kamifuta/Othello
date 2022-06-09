using ExitGames.Client.Photon;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;
using Games;

namespace Photons
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

        public static List<Players> GetTurnList(this Room room)
        {
            return (room.CustomProperties[TurnKey] is List<Players> turnList) ? turnList : null;
        }

        public static void SetRandomTurn(this Room room, bool isRandom)
        {
            propsToSet[RandomTurnKey] = isRandom;
            room.SetCustomProperties(propsToSet);
            propsToSet.Clear();
        }

        public static void SetTurnList(this Room room, List<Players> turnList)
        {
            propsToSet[TurnKey] = turnList;
            room.SetCustomProperties(propsToSet);
            propsToSet.Clear();
        }
    }
}

