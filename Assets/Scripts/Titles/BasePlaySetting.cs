using Games;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titles
{
    public abstract class BasePlaySetting : MonoBehaviourPunCallbacks
    {
        abstract public Players[] turnArray { get; protected set; }
        abstract public int allPlayerNum { get;}
        abstract public int playerNum { get; protected set; }
        abstract public int CPUNum { get; protected set; }
        abstract public Dictionary<Players, string> nicknameDic { get; protected set; }
    }
}

