using Games.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Games.Views;
using Games.Utils;
using System.Linq;

namespace Games.Presenters
{
    public class TurnPresenter : MonoBehaviour
    {
        [SerializeField] private UIManager _UIManager;

        private TurnManager turnManager = TurnManager.Instance;

        public void Init()
        {
            TurnCgangeObservables();
            PassObservables();
            ResultObservable();
        }

        private void TurnCgangeObservables()
        {
            turnManager.ChangeTurnObservable
                .Subscribe(x =>
                {
                    _UIManager.SetCurrentTurnText(GetPlayerName(x));
                    _UIManager.SetCurrentPlayerColor(Converter.ConvertToColor(EnumConverter.ConvertToColorType(turnManager.currentPlayer)));

                    _UIManager.UpdateDiscsCount(Referee.CountDiscs().Values.ToList());
                })
                .AddTo(this);
        }

        private void PassObservables()
        {
            turnManager.CurrentTurnPassObservable
                .Subscribe(_ =>
                {
                    _UIManager.ShowPassText(()=>turnManager.GoToNextTurn());
                })
                .AddTo(this);
        }

        private void ResultObservable()
        {
            Referee.ResultObservable
                .Subscribe(x =>
                {
                    _UIManager.ShowResult(GetPlayerName(x));
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

