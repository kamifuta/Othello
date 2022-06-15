using Cysharp.Threading.Tasks;
using Games;
using Games.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using System;

namespace AI
{
    public class AIManager : MonoBehaviour, IAITurnChecker
    {
        private TurnManager turnManager = TurnManager.Instance;

        private List<Players> _AIList = new List<Players>();
        public IReadOnlyList<Players> AIList => _AIList;

        public void GenerateAI(int playerNum, int AINum, CancellationToken token)
        {
            for(int i = 0; i < AINum; i++)
            {
                var player = (Players)Enum.ToObject(typeof(Players), playerNum + i + 1);
                new RandomPutAI(player, token);
                _AIList.Add(player);
            }
        }

        public void GenerateAI(Players players, CancellationToken token)
        {
            new RandomPutAI(players, token);
            _AIList.Add(players);
        }

        public bool CheckAITurn()
            => _AIList.Any(x => x == turnManager.currentPlayer);
    }
}

