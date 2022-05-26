using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.Views
{
    public class BoardView : MonoBehaviour
    {
        [SerializeField] private Transform boardParent;
        [SerializeField] private GameObject boardPartsPrefab;

        public void InstanceBoard(int side)
        {
            for (int i = 0; i < side; i++)
            {
                for (int j = 0; j < side; j++)
                {
                    var obj = Instantiate(boardPartsPrefab, boardParent);
                    obj.transform.localPosition = new Vector3(i, -0.5f, j);
                }
            }
        }
    }
}

