using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace Titles
{
    public class OnlinePlaySettingManager : MonoBehaviour
    {
        [SerializeField] private Dropdown[] turnDropdownArray;

        private readonly string[] DropDownOptionArray = { "1st", "2nd", "3rd", "4th" };

        public void Init()
        {
            InitTurnDropdowns();
            DropDownValueChangedObservables();
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
            foreach(var x in turnDropdownArray)
            {
                var observable = x.OnValueChangedAsObservable();

                observable
                    .Zip(observable.Skip(1),(x,y)=> new {oldvalue=x, newValue=y })
                    .Subscribe(v =>
                    {
                        //Debug.Log("old:" + v.oldvalue + "new" + v.newValue);
                        var dropdown = turnDropdownArray.Where(s=>s.gameObject!=x.gameObject).FirstOrDefault(t => t.value == v.newValue);
                        //Debug.Log("aaa", dropdown.gameObject);
                        if (!dropdown) return;
                        dropdown.value = v.oldvalue;
                    })
                    .AddTo(this);
            }
        }
    }
}

