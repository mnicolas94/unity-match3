using UnityEngine;
using System;
using Match3.Core;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// Variable of type `BoardCores.Core.GameController`. Inherits from `AtomVariable&lt;BoardCores.Core.GameController, GameControllerPair, GameControllerEvent, GameControllerPairEvent, GameControllerGameControllerFunction&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-lush")]
    [CreateAssetMenu(menuName = "Unity Atoms/Variables/GameController", fileName = "GameControllerVariable")]
    public sealed class GameControllerVariable : AtomVariable<GameController, GameControllerPair, GameControllerEvent, GameControllerPairEvent, GameControllerGameControllerFunction>
    {
        protected override bool ValueEquals(GameController other)
        {
            throw new NotImplementedException();
        }
    }
}
