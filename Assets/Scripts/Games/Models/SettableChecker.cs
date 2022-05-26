using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Games.Models
{
    public static class SettableChecker
    {
        //private int side;

        //public SettableChecker(int side)
        //{
        //    this.side = side;
        //}

        public static List<Vector2Int> GetSettablePoints(ColorType[,] discArray, ColorType colorType)
        {
            int side = (int)Mathf.Sqrt(discArray.Length);
            List<Vector2Int> returnList= new List<Vector2Int>();

            for(int i = 0; i < side; i++)
            {
                for(int j = 0; j < side; j++)
                {
                    if (discArray[i, j] != 0) continue;
                    if(CheckSettable(i,j,discArray,colorType)) returnList.Add(new Vector2Int(i, j));
                }
            }
            return returnList;
        }

        /// <summary>
        /// 指定した座標にcolorTypeの石を置けるかどうか確認する
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="discArray"></param>
        /// <param name="colorType"></param>
        /// <returns></returns>
        private static bool CheckSettable(int x, int y, ColorType[,] discArray, ColorType colorType)
        {
            int side = (int)Mathf.Sqrt(discArray.Length);
            for (int i = -1; i <= 1; i++)
            {
                if (x + i < 0) continue;
                if (x + i >= side) continue;
                for (int j = -1; j <= 1; j++)
                {
                    if (y + j < 0) continue;
                    if (y + j >= side) continue;
                    if (i == 0 && j == 0) continue;
                    if (discArray[x + i, y + j] == ColorType.None) continue;
                    if (discArray[x + i, y + j] == colorType) continue;

                    var h = x + i;
                    var v = y + j;

                    while (true)
                    {
                        h += i;
                        v += j;

                        if (h < 0 || v < 0) break;
                        if (h >= side || v >= side) break;

                        if (discArray[h,v] == ColorType.None) break;

                        if (discArray[h,v] == colorType) return true;
                    }
                }
            }
            return false;
        }
    }
}

