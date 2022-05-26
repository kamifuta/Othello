using System;
using UniRx;
using UnityEngine;

namespace Games.Models
{
    public class TurnManager
    {
        private static TurnManager instance = new TurnManager();
        public static TurnManager Instance => instance;

        //ターンが変わったことを通知するSubject
        private Subject<Players> changeTurnSubject = new Subject<Players>();
        public IObservable<Players> ChangeTurnObservable => changeTurnSubject.AsObservable();

        //パスされたことを通知するSubject
        private Subject<Unit> currentTurnPassSubject = new Subject<Unit>();
        public IObservable<Unit> CurrentTurnPassObservable => currentTurnPassSubject.AsObservable();

        private Players beforePlayer;
        public Players currentPlayer { get; private set; }
        private Players nextPlayer;

        private Board board = Board.Instance;
        private int playerNum;

        //最初のターンを設定する
        public void SetFirstTurn(int playerNum)
        {
            this.playerNum = playerNum;

            beforePlayer = Players.None;
            currentPlayer = Players.Player1;
            nextPlayer = Players.Player2;

            board.SetCurrentColorType(EnumConverter.ConvertToColorType(currentPlayer));
            board.SetSettablePoints();
            changeTurnSubject.OnNext(currentPlayer);
        }

        //次のターンに移動する
        public void GoToNextTurn()
        {
            beforePlayer = currentPlayer;
            currentPlayer = nextPlayer;
            nextPlayer = (int)nextPlayer + 1 > playerNum ? Players.Player1 : nextPlayer + 1;

            board.SetCurrentColorType(EnumConverter.ConvertToColorType(currentPlayer));
            bool settable = board.SetSettablePoints();

            //石を置くことができないならパスするかゲームが終了するかを調べる
            if (!settable)
            {
                if (Referee.CheckFinishGame())
                {
                    Referee.RefereeWinner();
                    return;
                }
                PassCurrentTurn();
                return;
            }
            changeTurnSubject.OnNext(currentPlayer);
        }

        //パスする
        private void PassCurrentTurn()
        {
            Debug.Log("pass");

            currentTurnPassSubject.OnNext(Unit.Default);
        }
    }
}

