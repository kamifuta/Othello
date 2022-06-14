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
        [SerializeField] private OnlinePlaySettingManager onlinePlaySettingManager;
        [SerializeField] private Toggle privateRoomToggle;
        [SerializeField] private InputField roomNameInputField;
        [SerializeField] private Text roomNameText;

        private Subject<Unit> joinedRoomSubject = new Subject<Unit>();
        public IObservable<Unit> JoinedRoomObservable => joinedRoomSubject.AsObservable();

        private int playerNum;
        private string inputRoomName;

        private void Start()
        {
            privateRoomToggle.OnValueChangedAsObservable()
                .Subscribe(x =>
                {
                    roomNameInputField.gameObject.SetActive(x);
                    roomNameText.gameObject.SetActive(x);
                })
                .AddTo(this);

            onlinePlaySettingManager.ObserveEveryValueChanged(x=>x.allPlayerNum)
                .Skip(1)
                .Subscribe(x =>
                {
                    StartMatching(x);
                })
                .AddTo(this);

            roomNameInputField.OnValueChangedAsObservable()
                .Subscribe(s => inputRoomName = s)
                .AddTo(this);
        }

        public void StartMatching(int playerNum)
        {
            this.playerNum = playerNum;
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            if (privateRoomToggle.isOn)
            {
                var roomOptions = new RoomOptions();
                roomOptions.MaxPlayers = Convert.ToByte(playerNum);
                roomOptions.IsVisible = !privateRoomToggle.isOn;

                PhotonNetwork.JoinOrCreateRoom(inputRoomName, roomOptions, TypedLobby.Default);
            }
            else
            {
                roomNameInputField.gameObject.SetActive(false);
                // �����_���ȃ��[���ɎQ������
                PhotonNetwork.JoinRandomRoom(null, Convert.ToByte(playerNum));
            }
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("�Q�����܂���"+PhotonNetwork.IsMasterClient);

            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.LocalPlayer.SetPlayerType(PhotonNetwork.CurrentRoom.PlayerCount);

            onlinePlaySettingManager.StartPlayerSetting();
            if (privateRoomToggle.isOn)
                roomNameText.text = "�������F" + inputRoomName;

            joinedRoomSubject.OnNext(Unit.Default);
            joinedRoomSubject.OnCompleted();
        }

        // �����_���ŎQ���ł��郋�[�������݂��Ȃ��Ȃ�A�V�K�Ń��[�����쐬����
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            var roomOptions = new RoomOptions();
            roomOptions.MaxPlayers =Convert.ToByte(playerNum);
            roomOptions.IsVisible = !privateRoomToggle.isOn;

            if (privateRoomToggle.isOn)
            {
                PhotonNetwork.CreateRoom(inputRoomName, roomOptions);
            }
            else
            {
                PhotonNetwork.CreateRoom(null, roomOptions);
            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            Debug.Log(targetPlayer.GetPlayerType());
        }
    }
}

