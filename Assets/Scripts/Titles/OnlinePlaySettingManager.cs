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

        //プレイ人数を決めるボタン
        [SerializeField] private Button twoPlayerButton;
        [SerializeField] private Button threePlayerButton;
        [SerializeField] private Button fourPlayerButton;
        //private ReactiveProperty<int> playerNumProperty = new ReactiveProperty<int>();
        //public IReadOnlyReactiveProperty<int> PlayerNumProperty => playerNumProperty;

        //プレイヤーの名前関連
        [SerializeField] private InputField nicknameinputField;
        private string inputNickname;

        //順番を決める関連
        [SerializeField] private Toggle randomTurnToggle;

        //プレイヤー情報を表示するパネル
        [SerializeField] private PlayerInfoObject[] playerInfoObjectArray;

        //インターフェースの実装
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

        //ルームに入る前
        public void StartRoomSetting()
        {
            DontDestroyOnLoad(this.gameObject);

            PlayerButtonObservable();
            SetMyNickname();
        }

        //プレイヤー人数を決めるボタンを押したときの処理
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

        //入力したニックネームを適宜更新する
        private void SetMyNickname()
        {
            nicknameinputField.OnValueChangedAsObservable()
                .Subscribe(s => inputNickname = s)
                .AddTo(this);
        }

        //ルームに入ったあと
        public void StartPlayerSetting()
        {
            //ターン設定をデフォルトにする（入った順）
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

        //表示するパネルの枚数を決める
        private void ViewPlayerPanels()
        {
            activeInfoObjectArray = playerInfoObjectArray.Take(playerNum).ToArray();
            foreach(var x in activeInfoObjectArray)
            {
                x.panel.SetActive(true);
            }
        }

        //表示パネルの位置を調整する
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

        //ドロップダウンが使える状態か設定する
        private void SetUsableDoropdowns(bool usable)
        {
            foreach (var x in activeInfoObjectArray)
            {
                x.turnDropDown.interactable = usable;
            }
        }

        //ドロップダウンのOptionを設定する
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

        //ニックネームを格納する
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

        //ニックネームを表示する
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
                    //ぐるぐるを表示
                    activeInfoObjectArray[i].nicknameText.text = "";
                }
            }
        }

        //ランダムボタンの調整（プライベートなら押せる、公開は押せない）（共有）
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

        //ランダムボタンを押したときの処理
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

        //ドロップダウンの設定
        private void DropDownValueChangedObservables()
        {
            for (int i = 0; i < playerNum; i++)
            {
                var observable = playerInfoObjectArray[i].turnDropDown.OnValueChangedAsObservable();
                var index = i;
                var notThisDropdownArray = activeInfoObjectArray.Where(x => x != playerInfoObjectArray[i]).Select(y => y.turnDropDown); //自身を除くドロップダウンの配列

                observable
                    .Skip(1)
                    .Zip(observable.Skip(2), (x, y) => new { oldvalue = x, newValue = y })
                    .Subscribe(v =>
                    {
                        //ドロップダウンの値の入れ替え
                        var changeDropdown = notThisDropdownArray.FirstOrDefault(x => x.value == v.newValue);
                        if (!changeDropdown) return;
                        changeDropdown.value = v.oldvalue;

                        SetTurn();
                    })
                    .AddTo(this);
            }
        }

        //ドロップダウン設定の共有
        private void UpdataTurnDropdowns()
        {
            if (PhotonNetwork.IsMasterClient) return;

            var turnArray = PhotonNetwork.CurrentRoom.GetTurnArray();
            for(int i = 0; i < playerNum; i++)
            {
                activeInfoObjectArray[i].turnDropDown.value = (int)turnArray[i]-1;
            }
        }

        //順番の設定
        private void SetTurn()
        {
            turnArray = turnDropdownDic.OrderBy(x => x.Value.value).ToDictionary(y => y.Key, z => z.Value).Keys.ToArray();
            PhotonNetwork.CurrentRoom.SetTurnArray(turnArray.Select(x=>(int)x).ToArray());
        }

        //順番をランダムにする
        public void SetRandomTurn()
        {
            if (!randomTurnToggle.isOn) return;

            turnArray = turnArray.OrderBy(x => random.Next()).ToArray();
            PhotonNetwork.CurrentRoom.SetTurnArray(turnArray.Select(x => (int)x).ToArray());
        }

        //ルームプロパティの変更時
        public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
            UpdataTurnDropdowns();
        }

        //プレイヤーが入ってきたときの処理
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

