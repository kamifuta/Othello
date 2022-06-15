using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Games.Views;
using Games.Models;
using Games.Utils;
using System.Linq;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Games.Models.ScriptableObjects;

namespace Games.Presenters
{
    public class DiscsPresenter : MonoBehaviour
    {
        [SerializeField] private DiscsView discsView;

        private Board board = Board.Instance;

        private Queue<List<GameObject>> changeDiscsQueue = new Queue<List<GameObject>>();
        private Dictionary<Vector2, GameObject> discsPointDic = new Dictionary<Vector2, GameObject>();

        public void Init(int playerNum, FirstDiscsInfoTable table)
        {
            board.CreatedBoardObservable
                .Subscribe(_ => {
                    DiscsChangeObservables();
                })
                .AddTo(this);

            var boardSide = GetBoardSide(playerNum);

            board.Init(boardSide, table.FirstDiscsInfoDic[boardSide]);
            AllDiscsChangeObservables();
        }

        //ボードの一片のマス数を決める
        private int GetBoardSide(int playerNum)
            => playerNum switch
                {
                    2 => 8,
                    3 => 9,
                    4 => 10,
                    _ => 0,
                };

        private void DiscsChangeObservables()
        {
            var token = this.GetCancellationTokenOnDestroy();
            
            //石が置かれたときその位置に石を生成させる
            board.PutDiscObservable
                .Where(v=> !discsPointDic.ContainsKey(v.Key))
                .Subscribe(v =>
                {
                    var point = v.Key;
                    var color = Converter.ConvertToColor(v.Value);

                    var disc = discsView.PutDisc(point, color);
                    discsPointDic.Add(point, disc);
                })
                .AddTo(this);

            //石の色が変わるとき石を反転させる
            board.DiscsChangeObservable
                .Subscribe(v =>
                {
                    var discsList = discsPointDic.Where(x => v.Any(y => y == x.Key))
                                        .ToDictionary(x => x.Key, y => y.Value).Values.ToList();
                    changeDiscsQueue.Enqueue(discsList);
                })
                .AddTo(this);

            //Model上ですべての石の色が変わったらViewを変える
            board.AllDiscsChangeObservable
                .Subscribe(x =>
                {
                    var color = Converter.ConvertToColor(x);
                    discsView.ChangeDiscsAsync(changeDiscsQueue, color, token).Forget();
                })
                .AddTo(this);
        }

        private void AllDiscsChangeObservables()
        {
            //石をひっくり返し終わった時の処理
            discsView.AllDiscsChangeObservable
                .Subscribe(_ =>
                {
                    changeDiscsQueue.Clear();
                })
                .AddTo(this);
        }
    }
}

