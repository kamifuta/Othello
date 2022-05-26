using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.Models
{
    public static class EnumConverter
    {
        public static ColorType ConvertToColorType(Players players)
            => (ColorType)Enum.ToObject(typeof(ColorType), (int)players);

        public static Players ConvertToPlayers(ColorType colorType)
            => (Players)Enum.ToObject(typeof(Players), (int)colorType);
    }
}

