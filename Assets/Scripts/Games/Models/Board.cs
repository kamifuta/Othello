using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using System.Linq;

namespace Games.Models
{
    public class Board
    {
        private static Board instance = new Board();
        public static Board Instance => instance;

        public int side { get; private set; } = 10;

        public ColorType[,] discColorsArray { get; private set; }

        //石が置かれたことを通知するSubject
        private Subject<KeyValuePair<Vector2Int, ColorType>> putDiscsSubject = new Subject<KeyValuePair<Vector2Int, ColorType>>();
        public IObservable<KeyValuePair<Vector2Int, ColorType>> PutDiscObservable => putDiscsSubject.AsObservable();

        //石の色が変わったことを通知するSubject
        private Subject<List<Vector2Int>> discsChangeSubject = new Subject<List<Vector2Int>>();
        public IObservable<List<Vector2Int>> DiscsChangeObservable => discsChangeSubject.AsObservable();

        //すべての石の色を変えたことを通知するSubject
        private Subject<ColorType> allDiscsChangeSubject = new Subject<ColorType>();
        public IObservable<ColorType> AllDiscsChangeObservable => allDiscsChangeSubject.AsObservable();

        //ボードの作成が完了したことを通知するSubject
        private Subject<int> createdBoardSubject = new Subject<int>();
        public IObservable<int> CreatedBoardObservable => createdBoardSubject.AsObservable();

        //ディスクを設置可能な位置を保持するReactiveCollection
        private ReactiveProperty<List<Vector2Int>> settablePointsCollection=new ReactiveProperty<List<Vector2Int>>();
        public IReadOnlyReactiveProperty<List<Vector2Int>> SettablePointsCollection => settablePointsCollection;

        //private SettableChecker settableChecker;
        private ReverceManager reverceManager = new ReverceManager();

        private ColorType currentColorType;

        public void Init(int playerNum)
        {
            SetSide(playerNum);
            CreateBoard();
            ClearBoard();
            PutCenterDisc();
        }

        public void SetCurrentColorType(ColorType currentColorType)
        {
            this.currentColorType = currentColorType;
        }

        private void SetSide(int playerNum)
        {
            switch (playerNum)
            {
                case 2:
                    side = 8;
                    break;
                case 3:
                    side = 9;
                    break;
                case 4:
                    side = 10;
                    break;
                default:
                    Debug.LogError("人数がおかしい");
                    break;
            }
        }

        //NOTE: 二回目にやるときにおかしくなりそう
        private void CreateBoard()
        {
            discColorsArray = new ColorType[side, side];

            createdBoardSubject.OnNext(side);
            createdBoardSubject.OnCompleted();
        }

        //ボードの上を片づける
        public void ClearBoard()
        {
            for(int i = 0; i < side; i++)
            {
                for(int j = 0; j < side; j++)
                {
                    discColorsArray[i, j] = ColorType.None;
                }
            }
        }

