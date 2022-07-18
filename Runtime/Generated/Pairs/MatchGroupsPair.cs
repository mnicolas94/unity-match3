using System;
using UnityEngine;
using Match3.Core.Matches;
namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// IPair of type `&lt;Match3.Core.Matches.MatchGroups&gt;`. Inherits from `IPair&lt;Match3.Core.Matches.MatchGroups&gt;`.
    /// </summary>
    [Serializable]
    public struct MatchGroupsPair : IPair<Match3.Core.Matches.MatchGroups>
    {
        public Match3.Core.Matches.MatchGroups Item1 { get => _item1; set => _item1 = value; }
        public Match3.Core.Matches.MatchGroups Item2 { get => _item2; set => _item2 = value; }

        [SerializeField]
        private Match3.Core.Matches.MatchGroups _item1;
        [SerializeField]
        private Match3.Core.Matches.MatchGroups _item2;

        public void Deconstruct(out Match3.Core.Matches.MatchGroups item1, out Match3.Core.Matches.MatchGroups item2) { item1 = Item1; item2 = Item2; }
    }
}