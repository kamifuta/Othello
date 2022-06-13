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
    public class OnlinePlaySettingManager : BasePlaySetting
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
        //private ReactiveProperty<int> playerNumProperty = new ReactiveProperty<int>();
        //public IReadOnlyReactiveProperty<int> PlayerNumProperty => playerNumProperty;

        //�v���C���[�̖��O�֘A
        [SerializeField] private InputField nicknameinputField;
        private string inputNickname;

        //���Ԃ����߂�֘A
        [SerializeField] private Toggle randomTurnToggle;

        //�v���C���[����\������p�l��
        [SerializeField] private PlayerInfoObject[] playerInfoObjectArray;

        //�C���^�[�t�F�[�X�̎���
        override public Players[] turnArray { get; protected set; }
        override public int allPlayerNum => playerNum;
        override public int playerNum { get; protected set; }
        override public int CPUNum { get; protected set; } = 0;
        override public Dictionary<Players, string> nicknameDic { get; protected set; } = new Dictionary<Players, string>();
        override public Players[] CPUArray { get; }

        public bool IsRandomTurn { get; private set; } = true;

        private PlayerInfoObject[] activeInfoObjectArray;
        private Dictionary<Players, Dropdown> turnDropdownDic = new Dictionary<Players, Dropdown>();
        private Random random = new Random();

        private readonly OptionData[] DropDownOptionArray = { new OptionData("1st"), new OptionData("2st"), new OptionData("3st"), new OptionData("4st") };

        //���[���ɓ���O
        public void StartRoomSetting()
        {
            DontDestroyOnLoad(this.gameObject);

            PlayerButtonObservable();
            SetMyNickname();
        }

        //�v���C���[�l�������߂�{�^�����������Ƃ��̏���
        private void PlayerButtonObservable()
        {
            twoPlayerButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    playerNum = 2;
                    PhotonNetwork.LocalPlayer.NickName = inputNickname;
                })
                .AddTo(this);

            threePlayerButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    playerNum = 3;
                    PhotonNetwork.LocalPlayer.NickName = inputNickname;
                })
                .AddTo(this);

            fourPlayerButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    playerNum = 4;
                    PhotonNetwork.LocalPlayer.NickName = inputNickname;
                })
                .AddTo(this);
        }

        //���͂����j�b�N�l�[����K�X�X�V����
        private void SetMyNickname()
        {
            nicknameinputField.OnValueChangedAsObservable()
                .Subscribe(s => inputNickname = s)
                .AddTo(this);
        }

        //���[���ɓ���������
        public void StartPlayerSetting()
        {
            //�^�[���ݒ���f�t�H���g�ɂ���i���������j
            turnArray = new Players[playerNum];
            for(int i = 0; i < playerNum; i++)
            {
                turnArray[i] = (Players)Enum.ToObject(typeof(Players), i + 1);
            }
            PhotonNetwork.CurrentRoom.SetTurnArray(turnArray.Select(x => (int)x).ToArray());

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
            activeInfoObjectArray = playerInfoObjectArray.Take(playerNum).ToArray();
            foreach(var x in activeInfoObjectArray)
            {
                x.panel.SetActive(true);
            }
        }

        //�\���p�l���̈ʒu�𒲐�����
        private void SetPanelPosition()
        {
            var startPositionX = playerNum switch
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
                for(int i = 0; i < playerNum; i++)
                {
                    x.turnDropDown.options.Add(DropDownOptionArray[i]);
                }
            }
        }

        //�j�b�N�l�[�����i�[����
        private void SetNicknames()
        {
            var playerList = PhotonNetwork.CurrentRoom.Players;

            for (int i = 0; i < playerNum; i++)
            {
                if (playerList.ContainsKey(i + 1))
                {
                    //activeInfoObjectArray[i].nicknameText.text = playerList[i + 1].NickName;
                    nicknameDic.Add(playerList[i + 1].GetPlayerType(), playerList[i + 1].NickName);
                }
            }
        }

        //�j�b�N�l�[����\������
        private void ViewNickname()
        {
            //var playerList = PhotonNetwork.CurrentRoom.Players;

            for (int i = 0; i < playerNum; i++)
            {
                if (i+1 <= nicknameDic.Count)
                {
                    activeInfoObjectArray[i].nicknameText.text = nicknameDic.FirstOrDefault(x => (int)x.Key == i + 1).Value;
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
                if (PhotonNetwork.CurrentRoom.PlayerCount == playerNum)
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
            for (int i = 0; i < playerNum; i++)
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
            for(int i = 0; i < playerNum; i++)
            {
                activeInfoObjectArray[i].turnDropDown.value = (int)turnArray[i]-1;
            }
        }

        //���Ԃ̐ݒ�
        private void SetTurn()
        {
            turnArray = turnDropdownDic.OrderBy(x => x.Value.value).ToDictionary(y => y.Key, z => z.Value).Keys.ToArray();
            PhotonNetwork.CurrentRoom.SetTurnArray(turnArray.Select(x=>(int)x).ToArray());
        }

        //���Ԃ������_���ɂ���
        public void SetRandomTurn()
        {
            if (!randomTurnToggle.isOn) return;

            turnArray = turnArray.OrderBy(x => random.Next()).ToArray();
            PhotonNetwork.CurrentRoom.SetTurnArray(turnArray.Select(x => (int)x).ToArray());
        }

        //���[���v���p�e�B�̕ύX��
        public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
            UpdataTurnDropdowns();
        }

        //�v���C���[�������Ă����Ƃ��̏���
        public void OnPlayerEnteredRoom(Player newPlayer)
        {
            SetNicknames();
            ViewNickname();

            var infoObject = activeInfoObjectArray[PhotonNetwork.CurrentRoom.PlayerCount - 1];

            turnDropdownDic.Add(newPlayer.GetPlayerType(), infoObject.turnDropDown);
            infoObject.turnDropDown.value = PhotonNetwork.CurrentRoom.PlayerCount - 1;
        }
    }
}

