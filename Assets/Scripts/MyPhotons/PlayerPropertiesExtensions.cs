using ExitGames.Client.Photon;
using Games;
using Photon.Realtime;
using System;
using UnityEngine;

namespace MyPhotons
{
    public static class PlayerPropertiesExtensions
    {
        private const string PlayerKey = "Player";
        private const string IsReversingKey = "Rverce";

        private static readonly Hashtable propsToSet = new Hashtable();

        public static Players GetPlayerType(this Player player)
        {
            return (player.CustomProperties[PlayerKey] is int playerType) ? (Players)Enum.ToObject(typeof(Players), playerType) : Players.None;
        }

        public static bool GetIsRevercing(this Player player)
        {
            return (player.CustomProperties[IsReversingKey] is bool isRevercing) ? isRevercing : false;
        }

        public static void SetPlayerType(this Player player, int playerType)
        {
            propsToSet[PlayerKey] = playerType;
            player.SetCustomProperties(propsToSet);
            propsToSet.Clear();
        }

        public static void SetIsRevercing(this Player player, bool isRevercing)
        {
            propsToSet[IsReversingKey] = isRevercing;
            player.SetCustomProperties(propsToSet);
            propsToSet.Clear();
        }
    }

}
