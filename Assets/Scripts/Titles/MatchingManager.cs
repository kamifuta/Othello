using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Titles
{
    public class MatchingManager : MonoBehaviourPunCallbacks
    {
        private Subject<Unit> joinedRoomSubject = new Subject<Unit>();
        public IObservable<Unit> JoinedRoomObservable => joinedRoomSubject.AsObservable();

        private int playerNum;

        public void StartMatching(int playerNum)
        {
            this.playerNum = playerNum;
            PhotonNetwork.ConnectUsingSettings();
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
            
            joinedRoomSubject.OnNext(Unit.Default);
            joinedRoomSubject.OnCompleted();
        }

        // ランダムで参加できるルームが存在しないなら、新規でルームを作成する
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            // ルームの参加人数を2人に設定する
            var roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 2;

            PhotonNetwork.CreateRoom(null, roomOptions);
        }
    }
}

