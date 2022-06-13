using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Games.Models
{
    public class TurnManager
    {
        private static TurnManager instance = new TurnManager();
        public static TurnManager Instance => instance;

        //�^�[�����ς�������Ƃ�ʒm����Subject
        private Subject<Players> changeTurnSubject = new Subject<Players>();
        public IObservable<Players> ChangeTurnObservable => changeTurnSubject.AsObservable();

        //�p�X���ꂽ���Ƃ�ʒm����Subject
        private Subject<Unit> currentTurnPassSubject = new Subject<Unit>();
        public IObservable<Unit> CurrentTurnPassObservable => currentTurnPassSubject.AsObservable();

        private (Players, Players)[] turnArray;
        private Players beforePlayer;
        public Players currentPlayer { get; private set; }
        private Players nextPlayer;

        private Board board = Board.Instance;
        private int playerNum;

        public void SetTurnList(Players[] turnArray)
        {
            playerNum = turnArray.Length;
            this.turnArray = new (Players, Players)[playerNum];
            
            for (int i = 0; i < playerNum; i++)
            {
                if (i == playerNum - 1)
                {
                    this.turnArray[i] = (turnArray[i], turnArray[0]);
                    continue;
                }
                this.turnArray[i] = (turnArray[i], turnArray[i + 1]);
                Debug.Log(turnArray[i]);
            }
            
        }

        //�ŏ��̃^�[����ݒ肷��
        public void SetFirstTurn()
        {
            beforePlayer = Players.None;
            currentPlayer = turnArray[0].Item1;
            nextPlayer = turnArray[0].Item2;

            board.SetCurrentColorType(EnumConverter.ConvertToColorType(currentPlayer));
            board.SetSettablePoints();
            changeTurnSubject.OnNext(currentPlayer);
        }

        //���̃^�[���Ɉړ�����
        public void GoToNextTurn()
        {
            beforePlayer = currentPlayer;
            currentPlayer = nextPlayer;
            nextPlayer = turnArray.FirstOrDefault(x => x.Item1 == nextPlayer).Item2;

            board.SetCurrentColorType(EnumConverter.ConvertToColorType(currentPlayer));
            bool settable = board.SetSettablePoints();

            //�΂�u�����Ƃ��ł��Ȃ��Ȃ�p�X���邩�Q�[�����I�����邩�𒲂ׂ�
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

        //�p�X����
        private void PassCurrentTurn()
        {
            Debug.Log("pass");

            currentTurnPassSubject.OnNext(Unit.Default);
        }
    }
}

