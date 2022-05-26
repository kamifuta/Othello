using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using AI;

namespace Games.Views
{
    public class SettablePointsView : MonoBehaviour
    {
        [SerializeField] private Transform pointParent;
        [SerializeField] private Transform triggerParent;
        [SerializeField] private GameObject pointPrefab;
        [SerializeField] private GameObject clickTriggerPrefab;
        [SerializeField] private AIManager _AIManager;

        private List<GameObject> settablePointsObjList = new List<GameObject>();
        private List<GameObject> triggerObjList = new List<GameObject>();

        //�V�[����ɒu����ꏊ��\������֐�
        public void ViewSettablePoints(List<Vector3> pointsList)
        {
            if (_AIManager.CheckAITurn()) return;

            int pointLength = pointsList.Count;

            AssortPointObj(pointLength);
            AssortTriggerObj(pointLength);

            for(int i = 0; i < pointLength; i++)
            {
                settablePointsObjList[i].transform.localPosition = pointsList[i];
                triggerObjList[i].transform.localPosition = pointsList[i];

                settablePointsObjList[i].SetActive(true);
                triggerObjList[i].SetActive(true);
            }
        }

        //�v�[���ɑ���Ȃ������[����
        private void AssortPointObj(int length)
        {
            while (settablePointsObjList.Count < length)
            {
                var obj = Instantiate(pointPrefab, pointParent);
                settablePointsObjList.Add(obj);
                obj.SetActive(false);
            }
        }

        //�v�[���ɑ���Ȃ������[����
        private void AssortTriggerObj(int length)
        {
            while (triggerObjList.Count < length)
            {
                var obj = Instantiate(clickTriggerPrefab, triggerParent);
                triggerObjList.Add(obj);
                obj.SetActive(false);
            }
        }

        //�u����|�C���g�ƃN���b�N�p�g���K�[�������Ȃ�����
        public void InvidibleSettablePointObj()
        {
            int length = settablePointsObjList.Count;
            for(int i = 0; i < length; i++)
            {
                settablePointsObjList[i].SetActive(false);
                triggerObjList[i].SetActive(false);
            }
        }
    }
}
