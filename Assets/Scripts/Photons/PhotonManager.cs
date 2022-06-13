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
        [SerializeField] private GameUIView gameUIView;

        private void Start()
        {
            //var players = PhotonNetwork.CurrentRoom.SortedPlayers();
            //List<string> nicknameList = new List<string>();

            //foreach(var x in players)
            //{
            //    nicknameList.Add(x.NickName);
            //}

            //gameUIView.SetNickname(nicknameList);
        }
    }

}
