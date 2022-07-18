using UnityEngine;

namespace Match3.Core.Matches.Patterns
{
    public abstract class MatchPatternRecognizerBase : ScriptableObject
    {
        public abstract bool MeetsPattern(Match match);
    }
}