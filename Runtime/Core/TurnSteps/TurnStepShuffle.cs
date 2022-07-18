using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Match3.Core.SerializableTuples;
using UnityEngine;

namespace Match3.Core.TurnSteps
{
    [Serializable]
    public class TurnStepShuffle : TurnStep
    {
        [SerializeField] private List<TokenMovement> _shuffleMovements;

        public ReadOnlyCollection<TokenMovement> ShuffleMovements => _shuffleMovements.AsReadOnly();

        public TurnStepShuffle(List<TokenMovement> shuffleMovements)
        {
            _shuffleMovements = shuffleMovements;
        }
    }
}