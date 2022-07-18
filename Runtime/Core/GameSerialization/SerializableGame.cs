using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Match3.Core.Levels;
using Match3.Core.TurnSteps;
using UnityEngine;

namespace Match3.Core.GameSerialization
{
    [Serializable]
    public class SerializableGame
    {
        [SerializeField] private Level _level;
        [SerializeField] private Board _firstBoardState;
        [SerializeReference, HideInInspector] private List<TurnStep> _turnSteps;

        public Level Level => _level;

        public Board FirstBoardState => _firstBoardState;

        public ReadOnlyCollection<TurnStep> TurnSteps => _turnSteps.AsReadOnly();

        public SerializableGame(Level level, Board firstState)
        {
            _level = level;
            _firstBoardState = firstState;
            _turnSteps = new List<TurnStep>();
        }

        public void AddTurnStep(TurnStep turnStep)
        {
            _turnSteps.Add(turnStep);
        }

        public override string ToString()
        {
            return $"{_level.name}: {_turnSteps.Count} turn steps";
        }
    }
}