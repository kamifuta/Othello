using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.Models
{
    public class ReverceManager
    {
        /// <summary>
        /// 色を変える石の位置を取得する
        /// </summary>
        /// <param name="discsColorArray">盤上の石の色</param>
        /// <param name="putPoint">置いた位置</param>
        /// <param name="putColor">置いた色</param>
        /// <returns>色を変える石の位置</returns>
        public List<List<Vector2Int>> GetRevercePoints(ColorType[,] discsColorArray, Vector2Int putPoint, ColorType putColor)
        {
            List<Vector2Int> directionalList = new List<Vector2Int>();

            int x = putPoint.x;
            int y = putPoint.y;
            int side = (int)Mathf.Sqrt(discsColorArray.Length);

            //置いた位置に対して周囲の違う意思がある方向を取得する
            for(int i = -1; i <= 1; i++)
            {
                if (x + i < 0) continue;
                if (x + i >= side) continue;
                for (int j = -1; j <= 1; j++)
                {
                    if (y + j < 0) continue;
                    if (y + j >= side) continue;
                    if (i == 0 && j == 0) continue;
                    if (discsColorArray[x + i, y + j] == ColorType.None) continue;
                    if (discsColorArray[x + i, y + j] == putColor) continue;

                    directionalList.Add(new Vector2Int(i, j));
                }
            }

            var returnList = GetRevercePoints(discsColorArray, directionalList, putPoint, putColor, side);
            return returnList;
        }

        //石の色が変わるすべての位置を取得する
        private List<List<Vector2Int>> GetRevercePoints(ColorType[,] discsColorArray, List<Vector2Int> directionList, Vector2Int putPoint, ColorType putColor, int side)
        {
            int count = directionList.Count;
            List<List<Vector2Int>> returnList = new List<List<Vector2Int>>();
            for (int k = 0; k < count; k++)
            {
                int i = 0;
                bool found = false;
                List<Vector2Int> revercePointList = new List<Vector2Int>();
                while (true)
                {
                    i++;
                    var pos = putPoint + directionList[k] * i;

                    if (pos.x < 0) break;
                    if (pos.x >= side) break;

                    if (pos.y < 0) break;
                    if (pos.y >= side) break;

                    if (discsColorArray[pos.x, pos.y] == ColorType.None) break;

                    if(discsColorArray[pos.x, pos.y] != putColor)
                    {
                        revercePointList.Add(pos);
                    }

                    if (discsColorArray[pos.x, pos.y] == putColor)
                    {
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    returnList.Add(revercePointList);
                }
            }

            return returnList;
        }
    }
}

