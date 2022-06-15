using Games.Models;
using System;
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

        public static ColorType ConvertToColorType(Players players)
            => (ColorType)Enum.ToObject(typeof(ColorType), (int)players);

        public static Players ConvertToPlayers(ColorType colorType)
            => (Players)Enum.ToObject(typeof(Players), (int)colorType);

        public static Color ConvertToColor(ColorType colorType)
            => colorType switch
            {
                ColorType.Black => Color.black,
                ColorType.White => Color.white,
                ColorType.Red => Color.red,
                ColorType.Blue => Color.blue,
            };
    }
}

