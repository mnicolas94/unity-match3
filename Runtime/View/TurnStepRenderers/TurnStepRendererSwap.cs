using System.Threading;
using System.Threading.Tasks;
using Match3.Core;
using Match3.Core.TurnSteps;
using UnityEngine;
using Utils.Tweening;

namespace Match3.View.TurnStepRenderers
{
    public class TurnStepRendererSwap : TurnStepRenderer<TurnStepSwap>
    {
        [SerializeField] private float movementTime;

        protected override async Task RenderTurnStep(
            TurnStepSwap step,
            Grid grid,
            Board board,
            TokenDataViewMap dataViewMap,
            CancellationToken ct)
        {
            var positionA = step.TokenPositionA;
            var positionB = step.TokenPositionB;
            var tokenA = step.TokenA;
            var tokenB = step.TokenB;
            var tokenViewA = dataViewMap.GetTokenView(tokenA);
            var tokenViewB = dataViewMap.GetTokenView(tokenB);
            var transformA = tokenViewA.transform;
            var transformB = tokenViewB.transform;
            var worldPositionA = grid.GetCellCenterWorld((Vector3Int) positionA);
            var worldPositionB = grid.GetCellCenterWorld((Vector3Int) positionB);

            transformA.position = worldPositionA;
            transformB.position = worldPositionB;
            
            var taskA = transformA.TweenMoveAsync(worldPositionA, worldPositionB, movementTime, Curves.Linear, ct);
            var taskB = transformB.TweenMoveAsync(worldPositionB, worldPositionA, movementTime, Curves.Linear, ct);

            await Task.WhenAll(taskA, taskB);
        }
    }
}