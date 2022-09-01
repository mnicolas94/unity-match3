using System;
using System.Collections.Generic;
using Match3.Core.GameDataExtraction;
using Match3.Core.GameDataExtraction.DataExtractors;
using Match3.Core.TurnSteps;
using UnityEngine;

namespace Match3.Core.GameEndConditions.Defeat
{
    [Serializable]
    public class MovementsDefeatEvaluator : IDefeatEvaluator
    {
        [SerializeField] private int _movements;
        public int Movements => _movements;

        private int _turnCount;

        public void Initialize(GameController gameController)
        {
            _turnCount = 0;
        }

        public bool CheckDefeatInTurnStep(TurnStep turnStep)
        {
            if (turnStep is TurnStepTurnEnd endStep)
            {
                if (endStep.CountAsTurn)
                {
                    _turnCount++;
                }
            }
            return GetRemainingMovements() <= 0;
        }

        public int GetRemainingMovements()
        {
            int remaining = _movements - _turnCount;
            remaining = Math.Max(0, remaining);
            return remaining;
        }
    }
}