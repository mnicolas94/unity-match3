using System;
using System.Collections.Generic;
using Match3.Core.TurnSteps;
using UnityEngine;

namespace Match3.Core
{
    [Serializable]
    public class Turn
    {
        public static readonly Turn EmptyTurn = new Turn(false, new List<TurnStep>());
        
        [SerializeField] private bool _countAsTurn;
        [SerializeField] private IEnumerable<TurnStep> _turnSteps;

        public bool CountAsTurn => _countAsTurn;

        public IEnumerable<TurnStep> TurnSteps => _turnSteps;

        public Turn(bool countAsTurn, IEnumerable<TurnStep> turnSteps)
        {
            _countAsTurn = countAsTurn;
            _turnSteps = turnSteps;
        }

        public void TransformTurnSteps(Func<IEnumerable<TurnStep>, IEnumerable<TurnStep>> turnStepsTransformation)
        {
            _turnSteps = turnStepsTransformation(_turnSteps);
        }

        public void ForEachTurnStep(Action<TurnStep> turnStepProcessor)
        {
            IEnumerable<TurnStep> Process()
            {
                foreach (var turnStep in _turnSteps)
                {
                    turnStepProcessor(turnStep);
                    yield return turnStep;
                }
            }

            _turnSteps = Process();
        }
    }
}