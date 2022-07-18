using System;
using System.Collections.Generic;
using System.Linq;
using Match3.Core.SerializableDictionaries;
using UnityEngine;

namespace Match3.Core
{
    [Serializable]
    public class BoardLayer
    {
        [SerializeField] [HideInInspector] private TokensPositions _tokensPositions;
        [SerializeField] [HideInInspector] private BoardShape _boardShape;

        public BoardLayer(BoardShape boardShape)
        {
            _tokensPositions = new TokensPositions();
            _boardShape = boardShape;
        }

        public void Clear()
        {
            _tokensPositions.Clear();
        }

        public bool ExistsTokenAtWhere(Vector2Int position, Predicate<Token> wherePredicate)
        {
            if (_tokensPositions.ContainsKey(position))
            {
                var token = GetTokenAt(position);
                return wherePredicate(token);
            }

            return false;
        }
        
        public bool ExistsTokenAt(Vector2Int position)
        {
            return ExistsTokenAtWhere(position, token => true);
        }

        public bool ExistsToken(Token token)
        {
            return _tokensPositions.ContainsValue(token);
        }

        public Token GetTokenAt(Vector2Int position)
        {
            return _tokensPositions[position];
        }

        public Vector2Int GetPositionOfToken(Token token)
        {
            foreach (var position in _tokensPositions.Keys)
            {
                var tok = _tokensPositions[position];
                if (token == tok)
                {
                    return position;
                }
            }

            throw new ArgumentException($"Token {token} does not exists");
        }

        public Token AddTokenAt(TokenData tokenData, Vector2Int position)
        {
            var token = new Token(tokenData);
            AddTokenAt(token, position);
            return token;
        }
        
        public void AddTokenAt(Token token, Vector2Int position)
        {
            bool isValidPosition = _boardShape.ExistsPosition(position);
            if (!isValidPosition)
            {
                throw new ArgumentException($"Position {position} does not exist in board");
            }
            
            if (!_tokensPositions.ContainsKey(position))
            {
                _tokensPositions.Add(position, token);
            }
        }

        public void RemoveTokenAt(Vector2Int position)
        {
            _tokensPositions.Remove(position);
        }

        public void RemoveToken(Token token)
        {
            var position = GetPositionOfToken(token);
            RemoveTokenAt(position);
        }

        public List<Vector2Int> GetPositions()
        {
            return new List<Vector2Int>(_tokensPositions.Keys);
        }

        public List<(Vector2Int, Token)> GetTokenPositions()
        {
            var tokensPositions = new List<(Vector2Int, Token)>();
            foreach (var position in _tokensPositions.Keys)
            {
                var token = _tokensPositions[position];
                tokensPositions.Add((position, token));
            }

            return tokensPositions;
        }

        public List<Vector2Int> GetAllPositionsOfTokenData(TokenData tokenData)
        {
            return GetAllPositionsOfTokenWhere(token => token.TokenData == tokenData);
        }
        
        public List<Vector2Int> GetAllPositionsOfTokenWhere(Predicate<Token> whereFunction)
        {
            var positions = new List<Vector2Int>();
            var tokensPositions = GetTokenPositions();
            foreach (var (position, tk) in tokensPositions)
            {
                if (whereFunction.Invoke(tk))
                {
                    positions.Add(position);
                }
            }

            return positions;
        }
    }

    public static class BoardLayerListExtensions
    {
        public static bool ExistsAnyTokenAtWhere(this IEnumerable<BoardLayer> layers, Vector2Int position,
            Predicate<Token> wherePredicate)
        {
            foreach (var layer in layers)
            {
                return layer.ExistsTokenAtWhere(position, wherePredicate);
            }

            return false;
        }
        
        public static bool ExistsAnyTokenAt(this IEnumerable<BoardLayer> layers, Vector2Int position)
        {
            return layers.ExistsAnyTokenAtWhere(position, token => true);
        }

        public static bool ExistsToken(this IEnumerable<BoardLayer> layers, Token token)
        {
            foreach (var layer in layers)
            {
                if (layer.ExistsToken(token))
                    return true;
            }

            return false;
        }

        public static IEnumerable<Token> GetAllTokensAt(this IEnumerable<BoardLayer> layers, Vector2Int position)
        {
            foreach (var layer in layers)
            {
                if (layer.ExistsTokenAt(position))
                    yield return layer.GetTokenAt(position);
            }
        }
        
        public static (Token, BoardLayer) GetTopTokenAt(this IList<BoardLayer> layers, Vector2Int position) 
        {
            foreach (var layer in IterateFromTopToBottom(layers))
            {
                if (layer.ExistsTokenAt(position))
                    return (layer.GetTokenAt(position), layer);
            }

            return (null, null);
        }
        
        public static (Token, BoardLayer) GetBottomTokenAt(this IList<BoardLayer> layers, Vector2Int position)
        {
            foreach (var layer in IterateFromBottomToTop(layers))
            {
                if (layer.ExistsTokenAt(position))
                    return (layer.GetTokenAt(position), layer);
            }

            return (null, null);
        }

        public static IEnumerable<BoardLayer> IterateFromTopToBottom(this IList<BoardLayer> layers)
        {
            return layers.Reverse();
        }

        public static IEnumerable<BoardLayer> IterateFromBottomToTop(this IList<BoardLayer> layers)
        {
            return layers;
        }
        
        public static IEnumerable<(Vector2Int, Token)> GetTokenPositions(this IList<BoardLayer> layers)
        {
            return layers.SelectMany(layer => layer.GetTokenPositions());
        }
        
        public static IEnumerable<(Vector2Int, Token, int)> GetTokenPositionsWithLayerIndex(this IList<BoardLayer> layers)
        {
            for (int i = 0; i < layers.Count; i++)
            {
                var layer = layers[i];
                var tokensPositions = layer.GetTokenPositions();
                foreach (var (position, token) in tokensPositions)
                {
                    yield return (position, token, i);
                }
            }
        }
    }
}