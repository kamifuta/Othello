using AI;
using Cysharp.Threading.Tasks;
using Games.Models;
using Games.Presenters;
using Games.Views;
using Photon.Pun;
using MyPhotons;
using System.Collections.Generic;
using Titles;
using UnityEngine;
using UnityEngine.SceneManagement;
using Games.Models.ScriptableObjects;
using System.Linq;

namespace Games.Managers
{
    public class GameManager : MonoBehaviour
    {
        //[SerializeField] BasePlaySetting offlineSetting;
        //[SerializeField] BasePlaySetting onlineSetting;

        [SerializeField] FirstDiscsInfoTable firstDiscsInfoTable;

        private void Start()
        {
            MyCustomType.Register();
            DontDestroyOnLoad(this);
        }

        [PunRPC]
        public async void LoadGameScene(IPlaySetting playSetting)
        {
            if (PhotonNetwork.OfflineMode)
            {
                await SceneManager.LoadSceneAsync("GameScene");
            }
            else
            {
                PhotonNetwork.LoadLevel("GameScene");
                await UniTask.WaitUntil(() => SceneManager.GetActiveScene().name == "GameScene");
            }

            StartGame(playSetting);
            playSetting.Destroy();
        }

        private void StartGame(IPlaySetting playSetting)
        {
            FindObjectOfType<CameraManager>().Init(playSetting.allPlayerNum);
            FindObjectOfType<BoardPresenter>().Init();
            FindObjectOfType<DiscsPresenter>().Init(playSetting.allPlayerNum, firstDiscsInfoTable);
            FindObjectOfType<SettablePointsPresenter>().Init();
            FindObjectOfType<TurnPresenter>().Init(playSetting.turnArray, playSetting.nicknameDic);

            FindObjectOfType<GameUIView>().Init(playSetting.allPlayerNum, playSetting.nicknameDic.Values.ToList(), BackTitle);

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

