using Games;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titles
{
    public interface IPlaySetting
    {
        public Players[] turnArray { get; }
        public int allPlayerNum { get; }
        public int playerNum { get; }
        public int CPUNum { get; }
        public Dictionary<Players, string> nicknameDic { get; }
    }
}

