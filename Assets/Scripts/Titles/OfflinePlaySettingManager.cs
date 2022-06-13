using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using static UnityEngine.UI.Dropdown;
using System.Linq;
using System;
using Games;
using UniRx.Triggers;

namespace Titles
{
    public class OfflinePlaySettingManager : BasePlaySetting
    {
        private enum PlayerType
        {
            None=0,
            Player=1,
            CPU=2
        }

        [Serializable]
        private class PlayerInfoObject
        {
            public GameObject panel;
            public InputField nicknameInputField;
            public Dropdown dropdown;
            public Text playerOrCPUText;
            public Button rightButton;
            public Button leftButton;
        }

        private class PlayerInfo
        {
            public PlayerType playerType;
            public Players players;
            public PlayerInfoObject playerInfoObject;

            public PlayerInfo(Players players, PlayerInfoObject infoObject)
            {
                this.players = players;
                this.playerInfoObject = infoObject;
            }
        }

        private const int MaxPlayerNum = 4;

        [SerializeField] private PlayerInfoObject[] playerInfoObjectArray;
        private PlayerInfo[] playerInfoArray = new PlayerInfo[MaxPlayerNum];

        //インターフェースの実装
        override public Players[] turnArray { get; protected set; }
        override public int allPlayerNum => playerNum + CPUNum;
        override public int playerNum { get; protected set; }
        override public int CPUNum { get; protected set; }
        override public Dictionary<Players, string> nicknameDic { get; protected set; }
        override public Players[] CPUArray => playerInfoArray.Where(x => x.playerType == PlayerType.CPU).Select(y => y.players).ToArray();

        [SerializeField] private Toggle randamToggle;
        private System.Random random = new System.Random();

        private readonly OptionData[] DropdownOptionArray = { new OptionData("1st"), new OptionData("2st"), new OptionData("3st"), new OptionData("4st") };

        public void Init()
        {
            DontDestroyOnLoad(this.gameObject);

            InitPlayerInfos();

            SetDropdownOptions();
            DropDownValueChangedObservables();
            ChangePlayerTypeButtonClickedObservables();
            PlayerTypeChangeObservable();
            SetTurn();
            RandomTurnChangeObservables();

            this.ObserveEveryValueChanged(x=>x.allPlayerNum)
                .Subscribe(x =>
                {
                    AdjustDropdoenOption(x);
                })
                .AddTo(this);
        }

        //PlayerInfoクラスの初期化
        private void InitPlayerInfos()
        {
            for (int i = 0; i < MaxPlayerNum; i++)
            {
                playerInfoArray[i] = new PlayerInfo((Players)Enum.ToObject(typeof(Players), i + 1), playerInfoObjectArray[i]);
                if (i == 0)
                {
                    playerInfoArray[i].playerType = PlayerType.Player;
                    playerNum++;
                    ViewPlayerInfo(playerInfoObjectArray[i]);
                }
                else if (i == 1)
                {
                    playerInfoArray[i].playerType = PlayerType.CPU;
                    CPUNum++;
                    ViewCPUInfo(playerInfoObjectArray[i]);
                }
                else
                {
                    playerInfoArray[i].playerType = PlayerType.None;
                    ViewNonePlayerInfo(playerInfoObjectArray[i]);
                }
            }
        }

        //ドロップダウンのoptionを設定する
        private void SetDropdownOptions()
        {
            for(int i=0;i< playerInfoObjectArray.Length; i++)
            {
                playerInfoObjectArray[i].dropdown.options = DropdownOptionArray.ToList();
                playerInfoObjectArray[i].dropdown.value = i;
            }
        }

        //ドロップダウンのOptionを調整
        private void AdjustDropdoenOption(int optionCount)
        {
            foreach(var x in playerInfoObjectArray)
            {
                x.dropdown.options = DropdownOptionArray.Take(optionCount).ToList();
            }
        }

        //ターンの設定
        private void SetTurn()
        {
            turnArray = playerInfoArray.OrderBy(x => x.playerInfoObject.dropdown.value).Select(x => x.players).Take(allPlayerNum).ToArray();
        }

        //ランダムなターンの設定
        private void SetRandomTurn()
        {
            turnArray = turnArray.OrderBy(x => random.Next()).ToArray();
        }

