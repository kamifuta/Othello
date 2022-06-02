using Games.Views;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Photons
{
    public class PhotonManager : MonoBehaviour
    {
        [SerializeField] private UIManager _UIManager;

        private void Start()
        {
            var players = PhotonNetwork.CurrentRoom.Players.Values.OrderBy(x => x.GetPlayerType());
            List<string> nicknameList = new List<string>();

            foreach(var x in players)
            {
                nicknameList.Add(x.NickName);
            }

            _UIManager.SetNickname(nicknameList);
        }
    }

}
