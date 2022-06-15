using Games.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Games.Views;
using Games.Utils;
using System.Linq;
using Photon.Pun;
using MyPhotons;
using System;

namespace Games.Presenters
{
    public class TurnPresenter : MonoBehaviour
    {
        [SerializeField] private GameUIView gameUIView;
        [SerializeField] private DiscsView discsView;

        private TurnManager turnManager = TurnManager.Instance;

        private Dictionary<Players, string> nicknameDic;

        public void Init(Players[] turnArray, Dictionary<Players,string> nicknameDic)
        {
            this.nicknameDic = nicknameDic;

            TurnCgangeObservables();
            PassObservables();
            ResultObservable();

            turnManager.SetTurnList(turnArray);
            turnManager.SetFirstTurn();

            //���ׂĂ̐΂��Ђ�����Ԃ��I������玟�̃^�[���ɐi��
            discsView.AllDiscsChangeObservable
                .Subscribe(_ =>
                {
                    turnManager.GoToNextTurn();
                })
                .AddTo(this);
        }

        //�^�[�����؂�ւ�����Ƃ��̏���
        private void TurnCgangeObservables()
        {
            turnManager.ChangeTurnObservable
                .Subscribe(x =>
                {
                    gameUIView.SetCurrentTurnText(GetPlayerName(x));
                    gameUIView.SetCurrentPlayerColor(Converter.ConvertToColor(Converter.ConvertToColorType(turnManager.currentPlayer)));

                    gameUIView.UpdateDiscsCount(Referee.CountDiscs().Values.ToList());
                })
                .AddTo(this);
        }

        //�p�X�����Ƃ��̏���
        private void PassObservables()
        {
            turnManager.CurrentTurnPassObservable
                .Subscribe(_ =>
                {
                    gameUIView.ShowPassText(()=>turnManager.GoToNextTurn());
                })
                .AddTo(this);
        }

        //���s�����܂����Ƃ��̏���
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
            => nicknameDic.FirstOrDefault(x => x.Key == player).Value;
    }
}

