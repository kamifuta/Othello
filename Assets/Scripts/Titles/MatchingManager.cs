using Games;
using Photon.Pun;
using Photon.Realtime;
using Photons;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Titles
{
    public class MatchingManager : MonoBehaviourPunCallbacks
    {
        [Serializable]
        private class PlayerInfoPanel
        {
            public GameObject panel;
            public Text nickname;
        }

        [SerializeField] private InputField nicknameInputField;
        [SerializeField] private List<PlayerInfoPanel> panels;

        private Subject<Unit> joinedRoomSubject = new Subject<Unit>();
        public IObservable<Unit> JoinedRoomObservable => joinedRoomSubject.AsObservable();

        private int playerNum;
        private int viewedCount = 0;
        private string nickname = "";

        public void StartMatching(int playerNum)
        {
            this.playerNum = playerNum;
            PhotonNetwork.ConnectUsingSettings();

            nicknameInputField.OnValueChangedAsObservable()
                .Subscribe(_ => nickname = nicknameInputField.text)
                .AddTo(this);
        }

        public override void OnConnectedToMaster()
        {
            // ランダムなルームに参加する
            PhotonNetwork.JoinRandomRoom(null, Convert.ToByte(playerNum));
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("参加しました"+PhotonNetwork.IsMasterClient);

            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.LocalPlayer.SetPlayerType(PhotonNetwork.CurrentRoom.PlayerCount);
            PhotonNetwork.LocalPlayer.NickName = nickname;

            ViewPlayerInfoPanels(playerNum);

            joinedRoomSubject.OnNext(Unit.Default);
            joinedRoomSubject.OnCompleted();
        }

        // ランダムで参加できるルームが存在しないなら、新規でルームを作成する
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            // ルームの参加人数を2人に設定する
            var roomOptions = new RoomOptions();
            roomOptions.MaxPlayers =Convert.ToByte(playerNum);

            PhotonNetwork.CreateRoom(null, roomOptions);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            SetPlayerNickname(newPlayer);
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            if (targetPlayer != PhotonNetwork.LocalPlayer) return;

            var players = PhotonNetwork.CurrentRoom.SortedPlayers();
            foreach (var player in players)
            {
                SetPlayerNickname(player);
            }
        }

        private void ViewPlayerInfoPanels(int playerNum)
        {
            for(int i = 0; i < playerNum; i++)
            {
                panels[i].panel.SetActive(true);
            }
        }

        private void SetPlayerNickname(Player player)
        {
            panels[viewedCount].nickname.text = player.NickName;
            viewedCount++;
        }
    }
}

