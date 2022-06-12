using AI;
using Cysharp.Threading.Tasks;
using Games.Presenters;
using Games.Views;
using Photon.Pun;
using Photons;
using Titles;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Games.Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] BasePlaySetting offlineSetting;
        [SerializeField] BasePlaySetting onlineSetting;

        private void Start()
        {
            MyCustomType.Register();
            DontDestroyOnLoad(this);
        }

        public async void LoadGameScene()
        {
            if (PhotonNetwork.OfflineMode)
            {
                await SceneManager.LoadSceneAsync("GameScene");
                StartGame(offlineSetting);
            }
            else
            {
                PhotonNetwork.LoadLevel("GameScene");
                await UniTask.WaitUntil(() => SceneManager.GetActiveScene().name == "GameScene");
                StartGame(onlineSetting);
            }

            Destroy(onlineSetting.gameObject);
            Destroy(offlineSetting.gameObject);
        }

        private void StartGame(BasePlaySetting playSetting)
        {
            FindObjectOfType<CameraManager>().Init(playSetting.allPlayerNum);
            FindObjectOfType<DiscsPresenter>().Init(playSetting.allPlayerNum);
            FindObjectOfType<TurnPresenter>().Init();

            FindObjectOfType<GameUIView>().VisiblePlayerInfoPanels(playSetting.allPlayerNum);

            var token = this.GetCancellationTokenOnDestroy();
            FindObjectOfType<AIManager>().GenerateAI(playSetting.playerNum, playSetting.CPUNum, token);
        }

        public void BackTitle()
        {
            SceneManager.LoadScene("TitleScene");
            Destroy(this.gameObject);
        }
    }
}

