using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using System;
using Games.Utils;

namespace Games.Models
{
    public static class Referee
    {
        private static Subject<Players> resultSubject = new Subject<Players>();
        public static IObservable<Players> ResultObservable => resultSubject.AsObservable();

        //�Q�[�����I�����邩�ǂ������ׂ�
        public static bool CheckFinishGame()
        {
            var discColorsArray = Board.Instance.discColorsArray.Cast<ColorType>();

            if (!discColorsArray.Any(x => x == ColorType.None)) return true;
            if (!discColorsArray.Any(x => x == ColorType.Black)) return true;
            if (!discColorsArray.Any(x => x == ColorType.White)) return true;
            return false;
        }

        //�����ҁi�����̐F�̐΂̐�����ԑ����l�j�𒲂ׂ�
        public static void RefereeWinner()
        {
            var countDic = CountDiscs();
            int max = countDic.Values.Max();
            Players winner=Converter.ConvertToPlayers(countDic.FirstOrDefault(x => x.Value == max).Key);

            resultSubject.OnNext(winner);
        }

        //���ꂼ��̐F�̐΂̐��𐔂���
        public static Dictionary<ColorType, int> CountDiscs()
        {
            var discColorsArray = Board.Instance.discColorsArray.Cast<ColorType>();
            Dictionary<ColorType, int> countDic = new Dictionary<ColorType, int>();

            int blackCount = discColorsArray.Where(x => x == ColorType.Black).Count();
            int whiteCount = discColorsArray.Where(x => x == ColorType.White).Count();
            int redCount = discColorsArray.Where(x => x == ColorType.Red).Count();
            int blueCount = discColorsArray.Where(x => x == ColorType.Blue).Count();

            countDic.Add(ColorType.Black, blackCount);
            countDic.Add(ColorType.White, whiteCount);
            countDic.Add(ColorType.Red, redCount);
            countDic.Add(ColorType.Blue, blueCount);

            return countDic;
        }
    }
}

