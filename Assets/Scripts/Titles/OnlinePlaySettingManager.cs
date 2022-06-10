using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Photons;
using Games;
using System;
using Photon.Realtime;
using static UnityEngine.UI.Dropdown;
using Random = System.Random;

namespace Titles
{
    public class OnlinePlaySettingManager : MonoBehaviourPunCallbacks
    {
        [Serializable]
        private class PlayerInfoObject
        {
            public GameObject panel;
            public Text nicknameText;
            public Dropdown turnDropDown;
        }

        //�v���C�l�������߂�{�^��
        [SerializeField] private Button twoPlayerButton;
        [SerializeField] private Button threePlayerButton;
        [SerializeField] private Button fourPlayerButton;
        private ReactiveProperty<int> playerNum = new ReactiveProperty<int>();
        public IReadOnlyReactiveProperty<int> PlayerNum => playerNum;

        //�v���C���[�̖��O�֘A
        [SerializeField] private InputField nicknameinputField;
        private string inputNickname;

        //���Ԃ����߂�֘A
        [SerializeField] private Toggle randomTurnToggle;

        //�v���C���[����\������p�l��
        [SerializeField] private PlayerInfoObject[] playerInfoObjectArray;

        public bool IsRandomTurn { get; private set; } = true;

        private PlayerInfoObject[] activeInfoObjectArray;
        private int[] turnArray;
        private Random random = new Random();

        private readonly string[] DropDownOptionArray = { "1st", "2nd", "3rd", "4th" };

        //���[���ɓ���O
        public void StartRoomSetting()
        {
            PlayerButtonObservable();
            SetNickname();
        }

        //�v���C���[�l�������߂�{�^�����������Ƃ��̏���
        private void PlayerButtonObservable()
        {
            twoPlayerButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    playerNum.Value = 2;
                    PhotonNetwork.LocalPlayer.NickName = inputNickname;
                })
                .AddTo(this);

            threePlayerButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    playerNum.Value = 3;
                    PhotonNetwork.LocalPlayer.NickName = inputNickname;
                })
                .AddTo(this);

            fourPlayerButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    playerNum.Value = 4;
                    PhotonNetwork.LocalPlayer.NickName = inputNickname;
                })
                .AddTo(this);
        }

        //���͂����j�b�N�l�[����K�X�X�V����
        private void SetNickname()
        {
            nicknameinputField.OnValueChangedAsObservable()
                .Subscribe(s => inputNickname = s)
                .AddTo(this);
        }

        //���[���ɓ���������
        public void StartPlayerSetting()
        {
            //�^�[���ݒ���f�t�H���g�ɂ���i���������j
            turnArray = new int[playerNum.Value];
            for(int i = 0; i < playerNum.Value; i++)
            {
                turnArray[i] = i + 1;
            }
            PhotonNetwork.CurrentRoom.SetTurnArray(turnArray);

            ViewPlayerPanels();
            SetPanelPosition();
            SetUsableDoropdowns(false);
            SetDropdownOptions();
            ViewNickname();
            SetUsableRandomToggle();
            RandomToggleObservable();
            DropDownValueChangedObservables();
        }

        //�\������p�l���̖��������߂�
        private void ViewPlayerPanels()
        {
            activeInfoObjectArray = playerInfoObjectArray.Take(playerNum.Value).ToArray();
            foreach(var x in activeInfoObjectArray)
            {
                x.panel.SetActive(true);
            }
        }

        //�\���p�l���̈ʒu�𒲐�����
        private void SetPanelPosition()
        {
            var startPositionX = playerNum.Value switch
            {
                2 => -100,
                3 => -200,
                4 => -300,
                _=>0
            };

            foreach(var x in activeInfoObjectArray)
            {
                x.panel.transform.localPosition = new Vector3(startPositionX, 0, 0);
                startPositionX += 200;
            }
        }

        //�h���b�v�_�E�����g�����Ԃ��ݒ肷��
        private void SetUsableDoropdowns(bool usable)
        {
            foreach (var x in activeInfoObjectArray)
            {
                x.turnDropDown.interactable = usable;
            }
        }

        //�h���b�v�_�E����Option��ݒ肷��
        private void SetDropdownOptions()
        {
            foreach (var x in activeInfoObjectArray)
            {
                for(int i = 0; i < playerNum.Value; i++)
                {
                    x.turnDropDown.options.Add(new OptionData(DropDownOptionArray[i]));
                }
            }
        }

        //�j�b�N�l�[����\������
        private void ViewNickname()
        {
            var playerList = PhotonNetwork.CurrentRoom.Players;

            for (int i = 0; i < playerNum.Value; i++)
            {
                if (playerList.ContainsKey(i+1))
                {
                    activeInfoObjectArray[i].nicknameText.text = playerList[i+1].NickName;
                }
                else
                {
                    //���邮���\��
                    activeInfoObjectArray[i].nicknameText.text = "";
                }
            }
        }

        //�����_���{�^���̒����i�v���C�x�[�g�Ȃ牟����A���J�͉����Ȃ��j�i���L�j
        private void SetUsableRandomToggle()
        {
            if (PhotonNetwork.IsMasterClient && !PhotonNetwork.CurrentRoom.IsVisible)
            {
                if (PhotonNetwork.CurrentRoom.PlayerCount == playerNum.Value)
                {
                    randomTurnToggle.interactable = true;
                }
                else
                {
                    randomTurnToggle.interactable = false;
                }
            }
            else
            {
                randomTurnToggle.interactable = false;
            }
        }

        //�����_���{�^�����������Ƃ��̏���
        private void RandomToggleObservable()
        {
            randomTurnToggle.OnValueChangedAsObservable()
                .Subscribe(x =>
                {
                    IsRandomTurn = x;
                    SetUsableDoropdowns(!x);
                })
                .AddTo(this);
        }

        //�h���b�v�_�E���̐ݒ�
        private void DropDownValueChangedObservables()
        {
            for (int i = 0; i < playerNum.Value; i++)
            {
                var observable = playerInfoObjectArray[i].turnDropDown.OnValueChangedAsObservable();
                var index = i;
                var notThisDropdownArray = activeInfoObjectArray.Where(x => x != playerInfoObjectArray[i]).Select(y => y.turnDropDown); //���g�������h���b�v�_�E���̔z��

                observable
                    .Skip(1)
                    .Zip(observable.Skip(2), (x, y) => new { oldvalue = x, newValue = y })
                    .Subscribe(v =>
                    {
                        //�h���b�v�_�E���̒l�̓���ւ�
                        var changeDropdown = notThisDropdownArray.FirstOrDefault(x => x.value == v.newValue);
                        if (!changeDropdown) return;
                        changeDropdown.value = v.oldvalue;

                        SetTurn();
                    })
                    .AddTo(this);
            }
        }

        //�h���b�v�_�E���ݒ�̋��L
        private void UpdataTurnDropdowns()
        {
            if (PhotonNetwork.IsMasterClient) return;

            var turnArray = PhotonNetwork.CurrentRoom.GetTurnArray();
            for(int i = 0; i < playerNum.Value; i++)
            {
                activeInfoObjectArray[i].turnDropDown.value = turnArray[i];
            }
        }

        //���Ԃ̐ݒ�
        private void SetTurn()
        {
            turnArray = activeInfoObjectArray.Select(x => x.turnDropDown.value + 1).OrderBy(y => y).ToArray();
            PhotonNetwork.CurrentRoom.SetTurnArray(turnArray);
        }

        //���Ԃ������_���ɂ���
        public void SetRandomTurn()
        {
            if (!randomTurnToggle.isOn) return;

            turnArray = turnArray.OrderBy(x => random.Next()).ToArray();
            PhotonNetwork.CurrentRoom.SetTurnArray(turnArray);

            foreach (var x in turnArray)
            {
                Debug.Log("aaa"+x);
            }
        }

        //���[���v���p�e�B�̕ύX��
        public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
            UpdataTurnDropdowns();
        }

        //�v���C���[�������Ă����Ƃ��̏���
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            ViewNickname();

            activeInfoObjectArray[PhotonNetwork.CurrentRoom.PlayerCount-1].turnDropDown.value = PhotonNetwork.CurrentRoom.PlayerCount - 1;
        }
    }
}

