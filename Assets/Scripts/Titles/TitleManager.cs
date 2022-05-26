using Games.Managers;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Titles
{
    public class TitleManager : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private PlayerSettingManager playerSettingManager;


        //[SerializeField] private Button playWithComputerButton;
        //[SerializeField] private Button playWithFriendsButton;
        [SerializeField] private Button playOfflineButton;
        [SerializeField] private Button playOnlineButton;

        [SerializeField] private Button startOfflineButton;

        [SerializeField] private GameObject playerSettingPanel;
        [SerializeField] private GameObject chooseGameModePanel;

        //private ReactiveProperty<int> playerNum = new ReactiveProperty<int>(2);

        private void Start()
        {
            //PlayerNumChangeObservables();
            StartButtonObservable();
        }

        //private void PlayerNumChangeObservables()
        //{
        //    upButton.OnClickAsObservable()
        //        .Subscribe(_ =>
        //        {
        //            playerNum.Value++;
        //        })
        //        .AddTo(this);

        //    downButton.OnClickAsObservable()
        //        .Subscribe(_ =>
        //        {
        //            playerNum.Value--;
        //        })
        //        .AddTo(this);

        //    playerNum
        //        .Subscribe(x =>
        //        {
        //            playerNumText.text = x.ToString();

        //            if (x == 2)
        //            {
        //                upButton.interactable = true;
        //                downButton.interactable = false;
        //            }
        //            else if (x == 4)
        //            {
        //                upButton.interactable = false;
        //                downButton.interactable = true;
        //            }
        //            else
        //            {
        //                upButton.interactable = true;
        //                downButton.interactable = true;
        //            }
        //        })
        //        .AddTo(this);
        //}

        private void StartButtonObservable()
        {
            playOfflineButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    playerSettingPanel.SetActive(true);
                    chooseGameModePanel.SetActive(false);
                })
                .AddTo(this);

            startOfflineButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    //var allPlayerCount = playerSettingManager.PlayerNum + playerSettingManager.ComCount;
                    gameManager.StartGameOffline(playerSettingManager.PlayerNum, playerSettingManager.ComCount);
                })
                .AddTo(this);

            playOnlineButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    //var allPlayerCount = playerSettingManager.PlayerNum + playerSettingManager.ComCount;
                    gameManager.StartGameOnline(2);
                })
                .AddTo(this);
        }
    }
}

