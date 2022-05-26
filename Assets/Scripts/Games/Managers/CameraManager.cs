using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.Managers
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;

        public void Init(int playerNum)
        {
            switch (playerNum)
            {
                case 2:
                    mainCamera.transform.position = new Vector3(3.5f, 10, 3.5f);
                    mainCamera.orthographicSize = 4.2f;
                    break;
                case 3:
                    mainCamera.transform.position = new Vector3(4f, 10, 4f);
                    mainCamera.orthographicSize = 4.7f;
                    break;
                case 4:
                    mainCamera.transform.position = new Vector3(4.5f, 10, 4.5f);
                    mainCamera.orthographicSize = 5.5f;
                    break;
            }
        }
    }
}

