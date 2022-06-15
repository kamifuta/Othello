using Games;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titles
{
    public interface IPlaySetting
    {
        Players[] turnArray { get;}
        int allPlayerNum { get; }
        int playerNum { get;}
        int CPUNum { get; }
        Dictionary<Players, string> nicknameDic { get;}
        Players[] CPUArray { get; }

        void Destroy();
    }
}

