using System.Collections.Generic;
using UnityEngine;

namespace Match3.Core.Matches.Patterns
{
    [CreateAssetMenu(fileName = "MatchPatternComposite", menuName = "Facticus/Match3/MatchPatterns/Composite")]
    public class MatchPatternComposite : MatchPatternRecognizerBase
    {
        [SerializeField] private List<MatchPatternRecognizerBase> _recognizers;
        
        public override bool MeetsPattern(Match match)
        {
            foreach (var recognizer in _recognizers)
            {
                if (!recognizer.MeetsPattern(match))
                    return false;
            }

            return true;
        }

        public static MatchPatternComposite GetComposite(List<MatchPatternRecognizerBase> recognizers, string name="")
        {
            var composite = CreateInstance<MatchPatternComposite>();
            composite._recognizers = recognizers;
            composite.name = name;
            return composite;
        }
    }
}