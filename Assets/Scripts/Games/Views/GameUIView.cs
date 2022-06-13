using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using System;
using UniRx;
using Games.Managers;
using System.Linq;

namespace Games.Views
{
    public class GameUIView : MonoBehaviour
    {
        [SerializeField] private DiscsView discsView;

        [SerializeField] private Text turnText;
        [SerializeField] private Image colorImage;
        [SerializeField] private GameObject passTextObj;

        [SerializeField] private GameObject resultPanel;
        [SerializeField] private Text winnerText;

        [SerializeField] private GameObject[] playerInfoPanelsArray;
        [SerializeField] private Text[] discCountTextArray;
        [SerializeField] private Text[] nicknameTextArray;

        private CancellationToken token;

        public void Init(int allPlayerNum, Dictionary<Players, string> nicknameDic)
        {
            token = this.GetCancellationTokenOnDestroy();
            VisiblePlayerInfoPanels(allPlayerNum);
            SetNickname(nicknameDic.Values.ToList());
        }

        //プレイヤー情報を必要な数だけ表示
        public void VisiblePlayerInfoPanels(int allPlayerNum)
        {
            for(int i = 0; i < allPlayerNum; i++)
            {
                playerInfoPanelsArray[i].SetActive(true);
            }
        }

        public void SetNickname(IReadOnlyList<string> nicknames)
        {
            var count = nicknames.Count;
            for(int i = 0; i < count; i++)
            {
                nicknameTextArray[i].text = nicknames[i];
                Debug.Log(nicknames[i]);
            }
        }

        public void SetCurrentTurnText(string text)
        {
            turnText.text = text;
        }

        public void SetCurrentPlayerColor(Color color)
        {
            colorImage.color = color;
        }

        public void UpdateDiscsCount(IReadOnlyList<int> countList)
        {
            var length = countList.Count;
            for(int i = 0; i < length; i++)
            {
                discCountTextArray[i].text = countList[i].ToString();
            }
        }

        public async void ShowPassText(Action callback)
        {
            passTextObj.gameObject.SetActive(true);

            await InvisiblePassTextAsync(token);

            callback();
        }

        private async UniTask InvisiblePassTextAsync(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken:token);
            await UniTask.WhenAny(
                UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken:token),
                UniTask.WaitUntil(() => Input.GetMouseButtonDown(0), cancellationToken:token)
                );
            passTextObj.SetActive(false);
        }

        public async void ShowResult(string winnerName)
        {
            await UniTask.WaitUntil(() => !discsView.IsRevercing, cancellationToken:token);

            winnerText.text = winnerName;
            resultPanel.SetActive(true);
        }
    }
}

