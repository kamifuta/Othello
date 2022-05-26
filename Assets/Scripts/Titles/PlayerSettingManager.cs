using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace Titles
{
    public class PlayerSettingManager : MonoBehaviour
    {
        [SerializeField] private Button playerCountUpButton;
        [SerializeField] private Button playerCountDownButton;
        [SerializeField] private Text playerCountText;

        [SerializeField] private Button _COMCountUpButton;
        [SerializeField] private Button _COMCountDownButton;
        [SerializeField] private Text _COMCountText;

        private ReactiveProperty<int> playerNum = new ReactiveProperty<int>(2);
        private ReactiveProperty<int> _COMNum = new ReactiveProperty<int>(2);

        public int PlayerNum => playerNum.Value;
        public int ComCount => _COMNum.Value;

        // Start is called before the first frame update
        void Start()
        {
            PlayerCountObservables();
            COMCountObservables();
        }

        private void PlayerCountObservables()
        {
            playerCountUpButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    playerNum.Value++;
                })
                .AddTo(this);

            playerCountDownButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    playerNum.Value--;

                    _COMCountUpButton.interactable = true;
                })
                .AddTo(this);

            playerNum
                .Subscribe(x =>
                {
                    playerCountText.text = x.ToString();

                    if (x == 1)
                    {
                        playerCountUpButton.interactable = true;
                        playerCountDownButton.interactable = false;

                        _COMNum.Value = Mathf.Clamp(_COMNum.Value, 1, 3);
                    }
                    else if (x == 2)
                    {
                        playerCountUpButton.interactable = true;
                        playerCountDownButton.interactable = true;

                        _COMNum.Value = Mathf.Clamp(_COMNum.Value, 0, 2);
                    }
                    else if (x == 3)
                    {
                        playerCountUpButton.interactable = true;
                        playerCountDownButton.interactable = true;

                        _COMNum.Value = Mathf.Clamp(_COMNum.Value, 0, 1);
                    }
                    else if (x == 4)
                    {
                        playerCountUpButton.interactable = false;
                        playerCountDownButton.interactable = true;

                        _COMNum.Value = 0;
                    }
                    else
                    {
                        playerCountUpButton.interactable = true;
                        playerCountDownButton.interactable = true;
                    }
                })
                .AddTo(this);
        }

        private void COMCountObservables()
        {
            _COMCountUpButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    _COMNum.Value++;
                })
                .AddTo(this);

            _COMCountDownButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    _COMNum.Value--;
                })
                .AddTo(this);

            _COMNum
                .Subscribe(x =>
                {
                    _COMCountText.text = x.ToString();

                    if (x == 0)
                    {
                        _COMCountUpButton.interactable = true;
                        _COMCountDownButton.interactable = false;
                    }
                    else if (x == 3)
                    {
                        _COMCountUpButton.interactable = false;
                        _COMCountDownButton.interactable = true;
                    }
                    else
                    {
                        _COMCountUpButton.interactable = true;
                        _COMCountDownButton.interactable = true;
                    }

                    if (x >= 4 - playerNum.Value)
                    {
                        _COMNum.Value = 4 - playerNum.Value;
                        _COMCountUpButton.interactable = false;
                    }
                })
                .AddTo(this);
        }

    }
} 

