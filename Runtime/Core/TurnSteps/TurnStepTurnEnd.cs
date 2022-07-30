using System;
using UnityEngine;

namespace Match3.Core.TurnSteps
{
    [Serializable]
    public class TurnStepTurnEnd : TurnStep
    {
        [SerializeField] private bool _countAsTurn;

        public bool CountAsTurn => _countAsTurn;

        public TurnStepTurnEnd(bool countAsTurn)
        {
            _countAsTurn = countAsTurn;
        }
    }
}