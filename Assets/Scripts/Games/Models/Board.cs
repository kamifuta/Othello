using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using System.Linq;
using Games.Models.ScriptableObjects;

namespace Games.Models
{
    public class Board
    {
        private static Board instance = new Board();
        public static Board Instance => instance;

        private ReverceManager reverceManager = new ReverceManager();

        public ColorType[,] discColorsArray { get; private set; }
        public int side { get; private set; } = 8;

        private ColorType currentColorType;
        
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
        private Subject<Unit> createdBoardSubject = new Subject<Unit>();
        public IObservable<Unit> CreatedBoardObservable => createdBoardSubject.AsObservable();

        //ディスクを設置可能な位置を保持するReactiveCollection
        private ReactiveProperty<List<Vector2Int>> settablePointsCollection=new ReactiveProperty<List<Vector2Int>>();
        public IReadOnlyReactiveProperty<List<Vector2Int>> SettablePointsCollection => settablePointsCollection;

        public void Init(int side, List<FirstDiscsInfo> infoList)
        {
            this.side = side;

            CreateBoard();
            ClearBoard();
            PutCenterDisc(infoList);
        }

        public void SetCurrentColorType(ColorType currentColorType)
        {
            this.currentColorType = currentColorType;
        }

        //ボードサイズを決定する
        private void CreateBoard()
        {
            discColorsArray = new ColorType[side, side];

            //createdBoardSubject.OnNext(side);
            createdBoardSubject.OnNext(Unit.Default);
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
        private void PutCenterDisc(List<FirstDiscsInfo> infoList)
        {
            foreach(var x in infoList)
            {
                PutDiscs(x.PutPoint, x.ColorType, isCenter: true);
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

        public bool SetSettablePoints()
        {
            settablePointsCollection.Value = SettableChecker.GetSettablePoints(discColorsArray, currentColorType);
            return settablePointsCollection.Value.Count != 0;
        }
    }
}
