using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Photons
{
    public static class RoomExtensions
    {
        public static IEnumerable<Player> SortedPlayers(this Room room)
        {
            return room.Players.Values.OrderBy(x => x.GetPlayerType());
        }
    }
}

