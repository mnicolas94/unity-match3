using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Match3.Core.Gravity;
using Match3.Core.Levels;
using Match3.Core.SerializableDictionaries;
using Match3.Core.SerializableTuples;
using UnityEngine;
using UnityEngine.Assertions;
using Utils.Extensions;

namespace Match3.Core
{
    [Serializable]
    public class Board
    {
        [SerializeField] [HideInInspector] private BoardShape _boardShape;
        [SerializeField] [HideInInspector] private BoardLayer _mainLayer;
        [SerializeField] [HideInInspector] private List<BoardLayer> _bottomLayers;
        [SerializeField] [HideInInspector] private List<BoardLayer> _topLayers;
        [SerializeField] [HideInInspector] private SourcesPositions _tokenSources;

        public BoardShape BoardShape => _boardShape;

        public BoardLayer MainLayer => _mainLayer;

        public ReadOnlyCollection<BoardLayer> BottomLayers => _bottomLayers.AsReadOnly();

        public ReadOnlyCollection<BoardLayer> TopLayers => _topLayers.AsReadOnly();

        public Board(BoardShape boardShape)
        {
            _boardShape = boardShape;
            _mainLayer = new BoardLayer(boardShape);
            _bottomLayers = new List<BoardLayer>();
            _topLayers = new List<BoardLayer>();
            _tokenSources = new SourcesPositions();
        }

        public static Board PopulateLevel(Level level, GameContext context)
        {
            var board = new Board(level.BoardShape);
            PopulateLevel(board, level, context);
            return board;
        }
        
        public static Board PopulateLevelNoFillEmpty(Level level)
        {
            var board = new Board(level.BoardShape);
            PopulateLevelNoFillEmpty(board, level);
            return board;
        }
        
        public static void PopulateLevel(Board board, Level level, GameContext context)
        {
            Assert.IsTrue(
                level.TokensCreationData.Tokens.Count > 0,
                $"The level {level} has no tokens."
            );

            Assert.IsTrue(
                level.TokenSources.Count > 0,
                $"The level {level} has no token sources."
            );
            
            PopulateLevelNoFillEmpty(board, level);
            bool fillEmptyPositions = !level.FallTokensAtStart;
            if (fillEmptyPositions)
            {
                board.FillEmptyTilesWithoutMatches(context, level.TokensCreationData.AvailableTokens(), level.ProhibitedPositions);
            }
        }

        public static void PopulateLevelNoFillEmpty(Board board, Level level)
        {
            void AddTokensToLayer(IList<PositionTokenData> tokens, BoardLayer layer)
            {
                foreach (var (position, tokenData) in tokens)
                {
                    layer.AddTokenAt(tokenData, position);
                }
            }

            board.Clear();

            // add token sources
            var tokenSources = level.TokenSources;
            foreach (var (position, tokenSource) in tokenSources)
            {
                board.AddTokenSourceAt(position, tokenSource);
            }

            // add main layer tokens
            var tokens = level.InitiallyPositionedTokens;
            AddTokensToLayer(tokens, board.MainLayer);

            // add bottom layers' tokens
            var bottomLayers = level.BottomLayersTokens;
            foreach (var layerTokens in bottomLayers)
            {
                var bottomLayer = board.AddBottomLayer();
                AddTokensToLayer(layerTokens, bottomLayer);
            }

            // add top layers' tokens
            var topLayers = level.TopLayersTokens;
            foreach (var layerTokens in topLayers)
            {
                var topLayer = board.AddTopLayer();
                AddTokensToLayer(layerTokens, topLayer);
            }
        }

        public void Clear()
        {
            _mainLayer.Clear();
            _bottomLayers.Clear();
            _topLayers.Clear();
            _tokenSources.Clear();
        }

        public BoardLayer AddTopLayer()
        {
            var layer = new BoardLayer(_boardShape);
            _topLayers.Add(layer);
            return layer;
        }
        
        public BoardLayer AddBottomLayer()
        {
            var layer = new BoardLayer(_boardShape);
            _bottomLayers.Add(layer);
            return layer;
        }

        public bool ExistsTokenSourceAt(Vector2Int position)
        {
            return _tokenSources.ContainsKey(position);
        }

        public TokenSource GetTokenSourceAt(Vector2Int position)
        {
            return _tokenSources[position];
        }

        private void AddTokenSourceAt(Vector2Int position, TokenSource tokenSource)
        {
            _tokenSources.Add(position, tokenSource);
        }

