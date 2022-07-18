using UnityEngine;
using System;
using Match3.Core.Matches;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// Variable of type `Match3.Core.Matches.MatchGroups`. Inherits from `AtomVariable&lt;Match3.Core.Matches.MatchGroups, MatchGroupsPair, MatchGroupsEvent, MatchGroupsPairEvent, MatchGroupsMatchGroupsFunction&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-lush")]
    [CreateAssetMenu(menuName = "Unity Atoms/Variables/MatchGroups", fileName = "MatchGroupsVariable")]
    public sealed class MatchGroupsVariable : AtomVariable<Match3.Core.Matches.MatchGroups, MatchGroupsPair, MatchGroupsEvent, MatchGroupsPairEvent, MatchGroupsMatchGroupsFunction>
    {
        protected override bool ValueEquals(Match3.Core.Matches.MatchGroups other)
        {
            throw new NotImplementedException();
        }
    }
}
