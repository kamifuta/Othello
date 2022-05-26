using AI;
using Cysharp.Threading.Tasks;
using Games.Presenters;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Games.Managers
{
    public enum GameMode
    {
        WithCPU,
        WithFriends,
        Online,
    }

    public class GameManager : MonoBehaviour
    {
        private DiscsPresenter discsPresenter;
        private TurnPresenter turnPresenter;

        private void Start()
        {
            DontDestroyOnLoad(this);
        }

        public async void StartGame(int playerNum, GameMode gameMode)
        {
            await SceneManager.LoadSceneAsync("GameScene");

            FindObjectOfType<CameraManager>().Init(playerNum);
            FindObjectOfType<TurnPresenter>().Init();
            FindObjectOfType<DiscsPresenter>().Init(playerNum);
            
            var token = this.GetCancellationTokenOnDestroy();

            switch (gameMode)
            {
                case GameMode.WithCPU:
                    FindObjectOfType<AIManager>().GenerateAI(playerNum, token);
                    break;
                case GameMode.WithFriends:
                    break;
                case GameMode.Online:
                    break;
            }
        }

        //private void GenerateAI(int playerNum)
        //{
        //    var token = this.GetCancellationTokenOnDestroy();

        //    new RandomPutAI(Players.Player2, token);
        //    if (playerNum == 2) return;
        //    new RandomPutAI(Players.Player3, token);
        //    if (playerNum == 3) return;
        //    new RandomPutAI(Players.Player4, token);
        //    if (playerNum == 4) return;
        //}

        public void BackTitle()
        {
            SceneManager.LoadScene("TitleScene");
            Destroy(this);
        }
    }
}

