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
        //���̃}�l�[�W���[
        [SerializeField] private OfflinePlaySettingManager offlinePlaySettingManager;
        [SerializeField] private OnlinePlaySettingManager onlinePlaySettingManager;
        [SerializeField] private MatchingManager matchingManager;

        //�I�t���C�����I�����C���������߂�{�^��
        [SerializeField] private Button chooseOfflineButton;
        [SerializeField] private Button chooseOnlineButton;

        //�Q�[���J�n�{�^��
        [SerializeField] private Button startOfflineButton;
        [SerializeField] private Button startOnlineButton;

        //�^�C�g����ʂ��\������p�l��
        [SerializeField] private GameObject chooseGameModePanel;
        [SerializeField] private GameObject offlineSettingPanel;
        [SerializeField] private GameObject matchingSettingPanel;
        [SerializeField] private GameObject onlineSettingPanel;

        private void Start()
        {
            ExchangePanelObservables();
            JoinedRoomObservables();
        }

        private void ExchangePanelObservables()
        {
            //�I�t���C���֘A�̃p�l���؂�ւ�
            chooseOfflineButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    PhotonNetwork.OfflineMode = true;

                    offlineSettingPanel.SetActive(true);
                    chooseGameModePanel.SetActive(false);
                })
                .AddTo(this);

            startOfflineButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    GameManager.SetAllPlayerNum(offlinePlaySettingManager.PlayerNum, offlinePlaySettingManager.ComCount);
                    SceneManager.LoadSceneAsync("GameScene");
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
                    onlinePlaySettingManager.SetRandomTurn();

                    GameManager.SetAllPlayerNum(PhotonNetwork.CurrentRoom.MaxPlayers, 0);
                    PhotonNetwork.LoadLevel("GameScene");
                })
                .AddTo(this);
        }

        private void JoinedRoomObservables()
        {
            matchingManager.JoinedRoomObservable
                .Subscribe(_ =>
                {
                    onlineSettingPanel.SetActive(true);
                    matchingSettingPanel.SetActive(false);

                    startOnlineButton.interactable = PhotonNetwork.IsMasterClient;
                })
                .AddTo(this);
        }

    }
}

