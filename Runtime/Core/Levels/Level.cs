using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Match3.Core.GameActions;
using Match3.Core.GameEndConditions.Defeat;
using Match3.Core.GameEndConditions.Victory;
using Match3.Core.SerializableTuples;
using Match3.Settings;
using UnityEngine;
using Utils.Attributes;
using Utils.Extensions;
using Validator;

namespace Match3.Core.Levels
{
    [Serializable]
    public class PositionTokenDataSerializableList
    {
        [SerializeField, ToStringLabel] private List<PositionTokenData> _positionTokens;

        public static implicit operator List<PositionTokenData>(PositionTokenDataSerializableList list)
        {
            return list._positionTokens;
        }
        
        public static implicit operator PositionTokenDataSerializableList(List<PositionTokenData> list)
        {
            var newList = new PositionTokenDataSerializableList();
            newList._positionTokens = list;
            return newList;
        }
    }
    
    [CreateAssetMenu(fileName = "Level", menuName = "Match3/Level")]
    public class Level : ScriptableObject, IHomologousTokenReplacer, IValidatable
    {
        [SerializeField] private BoardShape boardShape;
        
        [SerializeField, ToStringLabel, NaughtyAttributes.ValidateInput(
             nameof(ValidateTokensSourcesNotEmpty), "Tokens sources list should not be empty")]
        private List<PositionTokenSource> tokenSources;

        [Space, Header("- Valid Tokens")]
        [SerializeField, NaughtyAttributes.ValidateInput(nameof(ValidateTokensCreator))]
        private TokensCreationData _tokensCreationData;
        
        [Space, Header("- Initial state")]
        
        [SerializeField] private bool _fallTokensAtStart;
        [SerializeField, ToStringLabel]
         private List<PositionTokenData> initiallyPositionedTokens;
        [SerializeField]
         private List<PositionTokenDataSerializableList> bottomLayersTokens;
        [SerializeField]
         private List<PositionTokenDataSerializableList> topLayersTokens;
         [SerializeField]
         private List<Vector2Int> _prohibitedPositions;
        
        [Space, Header("- Game end conditions")]

        [SerializeReference, SubclassSelector, NaughtyAttributes.ValidateInput(
             nameof(ValidateVictoryNotNull), "Victory condition should not be null")]
        private IVictoryEvaluator _victoryEvaluator;
        
        [SerializeReference, SubclassSelector]
        [NaughtyAttributes.ValidateInput(nameof(ValidateDefeatNotNull), "Defeat condition should not be null")]
        private IDefeatEvaluator _defeatEvaluator;

        #region Properties
        
        public BoardShape BoardShape => boardShape;

        public TokensCreationData TokensCreationData => _tokensCreationData;

        public bool FallTokensAtStart => _fallTokensAtStart;

        public ReadOnlyCollection<PositionTokenData> InitiallyPositionedTokens => initiallyPositionedTokens.AsReadOnly();

        public IEnumerable<ReadOnlyCollection<PositionTokenData>> BottomLayersTokens
        {
            get
            {
                foreach (var layer in bottomLayersTokens)
                {
                    yield return ((List<PositionTokenData>) layer).AsReadOnly();
                }
            }
        }

        public IEnumerable<ReadOnlyCollection<PositionTokenData>> TopLayersTokens
        {
            get
            {
                foreach (var layer in topLayersTokens)
                {
                    yield return ((List<PositionTokenData>) layer).AsReadOnly();
                }
            }
        }

        public ReadOnlyCollection<Vector2Int> ProhibitedPositions => _prohibitedPositions.AsReadOnly();

        public ReadOnlyCollection<PositionTokenSource> TokenSources => tokenSources.AsReadOnly();

        public IVictoryEvaluator VictoryEvaluator => _victoryEvaluator;

        public IDefeatEvaluator DefeatEvaluator => _defeatEvaluator;
        
        #endregion
        
