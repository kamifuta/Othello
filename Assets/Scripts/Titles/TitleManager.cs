using Games.Managers;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Titles
{
    public class TitleManager : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private PlayerSettingManager playerSettingManager;
        [SerializeField] private MatchingManager matchingManager;

        [SerializeField] private Button playOfflineButton;
        [SerializeField] private Button playOnlineButton;

        [SerializeField] private Button startOfflineButton;
        [SerializeField] private Button startOnlineButton;

        [SerializeField] private Button TwoPlayerButton;
        [SerializeField] private Button ThreePlayerButton;
        [SerializeField] private Button FourPlayerButton;

        [SerializeField] private GameObject playerSettingPanel;
        [SerializeField] private GameObject chooseGameModePanel;
        [SerializeField] private GameObject lobyPanel;
        [SerializeField] private GameObject onlineSettingPanel;
        
        private void Start()
        {
            StartButtonObservable();
            JoinedRoomObservables();
            PlayerNumButtonObservables();
        }

        private void StartButtonObservable()
        {
            playOfflineButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    PhotonNetwork.OfflineMode = true;

                    playerSettingPanel.SetActive(true);
                    chooseGameModePanel.SetActive(false);
                })
                .AddTo(this);

            startOfflineButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    GameManager.SetAllPlayerNum(playerSettingManager.PlayerNum, playerSettingManager.ComCount);
                    SceneManager.LoadSceneAsync("GameScene");
                })
                .AddTo(this);

            playOnlineButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    PhotonNetwork.OfflineMode = false;

                    onlineSettingPanel.SetActive(true);
                    chooseGameModePanel.SetActive(false);
                    //matchingManager.StartMaching();
                })
                .AddTo(this);

            startOnlineButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    GameManager.SetAllPlayerNum(2,0);
                    PhotonNetwork.LoadLevel("GameScene");
                })
                .AddTo(this);
        }

        private void JoinedRoomObservables()
        {
            matchingManager.JoinedRoomObservable
                .Subscribe(_ =>
                {
                    lobyPanel.SetActive(true);
                    onlineSettingPanel.SetActive(false);

                    startOnlineButton.interactable = PhotonNetwork.IsMasterClient;
                })
                .AddTo(this);
        }

        private void PlayerNumButtonObservables()
        {
            TwoPlayerButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    matchingManager.StartMatching(2);
                })
                .AddTo(this);

            ThreePlayerButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    matchingManager.StartMatching(3);
                })
                .AddTo(this);

            FourPlayerButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    matchingManager.StartMatching(4);
                })
                .AddTo(this);
        }
    }
}