        //初期配置のディスクを設置する
        private void PutCenterDisc()
        {
            switch (side)
            {
                case 8:
                    PutDiscs(new Vector2Int(3, 3),ColorType.Black, isCenter: true);
                    PutDiscs(new Vector2Int(4, 4),ColorType.Black, isCenter: true);

                    PutDiscs(new Vector2Int(3, 4),ColorType.White, isCenter: true);
                    PutDiscs(new Vector2Int(4, 3),ColorType.White, isCenter: true);
                    break;
                case 9:
                    PutDiscs(new Vector2Int(3, 3), ColorType.Black, isCenter: true);
                    PutDiscs(new Vector2Int(4, 4), ColorType.Black, isCenter: true);
                    PutDiscs(new Vector2Int(5, 5), ColorType.Black, isCenter: true);

                    PutDiscs(new Vector2Int(3, 4), ColorType.White, isCenter: true);
                    PutDiscs(new Vector2Int(4, 5), ColorType.White, isCenter: true);
                    PutDiscs(new Vector2Int(5, 3), ColorType.White, isCenter: true);

                    PutDiscs(new Vector2Int(3, 5), ColorType.Red, isCenter: true);
                    PutDiscs(new Vector2Int(4, 3), ColorType.Red, isCenter: true);
                    PutDiscs(new Vector2Int(5, 4), ColorType.Red, isCenter: true);
                    break;
                case 10:
                    PutDiscs(new Vector2Int(3, 3), ColorType.Black, isCenter: true);
                    PutDiscs(new Vector2Int(4, 4), ColorType.Black, isCenter: true);
                    PutDiscs(new Vector2Int(5, 5), ColorType.Black, isCenter: true);
                    PutDiscs(new Vector2Int(6, 6), ColorType.Black, isCenter: true);

                    PutDiscs(new Vector2Int(3, 4), ColorType.White, isCenter: true);
                    PutDiscs(new Vector2Int(4, 5), ColorType.White, isCenter: true);
                    PutDiscs(new Vector2Int(5, 6), ColorType.White, isCenter: true);
                    PutDiscs(new Vector2Int(6, 3), ColorType.White, isCenter: true);

                    PutDiscs(new Vector2Int(3, 5), ColorType.Red, isCenter: true);
                    PutDiscs(new Vector2Int(4, 6), ColorType.Red, isCenter: true);
                    PutDiscs(new Vector2Int(5, 3), ColorType.Red, isCenter: true);
                    PutDiscs(new Vector2Int(6, 4), ColorType.Red, isCenter: true);

                    PutDiscs(new Vector2Int(3, 6), ColorType.Blue, isCenter: true);
                    PutDiscs(new Vector2Int(4, 3), ColorType.Blue, isCenter: true);
                    PutDiscs(new Vector2Int(5, 4), ColorType.Blue, isCenter: true);
                    PutDiscs(new Vector2Int(6, 5), ColorType.Blue, isCenter: true);
                    break;
            }
        }

        public void PutDiscs(Vector2Int putPoint)
            => PutDiscs(putPoint, currentColorType);

        /// <summary>
        /// 石が置かれたときその位置と色のペアを送信する
        /// </summary>
        /// <param name="putPoint"></param>
        /// <param name="colorType"></param>
        /// <param name="isCenter"></param>
        private void PutDiscs(Vector2Int putPoint, ColorType colorType, bool isCenter = false)
        {
            discColorsArray[putPoint.x, putPoint.y] = colorType;
            KeyValuePair<Vector2Int, ColorType> pair = new KeyValuePair<Vector2Int, ColorType>(putPoint, colorType);
            if(!isCenter)
                ChangeDiscsColor(reverceManager.GetRevercePoints(discColorsArray, putPoint, colorType), colorType);
            putDiscsSubject.OnNext(pair);
        }

        /// <summary>
        /// 石の色が変わった時置いた位置に近い石ごとに位置と色のペアを送る。
        /// </summary>
        /// <param name="pointList"></param>
        /// <param name="colorType"></param>
        private void ChangeDiscsColor(List<List<Vector2Int>> allPointsList, ColorType colorType)
        {
            int k = 0;
            int maxCount = allPointsList.Select(x => x.Count).Max();
            while (k<maxCount)
            {
                //落ちた位置から近い順に色を変える
                var list = allPointsList.Where(x => x.Count > k);
                var pointsList = new List<Vector2Int>();
                foreach(var x in list)
                {
                    ChangeDiscColor(x[k], colorType);
                    pointsList.Add(x[k]);
                }

                //一度に色を変える石の位置のリストを送る
                discsChangeSubject.OnNext(pointsList);

                k++;
            }

            allDiscsChangeSubject.OnNext(colorType);
        }

        private void ChangeDiscColor(Vector2Int changePoint, ColorType colorType)
        {
            discColorsArray[changePoint.x, changePoint.y] = colorType;
        }

        public bool SetSettablePoints(bool isAITurn=false)
        {
            settablePointsCollection.Value = SettableChecker.GetSettablePoints(discColorsArray, currentColorType);
            return settablePointsCollection.Value.Count != 0;
        }
    }
}