        public static void SetEditorData(
            Level level,
            BoardShape boardShape,
            List<PositionTokenData> initiallyPositionedTokens,
            List<PositionTokenDataSerializableList> bottomLayersTokens,
            List<PositionTokenDataSerializableList> topLayersTokens,
            List<PositionTokenSource> tokenSources,
            List<Vector2Int> prohibitedPositions)
        {
            level.boardShape = boardShape;
            level.initiallyPositionedTokens = initiallyPositionedTokens;
            level.bottomLayersTokens = bottomLayersTokens;
            level.topLayersTokens = topLayersTokens;
            level.tokenSources = tokenSources;
            level._prohibitedPositions = prohibitedPositions;
        }

        public IList<TokenData> GetAllTokens()
        {
            var tks = new List<TokenData>();
            tks.AddRange(_tokensCreationData.GetAllTokens());
            tks.AddRangeIfNotExists(initiallyPositionedTokens.ConvertAll(pt => pt.tokenData));
            tks.AddRangeIfNotExists(bottomLayersTokens
                .SelectMany(list => (List<PositionTokenData>)list)
                .Select(pt => pt.tokenData)
            );
            tks.AddRangeIfNotExists(topLayersTokens
                .SelectMany(list => (List<PositionTokenData>)list)
                .Select(pt => pt.tokenData)
            );

            return tks;
        }
        
        public void ReplaceToken(TokenData toReplace, TokenData replacement)
        {
            _tokensCreationData.ReplaceToken(toReplace, replacement);
            
            // initiallyPositionedTokens
            for (int i = 0; i < initiallyPositionedTokens.Count; i++)
            {
                var (position, token) = initiallyPositionedTokens[i];
                if (token == toReplace)
                    initiallyPositionedTokens[i] = new PositionTokenData
                    {
                        position = position,
                        tokenData = replacement
                    };
            }
            
            // bottomLayersTokens
            foreach (var bottomLayer in bottomLayersTokens)
            {
                var list = (List<PositionTokenData>) bottomLayer;
                for (int i = 0; i < list.Count; i++)
                {
                    var (position, token) = list[i];
                    if (token == toReplace)
                        list[i] = new PositionTokenData
                        {
                            position = position,
                            tokenData = replacement
                        };
                }
            }
            
            // topLayersTokens
            foreach (var topLayer in topLayersTokens)
            {
                var list = (List<PositionTokenData>) topLayer;
                for (int i = 0; i < list.Count; i++)
                {
                    var (position, token) = list[i];
                    if (token == toReplace)
                        list[i] = new PositionTokenData
                        {
                            position = position,
                            tokenData = replacement
                        };
                }
            }
            
            // _victoryEvaluator
            if (_victoryEvaluator is IHomologousTokenReplacer victoryReplacer)
            {
                victoryReplacer.ReplaceToken(toReplace, replacement);
            }
            
            // _defeatEvaluator
            if (_defeatEvaluator is IHomologousTokenReplacer defeatReplacer)
            {
                defeatReplacer.ReplaceToken(toReplace, replacement);
            }
        }

