using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    Queue<int> queue = new Queue<int>();
    // Start is called before the first frame update
    void Start()
    {
        queue.Enqueue(1);
        queue.Enqueue(2);
        queue.Enqueue(3);
        queue.Enqueue(4);
        queue.Enqueue(5);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            while (queue.Count > 0)
            {
                var n = queue.Dequeue();
                Debug.Log(n);
            }
        }
    }

}
