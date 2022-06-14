using AI;
using Cysharp.Threading.Tasks;
using Games.Presenters;
using Games.Views;
using Photon.Pun;
using Photons;
using System.Collections.Generic;
using Titles;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Games.Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] BasePlaySetting offlineSetting;
        [SerializeField] BasePlaySetting onlineSetting;

        private Dictionary<Players, string> nicknameDic;

        private void Start()
        {
            MyCustomType.Register();
            DontDestroyOnLoad(this);
        }

        [PunRPC]
        public async void LoadGameScene()
        {
            if (PhotonNetwork.OfflineMode)
            {
                await SceneManager.LoadSceneAsync("GameScene");
                StartGame(offlineSetting);
                nicknameDic = offlineSetting.nicknameDic;
                Destroy(offlineSetting.gameObject);
            }
            else
            {
                PhotonNetwork.LoadLevel("GameScene");
                await UniTask.WaitUntil(() => SceneManager.GetActiveScene().name == "GameScene");
                StartGame(onlineSetting);
                nicknameDic = onlineSetting.nicknameDic;
                Destroy(onlineSetting.gameObject);
            }
        }

        private void StartGame(BasePlaySetting playSetting)
        {
            FindObjectOfType<CameraManager>().Init(playSetting.allPlayerNum);
            FindObjectOfType<DiscsPresenter>().Init(playSetting.allPlayerNum);
            FindObjectOfType<TurnPresenter>().Init(playSetting.turnArray);

            FindObjectOfType<GameUIView>().Init(playSetting.allPlayerNum, playSetting.nicknameDic);

            var token = this.GetCancellationTokenOnDestroy();
            var _AIManager = FindObjectOfType<AIManager>();
            foreach (var x in playSetting.CPUArray)
            {
                _AIManager.GenerateAI(x, token);
            }
        }

        public void BackTitle()
        {
            SceneManager.LoadScene("TitleScene");
            Destroy(this.gameObject);
        }
    }
}

