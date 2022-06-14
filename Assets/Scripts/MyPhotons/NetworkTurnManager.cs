using Games.Models;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyPhotons
{
    public static class NetworkTurnManager
    {
        private static TurnManager turnManager = TurnManager.Instance;

        public static bool IsMyTurn()
        {
            return PhotonNetwork.LocalPlayer.GetPlayerType() == turnManager.currentPlayer;
        }
    }
}

