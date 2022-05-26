using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Linq;
using UniRx;
using System;

namespace Games.Views
{
    public class DiscsView : MonoBehaviour
    {
        private enum DiscSurface
        {
            Head, Tail,
        }

        [SerializeField] private Transform discsParent;
        [SerializeField] private GameObject discPrefab;

        public bool IsRevercing { get; private set; }

        private const float rotateSpeed = 250f;
        private Vector3 rotateVec = new Vector3(rotateSpeed, 0, 0);

        //石のオブジェクトとマテリアルを紐づけるDictionary
        private Dictionary<GameObject, Material> materialDic = new Dictionary<GameObject, Material>();

        private Subject<Unit> allDiscsChangedSubject = new Subject<Unit>();
        public IObservable<Unit> AllDiscsChangeObservable => allDiscsChangedSubject.AsObservable();

        //石を生成して置く
        public GameObject PutDisc(Vector2 putPoint, Color color)
        {
            var disc = Instantiate(discPrefab, discsParent);
            disc.transform.localPosition = new Vector3(putPoint.x, 0.5f, putPoint.y);
            ChangeDiscColor(disc, color, DiscSurface.Head);
            return disc;
        }

        //石の見た目を変える
        public async UniTask ChangeDiscsAsync(Queue<List<GameObject>>queue, Color color, CancellationToken token)
        {
            IsRevercing = true;
            while (queue.Count > 0)
            {
                var taskList = new List<UniTask>();
                var discsList = queue.Dequeue();
                foreach (var x in discsList)
                {
                    ChangeDiscColor(x, color, DiscSurface.Tail);
                    taskList.Add(ReverceDisc(x,token));
                }
                await UniTask.WhenAll(taskList);
            }
            allDiscsChangedSubject.OnNext(Unit.Default);
            IsRevercing = false;
        }

        //石の色を変える
        private void ChangeDiscColor(GameObject targetDisc, Color color, DiscSurface surface)
        {
            Material material;
            if (materialDic.ContainsKey(targetDisc))
            {
                material = materialDic.FirstOrDefault(x => x.Key == targetDisc).Value;
            }
            else
            {
                material = targetDisc.GetComponent<Renderer>().material;
                materialDic.Add(targetDisc, material);
            }

            //表か裏かを判別するbool値
            bool isLookingUp = targetDisc.transform.up.y >= 0;
            string name;
            switch (surface)
            {
                case DiscSurface.Head:
                    name = isLookingUp ? "_Color1" : "_Color2";
                    break;
                case DiscSurface.Tail:
                    name = isLookingUp ? "_Color2" : "_Color1";
                    break;
                default:
                    name = "Color1";
                    Debug.LogError("表でも裏でもない");
                    break;
            }
            material.SetColor(name, color);
        }

        //石を反転させる
        private async UniTask ReverceDisc(GameObject targetDisc, CancellationToken token)
        {
            await PlayReverceAnimation(targetDisc);
        }

        //石を回転させるアニメーション
        private IEnumerator PlayReverceAnimation(GameObject targetDisc)
        {
            float rotatedAngle = 0f;
            while (true)
            {
                if (!targetDisc) break;
                rotateVec.x = rotateSpeed * Time.deltaTime;
                targetDisc.transform.Rotate(rotateVec, Space.Self);
                rotatedAngle += rotateVec.x;
                rotatedAngle = Mathf.Clamp(rotatedAngle, 0f, 180f);
                if (rotatedAngle>=180) break;
                yield return null;
            }
        }

        private void OnDestroy()
        {
            foreach(var x in materialDic.Values)
            {
                Destroy(x);
            }
            materialDic.Clear();
        }
    }
}

