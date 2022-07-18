using UnityEngine;

namespace Match3.Core.Matches.Patterns
{
    [CreateAssetMenu(fileName = "MatchPatternTokenCount", menuName = "Facticus/Match3/MatchPatterns/Count")]
    public class MatchPatternTokenCount : MatchPatternRecognizerBase
    {
        [SerializeField] private int _count;
        
        public override bool MeetsPattern(Match match)
        {
            return match.Positions.Count >= _count;
        }

        public static MatchPatternTokenCount GetPatternRecognizer(int count, string name = "")
        {
            var recognizer = CreateInstance<MatchPatternTokenCount>();
            recognizer._count = count;
            recognizer.name = name;
            return recognizer;
        }
    }
}