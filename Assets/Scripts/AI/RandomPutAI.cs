using Cysharp.Threading.Tasks;
using Games;
using Games.Models;
using System.Threading;
using UnityEngine;
using System.Linq;

namespace AI
{
    public class RandomPutAI
    {
        private TurnManager turnManager = TurnManager.Instance;
        private Board board = Board.Instance;

        public RandomPutAI(Players me, CancellationToken token)
        {
            StartAIAsync(me, token).Forget();
        }

        private async UniTask StartAIAsync(Players me, CancellationToken token)
        {
            while (true)
            {
                await UniTask.WaitUntil(() => turnManager.currentPlayer == me, cancellationToken: token);

                var collection = board.SettablePointsCollection.Value;
                var count = collection.Count;
                if (count==0)
                {
                    await UniTask.WaitUntil(() => turnManager.currentPlayer != me, cancellationToken: token);
                    continue;
                }

                board.PutDiscs(collection.ElementAt(Random.Range(0, count)));
                await UniTask.WaitUntil(() => turnManager.currentPlayer != me, cancellationToken: token);
            }
        }
    }
}

