using Games.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.Utils
{
    public static class Converter
    {
        //モデル上での座標をシーン上の座標に変換する
        public static Vector3 ConvertToWorldPoint(Vector2Int modelPoint)
            => new Vector3(modelPoint.x, 0, modelPoint.y);

        public static Vector2Int ConvertToModelPoint(Vector3 worldPoint)
            => new Vector2Int((int)worldPoint.x, (int)worldPoint.z);

        public static Color ConvertToColor(ColorType colorType)
        {
            switch (colorType)
            {
                case ColorType.Black:
                    return Color.black;
                case ColorType.White:
                    return Color.white;
                case ColorType.Red:
                    return Color.red;
                case ColorType.Blue:
                    return Color.blue;
                default:
                    Debug.LogError("ColorTypeがおかしい");
                    return Color.green;
            }
        }
    }
}

