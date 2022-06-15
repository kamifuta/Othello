using AI;
using Games.Models;
using Games.Utils;
using Games.Views;
using MyPhotons;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Games.Presenters
{
    public class SettablePointsPresenter : MonoBehaviour
    {
        [SerializeField] private SettablePointsView settablePointsView;
        [SerializeField] private ClickPointsView clickPointsView;
        [SerializeField] private PhotonView photonView;
        private IAITurnChecker _AITurnChecker;

        private Board board = Board.Instance;

        public void Init()
        {
            _AITurnChecker = FindObjectOfType<AIManager>();

            clickPointsView.Init();

            SettablePointChangeObservables();
            ClickedObservables();
        }

        private void SettablePointChangeObservables()
        {
            //�ݒu�\�Ȉʒu���ς�������ɂ��̈ʒu�ɃI�u�W�F�N�g��z�u������
            board.SettablePointsCollection
                .Skip(1)
                .Subscribe(x =>
                {
                    settablePointsView.InvidibleSettablePointObj();

                    if (_AITurnChecker.CheckAITurn()) return;
                    if (!PhotonNetwork.OfflineMode && !NetworkTurnManager.IsMyTurn()) return;

                    settablePointsView
                        .ViewSettablePoints(x.Select(v => Converter.ConvertToWorldPoint(v)).ToList());
                })
                .AddTo(this);
        }

        //�΂�u���ʒu���N���b�N�����Ƃ��̏���
        private void ClickedObservables()
        {
            clickPointsView.ClickedPointObservable
                .Subscribe(x =>
                {
                    settablePointsView.InvidibleSettablePointObj();

                    photonView.RPC(nameof(SendPutDiscs), RpcTarget.All, Converter.ConvertToModelPoint(x));
                })
                .AddTo(this);
        }

        [PunRPC]
        private void SendPutDiscs(Vector2Int putPoint)
        {
            board.PutDiscs(putPoint);
        }
    }
}

