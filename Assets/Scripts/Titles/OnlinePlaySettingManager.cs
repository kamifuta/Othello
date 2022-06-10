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

        //プレイ人数を決めるボタン
        [SerializeField] private Button twoPlayerButton;
        [SerializeField] private Button threePlayerButton;
        [SerializeField] private Button fourPlayerButton;
        private ReactiveProperty<int> playerNum = new ReactiveProperty<int>();
        public IReadOnlyReactiveProperty<int> PlayerNum => playerNum;

        //プレイヤーの名前関連
        [SerializeField] private InputField nicknameinputField;
        private string inputNickname;

        //順番を決める関連
        [SerializeField] private Toggle randomTurnToggle;

        //プレイヤー情報を表示するパネル
        [SerializeField] private PlayerInfoObject[] playerInfoObjectArray;

        public bool IsRandomTurn { get; private set; } = true;

        private PlayerInfoObject[] activeInfoObjectArray;
        private int[] turnArray;
        private Random random = new Random();

        private readonly string[] DropDownOptionArray = { "1st", "2nd", "3rd", "4th" };

        //ルームに入る前
        public void StartRoomSetting()
        {
            PlayerButtonObservable();
            SetNickname();
        }

        //プレイヤー人数を決めるボタンを押したときの処理
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

        //入力したニックネームを適宜更新する
        private void SetNickname()
        {
            nicknameinputField.OnValueChangedAsObservable()
                .Subscribe(s => inputNickname = s)
                .AddTo(this);
        }

        //ルームに入ったあと
        public void StartPlayerSetting()
        {
            //ターン設定をデフォルトにする（入った順）
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

        //表示するパネルの枚数を決める
        private void ViewPlayerPanels()
        {
            activeInfoObjectArray = playerInfoObjectArray.Take(playerNum.Value).ToArray();
            foreach(var x in activeInfoObjectArray)
            {
                x.panel.SetActive(true);
            }
        }

        //表示パネルの位置を調整する
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
                for(int i = 0; i < playerNum.Value; i++)
                {
                    x.turnDropDown.options.Add(new OptionData(DropDownOptionArray[i]));
                }
            }
        }

        //ニックネームを表示する
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
            for (int i = 0; i < playerNum.Value; i++)
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
            for(int i = 0; i < playerNum.Value; i++)
            {
                activeInfoObjectArray[i].turnDropDown.value = turnArray[i];
            }
        }

        //順番の設定
        private void SetTurn()
        {
            turnArray = activeInfoObjectArray.Select(x => x.turnDropDown.value + 1).OrderBy(y => y).ToArray();
            PhotonNetwork.CurrentRoom.SetTurnArray(turnArray);
        }

        //順番をランダムにする
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

        //ルームプロパティの変更時
        public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
            UpdataTurnDropdowns();
        }

        //プレイヤーが入ってきたときの処理
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            ViewNickname();

            activeInfoObjectArray[PhotonNetwork.CurrentRoom.PlayerCount-1].turnDropDown.value = PhotonNetwork.CurrentRoom.PlayerCount - 1;
        }
    }
}

