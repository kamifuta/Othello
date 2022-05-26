using Cysharp.Threading.Tasks;
using Games;
using Games.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace AI
{
    public class AIManager : MonoBehaviour
    {
        private TurnManager turnManager = TurnManager.Instance;

        private List<Players> _AIList = new List<Players>();
        public IReadOnlyList<Players> AIList => _AIList;

        public void GenerateAI(int playerNum, CancellationToken token)
        {
            //var token = this.GetCancellationTokenOnDestroy();

            new RandomPutAI(Players.Player2, token);
            _AIList.Add(Players.Player2);
            if (playerNum == 2) return;
            new RandomPutAI(Players.Player3, token);
            _AIList.Add(Players.Player3);
            if (playerNum == 3) return;
            new RandomPutAI(Players.Player4, token);
            _AIList.Add(Players.Player4);
            if (playerNum == 4) return;
        }

        public bool CheckAITurn()
        {
            return _AIList.Any(x => x == turnManager.currentPlayer);
        }
    }
}

