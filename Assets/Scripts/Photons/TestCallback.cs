using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class TestCallback : MonoBehaviourPunCallbacks
{

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        // カスタムプロパティが更新されたプレイヤーのプレイヤー名とIDをコンソールに出力する
        Debug.Log($"{targetPlayer.NickName}({targetPlayer.ActorNumber})");

        // 更新されたプレイヤーのカスタムプロパティのペアをコンソールに出力する
        foreach (var prop in changedProps)
        {
            Debug.Log($"{prop.Key}: {prop.Value}");
        }
    }
}
