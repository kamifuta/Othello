using Games.Models;
using Games.Views;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Games.Presenters
{
    public class BoardPresenter : MonoBehaviour
    {
        [SerializeField] private BoardView boardView;

        private Board board = Board.Instance;

        public void Init()
        {
            board.CreatedBoardObservable
                .Subscribe(_ => {
                    boardView.InstanceBoard(board.side);
                })
                .AddTo(this);
        }
    }
}

