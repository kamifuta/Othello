using Games.Presenters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace Games.Views
{
    public class ClickPointsView : MonoBehaviour
    {
        [SerializeField] private Camera camera;

        private bool clickedMouse => Input.GetMouseButtonDown(0);

        private Subject<Vector3> clickedPointSubject = new Subject<Vector3>();
        public IObservable<Vector3> ClickedPointObservable => clickedPointSubject.AsObservable();

        private const int boardLayermask = 1 << 6;

        public void Init()
        {
            ClickedObservables();
        }

        private void ClickedObservables()
        {
            this.ObserveEveryValueChanged(x => x.clickedMouse)
                .Where(x => x)
                .Subscribe(_ =>
                {
                    RaycastHit hit = new RaycastHit();
                    if (CheckClickedPoint(out hit))
                    {
                        clickedPointSubject.OnNext(hit.transform.position);
                    }
                })
                .AddTo(this);
        }

        private bool CheckClickedPoint(out RaycastHit hit)
        {
            var ray = camera.ScreenPointToRay(Input.mousePosition);
            return Physics.Raycast(ray, out hit, Mathf.Infinity, boardLayermask);
        }
    }
}

