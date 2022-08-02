using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Match3.Core;
using Match3.Core.SerializableTuples;
using Match3.Core.TurnSteps;
using UnityEngine;
using Utils.Tweening;

namespace Match3.View.TurnStepRenderers
{
    public class TurnStepRendererGravity : TurnStepRenderer<TurnStepMovements>
    {
        [SerializeField] private float movementTime;
        [SerializeField] private float finalDelay;

        protected override async Task RenderTurnStep(
            TurnStepMovements step,
            Grid grid,
            Board board,
            TokenDataViewMap dataViewMap,
            CancellationToken ct)
        {
            foreach (var movements in step.TokensPaths)
            {
                if (ct.IsCancellationRequested)
                    return;
                await AnimateTokensMovements(movements, grid, movementTime, dataViewMap, ct);
            }

            try
            {
                await Task.Delay((int) (finalDelay * 1000), ct);
            }
            catch
            {
                // ignored
            }
        }

        public static async Task AnimateTokensMovements(
            List<TokenMovement> movements,
            Grid grid,
            float movementTime,
            TokenDataViewMap dataViewMap,
            CancellationToken ct)
        {
            var tweeners = movements.ConvertAll(movement =>
            {
                var (token, fromPosition, toPosition) = movement;
                var tokenView = dataViewMap.GetTokenViewOrAddIfNotExists(token, 0);
                var worldPositionA = grid.GetCellCenterWorld((Vector3Int) fromPosition);
                var worldPositionB = grid.GetCellCenterWorld((Vector3Int) toPosition);

                var tweenTask = tokenView.transform.TweenMoveAsync(
                    worldPositionA,
                    worldPositionB,
                    movementTime,
                    Curves.Linear,
                    ct);
                
                return tweenTask;
            });
            
            await Task.WhenAll(tweeners);
        }
    }
}