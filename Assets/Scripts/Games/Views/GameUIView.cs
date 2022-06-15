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

        [SerializeField] private Button backTitleButton;
        private Action backTitleButtonActin;

        private CancellationToken token;

        public void Init(int allPlayerNum, List<string> nicknameList, Action backTitleButtonAction)
        {
            this.backTitleButtonActin = backTitleButtonAction;
            BackTitleButtonObservable();

            token = this.GetCancellationTokenOnDestroy();
            VisiblePlayerInfoPanels(allPlayerNum);
            ViewNickname(nicknameList);
        }

        //�v���C���[����K�v�Ȑ������\��
        private void VisiblePlayerInfoPanels(int allPlayerNum)
        {
            for(int i = 0; i < allPlayerNum; i++)
            {
                playerInfoPanelsArray[i].SetActive(true);
            }
        }

        //���ꂼ��̃j�b�N�l�[����\��
        private void ViewNickname(IReadOnlyList<string> nicknames)
        {
            var count = nicknames.Count;
            for(int i = 0; i < count; i++)
            {
                nicknameTextArray[i].text = nicknames[i];
            }
        }

        //���ݒN�̃^�[�����\������
        public void SetCurrentTurnText(string text)
        {
            turnText.text = text;
        }

        //���݂̃v���C���[�̐F��\������
        public void SetCurrentPlayerColor(Color color)
        {
            colorImage.color = color;
        }

        //���݂̐΂̐���\������
        public void UpdateDiscsCount(IReadOnlyList<int> countList)
        {
            var length = countList.Count;
            for(int i = 0; i < length; i++)
            {
                discCountTextArray[i].text = countList[i].ToString();
            }
        }

        //�p�X�������Ƃ�m�点��e�L�X�g
        public async void ShowPassText(Action callback)
        {
            passTextObj.gameObject.SetActive(true);

            await InvisiblePassTextAsync(token);

            callback();
        }

        //�N���b�N�����Ԍo�߂Ńp�X�e�L�X�g��������
        private async UniTask InvisiblePassTextAsync(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken:token);
            await UniTask.WhenAny(
                UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken:token),
                UniTask.WaitUntil(() => Input.GetMouseButtonDown(0), cancellationToken:token)
                );
            passTextObj.SetActive(false);
        }

        //���ʁi���ҁj�̕\��
        public async void ShowResult(string winnerName)
        {
            await UniTask.WaitUntil(() => !discsView.IsRevercing, cancellationToken:token);

            winnerText.text = winnerName;
            resultPanel.SetActive(true);
        }

        //�ŏ��ɖ߂�{�^�����������Ƃ��̏���
        private void BackTitleButtonObservable()
        {
            backTitleButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    backTitleButtonActin();
                })
                .AddTo(this);
        }
    }
}

