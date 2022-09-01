using System;
using System.Collections.Generic;
using Match3.Core.Matches;
using Match3.Core.Matches.Patterns;
using Match3.Core.TurnSteps;
using Match3.Core.TurnSteps.TokensDestructionSource;
using UnityEngine;
using Utils.Serializables;

namespace Match3.Core.GameDataExtraction.DataExtractors
{
    [Serializable]
    public class DictionaryMatchPatternCount : SerializableDictionary<MatchPatternRecognizerBase, int>{}
    
    [Serializable]
    public class MatchesPatternCountData : IExtractedData
    {
        [SerializeField] private int _totalMatches;
        [SerializeField] private DictionaryMatchPatternCount _patternsCount;

        public int TotalMatches => _totalMatches;

        public DictionaryMatchPatternCount PatternsCount
        {
            get
            {
                var copy = new DictionaryMatchPatternCount();
                copy.CopyFrom(_patternsCount);
                return copy;
            }
        }

        public MatchesPatternCountData(int totalMatches, DictionaryMatchPatternCount patternsCount)
        {
            _totalMatches = totalMatches;
            _patternsCount = patternsCount;
        }

        public IExtractedData GetClone()
        {
            var dictCopy = new DictionaryMatchPatternCount();
            dictCopy.CopyFrom(_patternsCount);
            return new MatchesPatternCountData(_totalMatches, dictCopy);
        }

        public void Aggregate(IExtractedData other)
        {
            if (other is MatchesPatternCountData otherMatches)
            {
                _totalMatches += otherMatches._totalMatches;
                foreach (var pair in otherMatches._patternsCount)
                {
                    var pattern = pair.Key;
                    var count = pair.Value;
                    if (_patternsCount.ContainsKey(pattern))
                    {
                        _patternsCount[pattern] += count;
                    }
                    else
                    {
                        _patternsCount.Add(pattern, count);
                    }
                }
            }
        }

        public void Clear()
        {
            _totalMatches = 0;
            _patternsCount.Clear();
        }
    }

    [Serializable]
    public class DataExtractorMatchesPatternCount : DataExtractorBase<TurnStepDamageTokens, MatchesPatternCountData>
    {
        [SerializeField] private List<MatchPatternRecognizerBase> _patternsToEvaluate;
        
        public DataExtractorMatchesPatternCount(List<MatchPatternRecognizerBase> patternsToEvaluate)
        {
            _patternsToEvaluate = patternsToEvaluate;
        }
        
        public DataExtractorMatchesPatternCount() : this(new List<MatchPatternRecognizerBase>())
        {
        }
        
        public override MatchesPatternCountData ExtractData(TurnStepDamageTokens turnStep)
        {
            int totalMatches = 0;
            var patternsCount = new DictionaryMatchPatternCount();
            foreach (var destruction in turnStep.TokensDamaged)
            {
                if (destruction.Source is DestructionSourceMatch matchSource)
                {
                    totalMatches++;
                    bool found = TryGetPatternFromMatch(matchSource.Match, out var pattern);
                    if (found)
                    {
                        IncrementCountForPattern(pattern, patternsCount);
                    }
                }
            }
            
            return new MatchesPatternCountData(totalMatches, patternsCount);
        }
        
        private bool TryGetPatternFromMatch(Match match, out MatchPatternRecognizerBase pattern)
        {
            foreach (var recognizer in _patternsToEvaluate)
            {
                if (recognizer.MeetsPattern(match))
                {
                    pattern = recognizer;
                    return true;
                }
            }

            pattern = null;
            return false;
        }
        
        private void IncrementCountForPattern(
            MatchPatternRecognizerBase pattern,
            DictionaryMatchPatternCount patternsCount)
        {
            if (patternsCount.ContainsKey(pattern))
            {
                patternsCount[pattern]++;
            }
            else
            {
                patternsCount.Add(pattern, 1);
            }
        }
    }
}