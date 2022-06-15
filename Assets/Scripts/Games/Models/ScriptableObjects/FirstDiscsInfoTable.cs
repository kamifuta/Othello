using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.Models.ScriptableObjects
{
    [Serializable]
    public class FirstDiscsInfoList
    {
        [SerializeField] private List<FirstDiscsInfo> infoList;

        public List<FirstDiscsInfo> InfoList => infoList;
    }

    [Serializable]
    public class FirstDiscsInfo
    {
        [SerializeField] private Vector2Int putPoint;
        [SerializeField] private ColorType colorType;

        public Vector2Int PutPoint => putPoint;
        public ColorType ColorType => colorType;
    }

    [CreateAssetMenu(menuName = "MyScriptable/Creata FisrtDiscsInfoTable")]
    public class FirstDiscsInfoTable : ScriptableObject
    {
        private int[] sideNumArray = new int[3] { 8, 9, 10 };
        [SerializeField] private List<FirstDiscsInfoList> firstDiscsInfoList;

        public Dictionary<int, List<FirstDiscsInfo>> FirstDiscsInfoDic => sideNumArray.Zip(firstDiscsInfoList, (x, y) => new { key = x, value = y.InfoList }).ToDictionary(v => v.key, v => v.value);
    }
}