        public bool ExistsTokenAnyLayerAtWhere(Vector2Int position, Predicate<Token> wherePredicate)
        {
            bool existsTop = TopLayers.ExistsAnyTokenAtWhere(position, wherePredicate);
            bool existsMain = MainLayer.ExistsTokenAtWhere(position, wherePredicate);
            bool existsBottom = BottomLayers.ExistsAnyTokenAtWhere(position, wherePredicate);
            return existsTop || existsMain || existsBottom;
        }
        
        public bool ExistsTokenAnyLayerAt(Vector2Int position)
        {
            return ExistsTokenAnyLayerAtWhere(position, token => true);
        }

        public bool ExistsTokenInAnyLayer(Token token)
        {
            return MainLayer.ExistsToken(token) ||
                   TopLayers.ExistsToken(token) ||
                   BottomLayers.ExistsToken(token);
        }

        public (Token, BoardLayer) GetTopTokenAt(Vector2Int position)
        {
            return GetTopTokenAtWhere(position, token => true);
        }
        
        public (Token, BoardLayer) GetTopTokenAtWhere(Vector2Int position, Predicate<Token> wherePredicate)
        {
            foreach (var layer in TopLayers.IterateFromTopToBottom())
            {
                bool exists = layer.ExistsTokenAt(position);
                if (exists)
                {
                    var token = layer.GetTokenAt(position);
                    bool meetsPredicate = wherePredicate(token);
                    if (meetsPredicate)
                        return (token, layer);
                }
            }

            if (MainLayer.ExistsTokenAt(position))
            {
                var token = MainLayer.GetTokenAt(position);
                bool meetsPredicate = wherePredicate(token);
                if (meetsPredicate)
                    return (token, MainLayer);
            }

            foreach (var layer in BottomLayers.IterateFromTopToBottom())
            {
                bool exists = layer.ExistsTokenAt(position);
                if (exists)
                {
                    var token = layer.GetTokenAt(position);
                    bool meetsPredicate = wherePredicate(token);
                    if (meetsPredicate)
                        return (token, layer);
                }
            }

            return (null, null);
        }
        
        public IEnumerable<BoardLayer> GetAllLayersFromTopToBottom()
        {
            foreach (var topLayer in TopLayers.IterateFromTopToBottom())
            {
                yield return topLayer;
            }

            yield return MainLayer;
            
            foreach (var bottomLayer in BottomLayers.IterateFromTopToBottom())
            {
                yield return bottomLayer;
            }
        }
        
        public IEnumerable<BoardLayer> GetAllLayersFromBottomToTop()
        {
            foreach (var bottomLayer in BottomLayers.IterateFromBottomToTop())
            {
                yield return bottomLayer;
            }

            yield return MainLayer;
            
            foreach (var topLayer in TopLayers.IterateFromBottomToTop())
            {
                yield return topLayer;
            }            
        }

        public BoardLayer TryGetLayerOfTokenAtPosition(Token token, Vector2Int position, out bool exists)
        {
            var layers = GetAllLayersFromTopToBottom();
            foreach (var layer in layers)
            {
                if (layer.ExistsTokenAt(position))
                    if (layer.GetTokenAt(position) == token)
                    {
                        exists = true;
                        return layer;
                    }
            }
            exists = false;
            return null;
        }
        
        public List<(Vector2Int, Token)> GetTokenPositionsAllLayers()
        {
            var tokensPositions = new List<(Vector2Int, Token)>();
            
            tokensPositions.AddRange(MainLayer.GetTokenPositions());
            tokensPositions.AddRange(BottomLayers.GetTokenPositions());
            tokensPositions.AddRange(TopLayers.GetTokenPositions());

            return tokensPositions;
        }
        
        public List<(Vector2Int, Token, int)> GetTokenPositionsAllLayersWithLayerIndex()
        {
            var tokensPositions = new List<(Vector2Int, Token, int)>();
            foreach (var (position, token) in MainLayer.GetTokenPositions())
            {
                tokensPositions.Add((position, token, 0));
            }
            
            foreach (var (position, token, layerIndex) in BottomLayers.GetTokenPositionsWithLayerIndex())
            {
                tokensPositions.Add((position, token, layerIndex));
            }
            
            foreach (var (position, token, layerIndex) in TopLayers.GetTokenPositionsWithLayerIndex())
            {
                tokensPositions.Add((position, token, layerIndex));
            }

            return tokensPositions;
        }
        
        public bool MoveTokenTo(Vector2Int fromPosition, Vector2Int toPosition)
        {
            if (MainLayer.ExistsTokenAt(fromPosition))
            {
                var token = MainLayer.GetTokenAt(fromPosition);
                bool isEmpty = !MainLayer.ExistsTokenAt(toPosition);
                if (isEmpty)
                {
                    MainLayer.RemoveTokenAt(fromPosition);
                    MainLayer.AddTokenAt(token, toPosition);

                    return true;
                }
            }

            return false;
        }
        
