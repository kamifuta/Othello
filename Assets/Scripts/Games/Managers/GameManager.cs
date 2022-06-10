using AI;
using Cysharp.Threading.Tasks;
using Games.Presenters;
using Games.Views;
using Photon.Pun;
using Photons;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Games.Managers
{
    public class GameManager : MonoBehaviour
    {
        private static int playerNum=0;
        private static int COMNum=0;

        private void Start()
        {
            MyCustomType.Register();
            DontDestroyOnLoad(this);

            if (PhotonNetwork.OfflineMode)
            {
                StartGame(playerNum, COMNum);
            }
            else
            {
                StartGame(PhotonNetwork.CurrentRoom.MaxPlayers, 0);
            }
            
        }

        public static void SetAllPlayerNum(int playerNum, int COMNum)
        {
            GameManager.playerNum = playerNum;
            GameManager.COMNum = COMNum;
        }

        private void StartGame(int playerNum, int COMNum)
        {
            FindObjectOfType<CameraManager>().Init(playerNum + COMNum);
            FindObjectOfType<DiscsPresenter>().Init(playerNum + COMNum);
            FindObjectOfType<TurnPresenter>().Init();

            FindObjectOfType<GameUIView>().VisiblePlayerInfoPanels(playerNum + COMNum);

            var token = this.GetCancellationTokenOnDestroy();
            FindObjectOfType<AIManager>().GenerateAI(playerNum, COMNum, token);
        }

        public void BackTitle()
        {
            SceneManager.LoadScene("TitleScene");
            Destroy(this);
        }
    }
}