        public static float Similarity(Level a, Level b)
        {
            int matches = 0;
            int total = 0;
            
            // board shape
            var tilesA = a.boardShape.Tiles;
            var tilesB = b.boardShape.Tiles;
            var tokensA = a.TokensCreationData.GetAllTokens();
            var tokensB = b.TokensCreationData.GetAllTokens();
            var positionsIntersection = tilesA.Intersect(tilesB).ToList();
            int intersectionCount = positionsIntersection.Count();
            int unionCount = tilesA.Union(tilesB).Count();
            matches += intersectionCount;
            total += unionCount;

            // tokens
            matches += tokensA.Intersect(tokensB).Count();
            total += tokensA.Union(tokensB).Count();

            void ComputeLayersSimilarity(List<PositionTokenData> layerA, List<PositionTokenData> layerB)
            {
                int intersect = layerA.Count(da =>
                {
                    var (posA, tokenA) = da;
                    bool equal = layerB.Find(db => posA == db.position && tokenA == db.tokenData) != null;
                    return equal;
                });
                matches += intersect;
                total += layerA.Count + layerB.Count - intersect * 2;
            }
            
            // initial state - main layer
            ComputeLayersSimilarity(a.initiallyPositionedTokens, b.initiallyPositionedTokens);
            
            // initial state - bottom layer
            var botLayersA = a.bottomLayersTokens;
            var botLayersB = b.bottomLayersTokens;
            var botLessLayers = botLayersA.Count > botLayersB.Count ? botLayersB : botLayersA;
            var botMoreLayers = botLayersA.Count > botLayersB.Count ? botLayersA : botLayersB;
            for (int i = 0; i < botMoreLayers.Count; i++)
            {
                var botMoreLayer = botMoreLayers[i];
                if (i < botLessLayers.Count)
                {
                    var botLessLayer = botLessLayers[i];
                    ComputeLayersSimilarity(botMoreLayer, botLessLayer);
                }
                else
                {
                    total += ((List<PositionTokenData>) botMoreLayer).Count;
                }
            }
            
            // initial state - top layer
            var topLayersA = a.topLayersTokens;
            var topLayersB = b.topLayersTokens;
            var topLessLayers = topLayersA.Count > topLayersB.Count ? topLayersB : topLayersA;
            var topMoreLayers = topLayersA.Count > topLayersB.Count ? topLayersA : topLayersB;
            for (int i = 0; i < topMoreLayers.Count; i++)
            {
                var topMoreLayer = topMoreLayers[i];
                if (i < topLessLayers.Count)
                {
                    var topLessLayer = topLessLayers[i];
                    ComputeLayersSimilarity(topMoreLayer, topLessLayer);
                }
                else
                {
                    total += ((List<PositionTokenData>) topMoreLayer).Count;
                }
            }
            
            float similarity = (float) matches / total;
            return similarity;
        }

        #region Data validation

        public static void ValidateFromEditorState(Level level)
        {
            var board = Board.PopulateLevelNoFillEmpty(level);
            var context = new GameContext();
            
            bool hasMatch = board.ExistsMatch(context);
            if (hasMatch)
            {
                var matches = board.GetMatches(context);
                var matchesStrings = matches.Select(match =>
                {
                    var positionsStrings = match.Positions.Select(pos => pos.ToString());
                    string positionsString = string.Join(" - ", positionsStrings);
                    return positionsString;
                });
                string matchesString = string.Join("\t\n", matchesStrings);
                string errorString = $"The level contains matches\n{matchesString}";
                Debug.LogError(errorString);
            }
            
            // Test if can fill with solution
//            board = Board.PopulateLevel(level);
//            bool hasSolution = board.ExistsSolution();
//            if (!hasSolution)
//                Debug.LogError("The level can't be populated with at least one solution");


//            bool fine = !hasMatch && hasSolution;
            bool fine = !hasMatch;
            if (fine)
                Debug.Log("✔ Level is fine");
        }
        
        private bool ValidateTokensCreator()
        {
            return _tokensCreationData.ValidateData();
        }

        private bool ValidateTokensSourcesNotEmpty()
        {
            return tokenSources != null && tokenSources.Count > 0;
        }
        
        private bool ValidateVictoryNotNull()
        {
            return _victoryEvaluator != null;
        }
        
        private bool ValidateDefeatNotNull()
        {
            return _defeatEvaluator != null;
        }

        #endregion

#if UNITY_EDITOR
        public void Validate(Report report)
        {
            Debug.Log($"Validating {name}");
            bool hasSolution = true;
            try
            {
                var context = new GameContext();
                var gc = new GameController(this, context);
                if (FallTokensAtStart)
                {
                    var turn = gc.StartGame();
                    turn.TurnSteps.ExecuteTurnStepsNow();
                }

                hasSolution = gc.Board.ExistsSolution(context);
            }
            catch (Exception e)
            {
                report.Log(this, WarningType.Error, "Levels", e.Message, ""); 
            }
            if (!hasSolution)
                report.Log(this, WarningType.Error, "Levels",
                    $"The level {name} can't be populated with at least one solution", ""); 
        }
#endif
    }
}