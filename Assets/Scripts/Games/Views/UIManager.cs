using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using System;
using UniRx;

namespace Games.Views
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private DiscsView discsView;

        [SerializeField] private Text turnText;
        [SerializeField] private Image colorImage;
        [SerializeField] private GameObject passTextObj;

        [SerializeField] private GameObject resultPanel;
        [SerializeField] private Text winnerText;

        [SerializeField] private Text[] discCountTextArray;

        private Subject<Unit> finishShowUISubject = new Subject<Unit>();
        public IObservable<Unit> FinishShowUIObservable => finishShowUISubject.AsObservable();

        private CancellationToken token;

        private void Start()
        {
            token = this.GetCancellationTokenOnDestroy();
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

