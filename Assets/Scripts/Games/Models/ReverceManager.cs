using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.Models
{
    public class ReverceManager
    {
        /// <summary>
        /// �F��ς���΂̈ʒu���擾����
        /// </summary>
        /// <param name="discsColorArray">�Տ�̐΂̐F</param>
        /// <param name="putPoint">�u�����ʒu</param>
        /// <param name="putColor">�u�����F</param>
        /// <returns>�F��ς���΂̈ʒu</returns>
        public List<List<Vector2Int>> GetRevercePoints(ColorType[,] discsColorArray, Vector2Int putPoint, ColorType putColor)
        {
            List<Vector2Int> directionalList = new List<Vector2Int>();

            int x = putPoint.x;
            int y = putPoint.y;
            int side = (int)Mathf.Sqrt(discsColorArray.Length);

            //�u�����ʒu�ɑ΂��Ď��͂̈Ⴄ�ӎv������������擾����
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

        //�΂̐F���ς�邷�ׂĂ̈ʒu���擾����
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

