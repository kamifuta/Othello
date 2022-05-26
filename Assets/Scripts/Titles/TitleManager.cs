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

        [SerializeField] private Button upButton;
        [SerializeField] private Button downButton;
        [SerializeField] private Text playerNumText;

        [SerializeField] private Button playWithComputerButton;
        [SerializeField] private Button playWithFriendsButton;
        [SerializeField] private Button playOnlineButton;

        private ReactiveProperty<int> playerNum = new ReactiveProperty<int>(2);

        private void Start()
        {
            PlayerNumChangeObservables();
            StartButtonObservable();
        }

        private void PlayerNumChangeObservables()
        {
            upButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    playerNum.Value++;
                })
                .AddTo(this);

            downButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    playerNum.Value--;
                })
                .AddTo(this);

            playerNum
                .Subscribe(x =>
                {
                    playerNumText.text = x.ToString();

                    if (x == 2)
                    {
                        upButton.interactable = true;
                        downButton.interactable = false;
                    }
                    else if (x == 4)
                    {
                        upButton.interactable = false;
                        downButton.interactable = true;
                    }
                    else
                    {
                        upButton.interactable = true;
                        downButton.interactable = true;
                    }
                })
                .AddTo(this);
        }

        private void StartButtonObservable()
        {
            playWithComputerButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    gameManager.StartGame(playerNum.Value, GameMode.WithCPU);
                })
                .AddTo(this);

            playWithFriendsButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    gameManager.StartGame(playerNum.Value, GameMode.WithFriends);
                })
                .AddTo(this);

            playOnlineButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    gameManager.StartGame(playerNum.Value, GameMode.Online);
                })
                .AddTo(this);
        }
    }
}

