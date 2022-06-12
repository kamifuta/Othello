using Games.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Games.Views;
using Games.Utils;
using System.Linq;
using Photon.Pun;
using Photons;
using System;

namespace Games.Presenters
{
    public class TurnPresenter : MonoBehaviour
    {
        [SerializeField] private GameUIView gameUIView;
        [SerializeField] private DiscsView discsView;

        private TurnManager turnManager = TurnManager.Instance;

        public void Init(Players[] turnArray)
        {
            TurnCgangeObservables();
            PassObservables();
            ResultObservable();

            //var turnList = PhotonNetwork.CurrentRoom.GetTurnArray().Select(x => (Players)Enum.ToObject(typeof(Players), x)).ToArray();
            
            turnManager.SetTurnList(turnArray);
            turnManager.SetFirstTurn();

            discsView.AllDiscsChangeObservable
                .Subscribe(_ =>
                {
                    turnManager.GoToNextTurn();
                })
                .AddTo(this);
        }

        private void TurnCgangeObservables()
        {
            turnManager.ChangeTurnObservable
                .Subscribe(x =>
                {
                    gameUIView.SetCurrentTurnText(GetPlayerName(x));
                    gameUIView.SetCurrentPlayerColor(Converter.ConvertToColor(EnumConverter.ConvertToColorType(turnManager.currentPlayer)));

                    gameUIView.UpdateDiscsCount(Referee.CountDiscs().Values.ToList());
                })
                .AddTo(this);
        }

        private void PassObservables()
        {
            turnManager.CurrentTurnPassObservable
                .Subscribe(_ =>
                {
                    gameUIView.ShowPassText(()=>turnManager.GoToNextTurn());
                })
                .AddTo(this);
        }

        private void ResultObservable()
        {
            Referee.ResultObservable
                .Subscribe(x =>
                {
                    gameUIView.ShowResult(GetPlayerName(x));
                })
                .AddTo(this);
        }

        private string GetPlayerName(Players player)
        {
            switch (player)
            {
                case Players.Player1:
                    return "Player1";
                case Players.Player2:
                    return "Player2";
                case Players.Player3:
                    return "Player3";
                case Players.Player4:
                    return "Player4";
                default:
                    return "None";
            }
        }
    }
}