        public void SwapPositions(Vector2Int positionA, Vector2Int positionB)
        {
            bool existsA = MainLayer.ExistsTokenAt(positionA);
            bool existsB = MainLayer.ExistsTokenAt(positionB);

            if (!existsA || !existsB)
            {
                return;
            }

            var tokenA = MainLayer.GetTokenAt(positionA);
            var tokenB = MainLayer.GetTokenAt(positionB);
            MainLayer.RemoveTokenAt(positionA);
            MainLayer.RemoveTokenAt(positionB);
            MainLayer.AddTokenAt(tokenB, positionA);
            MainLayer.AddTokenAt(tokenA, positionB);
        }

        public List<TokenMovement> Shuffle()
        {
            var shuffleData = new List<TokenMovement>();
            var tokensAndPositions = MainLayer.GetTokenPositions();
            // filter only tokens that can be moved
            tokensAndPositions = tokensAndPositions.FindAll(tokenPosition =>
            {
                var (position, token) = tokenPosition;
                return GravityUtils.CanMoveFrom(this, position);
            });
            foreach (var (position, token) in tokensAndPositions)
            {
                MainLayer.RemoveTokenAt(position);
            }

            var positions = tokensAndPositions.ConvertAll(tp => tp.Item1);
            
            foreach (var (position, token) in tokensAndPositions)
            {
                var randomPosition = positions.PopRandom();
                MainLayer.AddTokenAt(token, randomPosition);
                shuffleData.Add(new TokenMovement(token, position, randomPosition));
            }

            return shuffleData;
        }

        public bool IsPositionEmpty(Vector2Int position)
        {
            bool existsPosition = _boardShape.ExistsPosition(position);
            bool isEmpty = !MainLayer.ExistsTokenAt(position);
            return existsPosition && isEmpty;
        }
        
        public bool ExistsEmptyPosition()
        {
            var positions = _boardShape.Tiles;
            foreach (var position in positions)
            {
                if (!MainLayer.ExistsTokenAt(position))
                    return true;
            }

            return false;
        }
        
        public List<Vector2Int> GetEmptyPositions()
        {
            var emptyPositions = new List<Vector2Int>();

            var positions = _boardShape.Tiles;
            foreach (var position in positions)
            {
                if (!MainLayer.ExistsTokenAt(position))
                    emptyPositions.Add(position);
            }

            return emptyPositions;
        }

        public static bool ArePositionsAdjacent(Vector2Int positionA, Vector2Int positionB)
        {
            int xDiff = Math.Abs(positionA.x - positionB.x);
            int yDiff = Math.Abs(positionA.y - positionB.y);
            bool adjacentHorizontal = xDiff == 1 && yDiff == 0;
            bool adjacentVertical = xDiff == 0 && yDiff == 1;
            return adjacentHorizontal || adjacentVertical;
        }

#if UNITY_EDITOR
        
        public override string ToString()
        {
            int largestTokenName = MainLayer.GetTokenPositions().Select(tp => tp.Item2.ToString()).Max(s => s.Length);
            int columnLen = largestTokenName + 2;
            var bounds = _boardShape.GetBounds();
            int lastRow = bounds.yMax;  // get to tokens sources
            string boardString = "";
            for (int y = lastRow; y >= bounds.yMin; y--)
            {
                string row = $"{y} ";
                for (int x = bounds.xMin; x < bounds.xMax; x++)
                {
                    var position = new Vector2Int(x, y);
                    bool hasToken = MainLayer.ExistsTokenAt(position);
                    bool hasSource = ExistsTokenSourceAt(position);
                    bool isInvalid = !_boardShape.ExistsPosition(position);
                    string tokenString = "";
                    if (hasSource)
                    {
                        tokenString = "Source";
                    }
                    else if (isInvalid)
                    {
                        tokenString = " ";
                    }
                    else if (hasToken)
                    {
                        var token = MainLayer.GetTokenAt(position);
                        tokenString = token.ToString();
                    }
                    else
                    {
                        tokenString = "Empty";
                    }

                    int tokenLen = tokenString.Length;
                    int halfDif = (columnLen - tokenLen) / 2;
                    row += tokenString.PadLeft(halfDif + tokenLen).PadRight(columnLen);
                }
                boardString += $"{row}\n";
            }

            return boardString;
        }
#endif
    }
}