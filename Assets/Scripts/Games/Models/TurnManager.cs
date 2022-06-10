using System;
using System.Collections.Generic;
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

        private Players[] turnArray;
        private Players beforePlayer;
        public Players currentPlayer { get; private set; }
        private Players nextPlayer;

        private Board board = Board.Instance;
        private int playerNum;

        public void SetTurnList(Players[] turnArray)
        {
            this.turnArray = turnArray;
            playerNum = turnArray.Length;
            foreach(var x in turnArray)
            {
                Debug.Log(x);
            }
        }

        //�ŏ��̃^�[����ݒ肷��
        public void SetFirstTurn()
        {
            //this.playerNum = playerNum;
            Debug.Log("qqq");

            beforePlayer = Players.None;
            currentPlayer = turnArray[0];
            nextPlayer = turnArray[1];

            board.SetCurrentColorType(EnumConverter.ConvertToColorType(currentPlayer));
            board.SetSettablePoints();
            changeTurnSubject.OnNext(currentPlayer);
        }

        //���̃^�[���Ɉړ�����
        public void GoToNextTurn()
        {
            beforePlayer = currentPlayer;
            currentPlayer = nextPlayer;
            nextPlayer = (int)nextPlayer + 1 > playerNum ? turnArray[0] : nextPlayer + 1;

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

