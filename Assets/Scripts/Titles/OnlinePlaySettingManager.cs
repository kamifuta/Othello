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

namespace Titles
{
    public class OnlinePlaySettingManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private Dropdown[] turnDropdownArray;
        [SerializeField] private Toggle randomTurnToggle;

        private readonly string[] DropDownOptionArray = { "1st", "2nd", "3rd", "4th" };

        public void Init()
        {
            SetDropdownInteractable(false);
            randomTurnToggle.interactable = PhotonNetwork.IsMasterClient;

            InitTurnDropdowns();
            DropDownValueChangedObservables();

            randomTurnToggle.OnValueChangedAsObservable()
                .Where(_=>PhotonNetwork.IsMasterClient)
                .Subscribe(x =>
                {
                    SetDropdownInteractable(!x);
                    PhotonNetwork.CurrentRoom.SetRandomTurn(x);
                })
                .AddTo(this);
        }

        private void SetDropdownInteractable(bool value)
        {
            foreach (var dropdown in turnDropdownArray)
            {
                dropdown.interactable = value;
            }
        }

        private void InitTurnDropdowns()
        {
            var playerCount = PhotonNetwork.CurrentRoom.MaxPlayers;
            for(int i = 0; i < playerCount; i++)
            {
                turnDropdownArray[i].gameObject.SetActive(true);

                for(int j = 0; j < playerCount; j++)
                {
                    turnDropdownArray[i].options.Add(new Dropdown.OptionData(DropDownOptionArray[j]));
                }

                turnDropdownArray[i].value = i;
            }
        }

        private void DropDownValueChangedObservables()
        {
            for(int i = 0; i < turnDropdownArray.Length; i++)
            {
                var observable = turnDropdownArray[i].OnValueChangedAsObservable();
                var index = i;

                observable
                    .Zip(observable.Skip(1), (x, y) => new { oldvalue = x, newValue = y })
                    .Subscribe(v =>
                    {
                        var dropdown = turnDropdownArray.Where(s => s.gameObject != turnDropdownArray[index].gameObject).Where(s => s.IsActive()).FirstOrDefault(t => t.value == v.newValue);
                        if (!dropdown) return;
                        Debug.Log(dropdown);
                        dropdown.value = v.oldvalue;

                        var turnArray = turnDropdownArray.Where(s => s.IsActive()).OrderBy(y => y.value).Select(h => index).ToArray();
                        Debug.Log(turnArray[0]);
                        PhotonNetwork.CurrentRoom.SetTurnList(turnArray);
                    })
                    .AddTo(this);
            }

            //foreach(var x in turnDropdownArray)
            //{
            //    var observable = x.OnValueChangedAsObservable();

            //    observable
            //        .Zip(observable.Skip(1),(x,y)=> new {oldvalue=x, newValue=y })
            //        .Subscribe(v =>
            //        {
            //            var dropdown = turnDropdownArray.Where(s=>s.gameObject!=x.gameObject).FirstOrDefault(t => t.value == v.newValue);
            //            if (!dropdown) return;
            //            dropdown.value = v.oldvalue;

            //            var turnArray = turnDropdownArray.Where(s => s.IsActive()).OrderBy(y => y.value).Select(h => (Players)Enum.ToObject(typeof(Players),h.value)).ToArray();
            //            Debug.Log(turnArray.Length);
            //            //PhotonNetwork.CurrentRoom.SetTurnList(turnArray);
            //        })
            //        .AddTo(this);
            //}
        }

        public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
            //Debug.Log(propertiesThatChanged);
            if(propertiesThatChanged.Any(x=>(string)x.Key== RoomPropertiesExtension.RandomTurnKey))
            {
                randomTurnToggle.isOn = (bool)propertiesThatChanged[RoomPropertiesExtension.RandomTurnKey];
            }
            
        }
    }
}

