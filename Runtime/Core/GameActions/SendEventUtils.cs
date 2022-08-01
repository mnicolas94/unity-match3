using System.Collections.Generic;
using System.Linq;
using Match3.Core.SerializableTuples;
using Match3.Core.TokensEvents.Events;
using Match3.Core.TokensEvents.Outputs;
using UnityEngine;

namespace Match3.Core.GameActions
{
    public static class SendEventUtils
    {
        internal static void SendDestroyedEvent(GameContext context, Board board, List<PositionToken> destroyedTokens, List<TokenEventOutput> outputs)
        {
            foreach (var (position, token) in destroyedTokens)
            {
                var @event = new EventDestroyed(board, token, position);
                SendEvent(context, token, @event, outputs);
            }
        }
        
        internal static void SendAdjacentMatchEvent(GameContext context, Board board, List<Vector2Int> matchesPositions, List<TokenEventOutput> outputs)
        {
            void SendAdjacentEvent(Vector2Int position, Vector2Int dir)
            {
                var adjacentPosition = position + dir;
                var existsToken = board.ExistsTokenAnyLayerAt(adjacentPosition);
                bool notMatch = !matchesPositions.Contains(adjacentPosition);
                if (existsToken && notMatch)
                {
                    var (token, layer) = board.GetTopTokenAt(adjacentPosition);
                    var @event = new EventAdjacentMatch(board, token, adjacentPosition, position);
                    SendEvent(context, token, @event, outputs);
                }
            }
            
            foreach (var matchPosition in matchesPositions)
            {
                SendAdjacentEvent(matchPosition, Vector2Int.down);
                SendAdjacentEvent(matchPosition, Vector2Int.up);
                SendAdjacentEvent(matchPosition, Vector2Int.left);
                SendAdjacentEvent(matchPosition, Vector2Int.right);
            }
        }
        
        internal static void SendAdjacentDestroyedEvent(GameContext context, Board board, List<Vector2Int> destroyedPositions, List<TokenEventOutput> outputs)
        {
            void SendDestroyedEvent(Vector2Int destroyedPosition, Vector2Int dir)
            {
                var position = destroyedPosition + dir;
                var existsToken = board.ExistsTokenAnyLayerAt(position);
                bool notMatch = !destroyedPositions.Contains(position);
                if (existsToken && notMatch)
                {
                    var (token, layer) = board.GetTopTokenAt(position);
                    var @event = new EventAdjacentDestroyed(board, token, position, destroyedPosition);
                    SendEvent(context, token, @event, outputs);
                }
            }
            
            foreach (var destroyedPosition in destroyedPositions)
            {
                SendDestroyedEvent(destroyedPosition, Vector2Int.down);
                SendDestroyedEvent(destroyedPosition, Vector2Int.up);
                SendDestroyedEvent(destroyedPosition, Vector2Int.left);
                SendDestroyedEvent(destroyedPosition, Vector2Int.right);
            }
        }
        
        internal static void SendBelowMatchedEvent(GameContext context, Board board, List<Vector2Int> matchedPositions, List<TokenEventOutput> outputs)
        {
            foreach (var matchedPosition in matchedPositions)
            {
                var existsTopToken = board.TopLayers.ExistsAnyTokenAt(matchedPosition);
                if (existsTopToken)
                {
                    var (token, layer) = board.TopLayers.GetTopTokenAt(matchedPosition);
                    var @event = new EventBelowMatched(board, token, matchedPosition);
                    SendEvent(context, token, @event, outputs);
                }
            }
        }
        
        internal static void SendAboveDestroyedEvent(GameContext context, Board board, List<Vector2Int> destroyedPositions, List<TokenEventOutput> outputs)
        {
            foreach (var destroyedPosition in destroyedPositions)
            {
                var existsToken = board.ExistsTokenAnyLayerAt(destroyedPosition);
                if (existsToken)
                {
                    var (token, layer) = board.GetTopTokenAt(destroyedPosition);
                    var @event = new EventAboveDestroyed(board, token, destroyedPosition);
                    SendEvent(context, token, @event, outputs);
                }
            }
        }

        internal static void SendSwapEvent(GameContext context, 
            Board board,
            Vector2Int position,
            Token token,
            Vector2Int otherPosition,
            Token otherToken,
            List<TokenEventOutput> outputs)
        {
            var @event = new EventSwapped(board, token, position, otherToken, otherPosition);
            SendEvent(context, token, @event, outputs);
        }
        
        internal static void SendReachBottomEvent(GameContext context, Board board, List<PositionToken> tokens, List<TokenEventOutput> outputs)
        {
            foreach (var (position, token) in tokens)
            {
                var @event = new EventReachBottom(board, token, position);
                SendEvent(context, token, @event, outputs);
            }
        }

        internal static void SendEvent<T>(GameContext context, Token token, T @event, List<TokenEventOutput> outputs) where T : TokenEventInput
        {
            var globalResolvers = context.GlobalResolvers.GetResolvers<T>();
            var tokenResolvers = token.TokenData.Resolvers.GetResolvers<T>();
            var resolvers = tokenResolvers.Concat(globalResolvers);
            foreach (var resolver in resolvers)
            {
                var output = resolver.OnEvent(@event);
                outputs.Add(output);
            }
        }
    }
}