using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Games.Views;
using Games.Models;
using Games.Utils;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace Games.Presenters
{
    public class DiscsPresenter : MonoBehaviour
    {
        [SerializeField] private BoardView boardView;
        [SerializeField] private SettablePointsView settablePointsView;
        [SerializeField] private DiscsView discsView;
        [SerializeField] private ClickPointsManager clickPointsManager;

        private Board board = Board.Instance;
        private TurnManager turnManager = TurnManager.Instance;

        private Queue<List<GameObject>> changeDiscsQueue = new Queue<List<GameObject>>();
        private Dictionary<Vector2, GameObject> discsPointDic = new Dictionary<Vector2, GameObject>();

        public void Init(int playerNum)
        {
            board.CreatedBoardObservable
                .Subscribe(x => {
                    boardView.InstanceBoard(x);
                    DiscsChangeObservables();
                })
                .AddTo(this);

            
            board.Init(playerNum);
            CreateBoardObservables();
            ClickedPointObservables();
            ViewChangeObservables();

            turnManager.SetFirstTurn(playerNum);
            clickPointsManager.Init();
        }

        private void CreateBoardObservables()
        {
            //�ݒu�\�Ȉʒu���ς�������ɂ��̈ʒu�ɃI�u�W�F�N�g��z�u������
            board.SettablePointsCollection
                .Skip(1)
                .Subscribe(x =>
                {
                    settablePointsView.InvidibleSettablePointObj();
                    settablePointsView
                        .ViewSettablePoints(x.Select(v => Converter.ConvertToWorldPoint(v)).ToList());
                })
                .AddTo(this);
        }

        private void DiscsChangeObservables()
        {
            var token = this.GetCancellationTokenOnDestroy();
            

            //�΂��u���ꂽ�Ƃ����̈ʒu�ɐ΂𐶐�������
            board.PutDiscObservable
                .Where(v=> !discsPointDic.Any(x => x.Key == v.Key))
                .Subscribe(v =>
                {
                    var point = v.Key;
                    var color = Converter.ConvertToColor(v.Value);

                    var disc = discsView.PutDisc(point, color);
                    discsPointDic.Add(point, disc);
                })
                .AddTo(this);

            //�΂̐F���ς��Ƃ��΂𔽓]������
            board.DiscsChangeObservable
                .Subscribe(v =>
                {
                    var discsList = discsPointDic.Where(x => v.Any(y => y == x.Key))
                                        .ToDictionary(x => x.Key, y => y.Value).Values.ToList();
                    changeDiscsQueue.Enqueue(discsList);
                })
                .AddTo(this);

            board.AllDiscsChangeObservable
                .Subscribe(x =>
                {
                    var color = Converter.ConvertToColor(x);
                    discsView.ChangeDiscsAsync(changeDiscsQueue, color, token).Forget();
                })
                .AddTo(this);
        }

        private void ClickedPointObservables()
        {
            clickPointsManager.ClickedPointObservable
                .Subscribe(x =>
                {
                    settablePointsView.InvidibleSettablePointObj();
                    board.PutDiscs(Converter.ConvertToModelPoint(x));
                })
                .AddTo(this);
        }

        private void ViewChangeObservables()
        {
            //�΂��Ђ�����Ԃ��I��������̏���
            discsView.AllDiscsChangeObservable
                .Subscribe(_ =>
                {
                    turnManager.GoToNextTurn();
                    changeDiscsQueue.Clear();
                })
                .AddTo(this);
        }
    }
}
