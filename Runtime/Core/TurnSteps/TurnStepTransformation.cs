using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Match3.Core.Matches;
using Match3.Core.Matches.Patterns;
using Match3.Core.SerializableTuples;
using UnityEngine;

namespace Match3.Core.TurnSteps
{
    [Serializable]
    public class TurnStepTransformations : TurnStep
    {
        [SerializeField] private List<Transformation> _transformations;

        public ReadOnlyCollection<Transformation> Transformations => _transformations.AsReadOnly();

        public TurnStepTransformations(List<Transformation> transformations)
        {
            _transformations = transformations;
        }
    }

    [Serializable]
    public class Transformation
    {
        [SerializeReference] private ITransformationSource _transformationSource;
        [SerializeField] private List<PositionToken> _transformedTokens;
        [SerializeField] private PositionToken _spawnedToken;

        public ITransformationSource TransformationSource => _transformationSource;

        public List<PositionToken> TransformedTokens => _transformedTokens;

        public PositionToken SpawnedToken => _spawnedToken;

        public Transformation(ITransformationSource transformationSource, List<PositionToken> transformedTokens, PositionToken spawnedToken)
        {
            _transformationSource = transformationSource;
            _transformedTokens = transformedTokens;
            _spawnedToken = spawnedToken;
        }
    }
    
    public interface ITransformationSource{}

    [Serializable]
    public class MatchTransformationSource : ITransformationSource
    {
        [SerializeField] private Match _match;
        [SerializeField] private MatchPatternRecognizerBase _patternRecognizer;

        public Match Match => _match;

        public MatchPatternRecognizerBase PatternRecognizer => _patternRecognizer;

        public MatchTransformationSource(Match match, MatchPatternRecognizerBase patternRecognizer)
        {
            _match = match;
            _patternRecognizer = patternRecognizer;
        }
    }
}