        //トグルを切り替えた時にターンの設定ができないようにする
        private void RandomTurnChangeObservables()
        {
            randamToggle.OnValueChangedAsObservable()
                .Subscribe(x =>
                {
                    foreach (var infoObj in playerInfoObjectArray)
                    {
                        infoObj.dropdown.interactable = !x;
                    }

                    SetRandomTurn();
                })
                .AddTo(this);
        }

        //ドロップダウンの切り替え
        private void DropDownValueChangedObservables()
        {
            for (int i = 0; i < MaxPlayerNum; i++)
            {
                var observable = playerInfoObjectArray[i].dropdown.OnValueChangedAsObservable();
                var index = i;
                
                observable
                    .Zip(observable.Skip(1), (x, y) => new { oldvalue = x, newValue = y })
                    .Subscribe(v =>
                    {
                        //ドロップダウンの値の入れ替え
                        var notThisDropdownArray = playerInfoObjectArray.Where(x => x != playerInfoObjectArray[index]).Select(y => y.dropdown).Where(z=>z.gameObject.activeSelf); //自身を除くドロップダウンの配列
                        var changeDropdown = notThisDropdownArray.FirstOrDefault(x => x.value == v.newValue);
                        if (!changeDropdown) return;
                        changeDropdown.value = v.oldvalue;

                        SetTurn();
                    })
                    .AddTo(this);
            }
        }

        //ボタンを押したときにプレイヤーかCPUかを切り替え
        private void ChangePlayerTypeButtonClickedObservables()
        {
            foreach(var x in playerInfoArray)
            {
                x.playerInfoObject.rightButton.OnClickAsObservable()
                    .Subscribe(_ =>
                    {
                        if (x.playerType == PlayerType.CPU)
                        {
                            x.playerType = PlayerType.None;
                        }
                        else
                        {
                            x.playerType++;
                        }
                    })
                    .AddTo(this);

                x.playerInfoObject.leftButton.OnClickAsObservable()
                    .Subscribe(_ =>
                    {
                        if (x.playerType == PlayerType.None)
                        {
                            x.playerType = PlayerType.CPU;
                        }
                        else
                        {
                            x.playerType--;
                        }
                    })
                    .AddTo(this);
            }
        }

        //プレイヤータイプを変更したときの処理
        private void PlayerTypeChangeObservable()
        {
            foreach(var x in playerInfoArray)
            {
                var observable = x.ObserveEveryValueChanged(x => x.playerType);

                observable
                    .Zip(observable.Skip(1), (x, y) => new { oldValue = x, newValue = y })
                    .Subscribe(v =>
                    {
                        switch (v.oldValue)
                        {
                            case PlayerType.None:
                                break;
                            case PlayerType.Player:
                                playerNum--;
                                break;
                            case PlayerType.CPU:
                                CPUNum--;
                                break;
                        }

                        switch (v.newValue)
                        {
                            case PlayerType.None:
                                ViewNonePlayerInfo(x.playerInfoObject);
                                break;
                            case PlayerType.Player:
                                ViewPlayerInfo(x.playerInfoObject);
                                playerNum++;
                                break;
                            case PlayerType.CPU:
                                ViewCPUInfo(x.playerInfoObject);
                                CPUNum++;
                                break;
                        }
                    })
                    .AddTo(this);
            }
        }

        //プレイヤー「無し」の時のパネル表示
        private void ViewNonePlayerInfo(PlayerInfoObject info)
        {
            info.nicknameInputField.gameObject.SetActive(false);
            info.dropdown.gameObject.SetActive(false);

            info.playerOrCPUText.text = "なし";
        }

        //プレイヤー「プレイヤー」の時のパネル表示
        private void ViewPlayerInfo(PlayerInfoObject info)
        {
            info.nicknameInputField.gameObject.SetActive(true);
            info.dropdown.gameObject.SetActive(true);

            info.playerOrCPUText.text = "プレイヤー";
        }

        //プレイヤー「CPU」の時のパネル表示
        private void ViewCPUInfo(PlayerInfoObject info)
        {
            info.nicknameInputField.gameObject.SetActive(true);
            info.dropdown.gameObject.SetActive(true);

            info.playerOrCPUText.text = "CPU";
        }
    }
} 

