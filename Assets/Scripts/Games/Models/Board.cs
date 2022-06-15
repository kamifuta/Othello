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
        
        //�΂��u���ꂽ���Ƃ�ʒm����Subject
        private Subject<KeyValuePair<Vector2Int, ColorType>> putDiscsSubject = new Subject<KeyValuePair<Vector2Int, ColorType>>();
        public IObservable<KeyValuePair<Vector2Int, ColorType>> PutDiscObservable => putDiscsSubject.AsObservable();

        //�΂̐F���ς�������Ƃ�ʒm����Subject
        private Subject<List<Vector2Int>> discsChangeSubject = new Subject<List<Vector2Int>>();
        public IObservable<List<Vector2Int>> DiscsChangeObservable => discsChangeSubject.AsObservable();

        //���ׂĂ̐΂̐F��ς������Ƃ�ʒm����Subject
        private Subject<ColorType> allDiscsChangeSubject = new Subject<ColorType>();
        public IObservable<ColorType> AllDiscsChangeObservable => allDiscsChangeSubject.AsObservable();

        //�{�[�h�̍쐬�������������Ƃ�ʒm����Subject
        private Subject<Unit> createdBoardSubject = new Subject<Unit>();
        public IObservable<Unit> CreatedBoardObservable => createdBoardSubject.AsObservable();

        //�f�B�X�N��ݒu�\�Ȉʒu��ێ�����ReactiveCollection
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

        //�{�[�h�T�C�Y�����肷��
        private void CreateBoard()
        {
            discColorsArray = new ColorType[side, side];

            //createdBoardSubject.OnNext(side);
            createdBoardSubject.OnNext(Unit.Default);
        }

        //�{�[�h�̏��ЂÂ���
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

        //�����z�u�̃f�B�X�N��ݒu����
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
        /// �΂��u���ꂽ�Ƃ����̈ʒu�ƐF�̃y�A�𑗐M����
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
        /// �΂̐F���ς�������u�����ʒu�ɋ߂��΂��ƂɈʒu�ƐF�̃y�A�𑗂�B
        /// </summary>
        /// <param name="pointList"></param>
        /// <param name="colorType"></param>
        private void ChangeDiscsColor(List<List<Vector2Int>> allPointsList, ColorType colorType)
        {
            int k = 0;
            int maxCount = allPointsList.Select(x => x.Count).Max();
            while (k<maxCount)
            {
                //�������ʒu����߂����ɐF��ς���
                var list = allPointsList.Where(x => x.Count > k);
                var pointsList = new List<Vector2Int>();
                foreach(var x in list)
                {
                    ChangeDiscColor(x[k], colorType);
                    pointsList.Add(x[k]);
                }

                //��x�ɐF��ς���΂̈ʒu�̃��X�g�𑗂�
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
