using UnityEngine;

namespace Match3.Core.Matches.Patterns
{
    [CreateAssetMenu(fileName = "MatchPatternCross", menuName = "Match3/MatchPatterns/Cross")]
    public class MatchPatternCross : MatchPatternRecognizerBase
    {
        public override bool MeetsPattern(Match match)
        {
            return match.Intersections >= 1;
        }
    }
}