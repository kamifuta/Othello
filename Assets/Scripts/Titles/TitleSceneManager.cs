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
    public class TitleSceneManager : MonoBehaviour
    {
        //他のマネージャー
        [SerializeField] private OfflinePlaySettingManager offlinePlaySettingManager;
        [SerializeField] private OnlinePlaySettingManager onlinePlaySettingManager;
        [SerializeField] private MatchingManager matchingManager;
        [SerializeField] private GameManager gameManager;

        //オフラインかオンラインかを決めるボタン
        [SerializeField] private Button chooseOfflineButton;
        [SerializeField] private Button chooseOnlineButton;

        //ゲーム開始ボタン
        [SerializeField] private Button startOfflineButton;
        [SerializeField] private Button startOnlineButton;

        //タイトル画面を構成するパネル
        [SerializeField] private GameObject chooseGameModePanel;
        [SerializeField] private GameObject offlineSettingPanel;
        [SerializeField] private GameObject matchingSettingPanel;
        [SerializeField] private GameObject onlineSettingPanel;

        [SerializeField] private PhotonView photonView;
        [SerializeField] private Button[] backButtonArray;

        private void Start()
        {
            ExchangePanelObservables();
            BackButtonObservable();
            JoinedRoomObservables();
        }

        private void ExchangePanelObservables()
        {
            //オフライン関連のパネル切り替え
            chooseOfflineButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    PhotonNetwork.OfflineMode = true;

                    offlinePlaySettingManager.Init();

                    offlineSettingPanel.SetActive(true);
                    chooseGameModePanel.SetActive(false);
                })
                .AddTo(this);

            startOfflineButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    offlinePlaySettingManager.SetRandomTurn();
                    photonView.RPC(nameof(gameManager.LoadGameScene), RpcTarget.All);
                })
                .AddTo(this);

            chooseOnlineButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    PhotonNetwork.OfflineMode = false;

                    onlinePlaySettingManager.StartRoomSetting();

                    matchingSettingPanel.SetActive(true);
                    chooseGameModePanel.SetActive(false);
                })
                .AddTo(this);

            startOnlineButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    //onlinePlaySettingManager.SetTurn();

                    photonView.RPC(nameof(gameManager.LoadGameScene), RpcTarget.All);
                })
                .AddTo(this);
        }

        private void BackButtonObservable()
        {
            foreach(var x in backButtonArray)
            {
                x.OnClickAsObservable()
                    .Subscribe(_ =>
                    {
                        chooseGameModePanel.SetActive(true);
                        onlineSettingPanel.SetActive(false);
                        offlineSettingPanel.SetActive(false);
                        matchingSettingPanel.SetActive(false);
                    })
                    .AddTo(this);
            }
        }

        private void JoinedRoomObservables()
        {
            matchingManager.JoinedRoomObservable
                .Subscribe(_ =>
                {
                    if (PhotonNetwork.OfflineMode) return;

                    onlineSettingPanel.SetActive(true);
                    matchingSettingPanel.SetActive(false);

                    startOnlineButton.interactable = PhotonNetwork.IsMasterClient;
                })
                .AddTo(this);
        }

    }
}

