using System;
using System.Collections.Generic;
using Match3.Core.TurnSteps;
using UnityEngine;

namespace Match3.Core.Gravity
{
    using LongReturnType = ValueTuple<List<MovementsList>, Dictionary<Token, (MovementsList, TokenSource)>>;

    public interface IBoardGravity
    {
        LongReturnType GetTurnStepMovements(Board board);
    }

    public static class GravityUtils
    {
        public static readonly IBoardGravity Default = new CandyCrushGravity();
        
        public static bool CanMoveTo(Board board, Vector2Int position)
        {
            bool isValid = board.BoardShape.ExistsPosition(position);
            bool isEmpty = !board.MainLayer.ExistsTokenAt(position);

            return isValid && isEmpty;
        }
        
        public static bool CanMoveFrom(Board board, Vector2Int position)
        {
            bool hasTokenSource = board.ExistsTokenSourceAt(position);
            bool hasToken = board.MainLayer.ExistsTokenAt(position);
            bool canMove = false;
            if (hasToken)
            {
                var token = board.MainLayer.GetTokenAt(position);
                bool isFake = token is FakeToken;
                canMove = isFake || token.TokenData.CanMove;
            }
            bool hasFrontObstacle = board.TopLayers.ExistsAnyTokenAt(position);

            bool canMoveFrom = hasToken && canMove && !hasFrontObstacle || hasTokenSource;
            return canMoveFrom;
        }
    }
}