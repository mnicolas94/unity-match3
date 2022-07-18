using System;
using UnityAtoms.BaseAtoms;
using Match3.Core.Matches;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// Reference of type `Match3.Core.Matches.MatchGroups`. Inherits from `AtomReference&lt;Match3.Core.Matches.MatchGroups, MatchGroupsPair, MatchGroupsConstant, MatchGroupsVariable, MatchGroupsEvent, MatchGroupsPairEvent, MatchGroupsMatchGroupsFunction, MatchGroupsVariableInstancer, AtomCollection, AtomList&gt;`.
    /// </summary>
    [Serializable]
    public sealed class MatchGroupsReference : AtomReference<
        Match3.Core.Matches.MatchGroups,
        MatchGroupsPair,
        MatchGroupsConstant,
        MatchGroupsVariable,
        MatchGroupsEvent,
        MatchGroupsPairEvent,
        MatchGroupsMatchGroupsFunction,
        MatchGroupsVariableInstancer>, IEquatable<MatchGroupsReference>
    {
        public MatchGroupsReference() : base() { }
        public MatchGroupsReference(Match3.Core.Matches.MatchGroups value) : base(value) { }
        public bool Equals(MatchGroupsReference other) { return base.Equals(other); }
        protected override bool ValueEquals(Match3.Core.Matches.MatchGroups other)
        {
            throw new NotImplementedException();
        }
    }
}
