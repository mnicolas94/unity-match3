using System.Threading;
using System.Threading.Tasks;
using Match3.Core;
using Match3.Core.TurnSteps;
using UnityEngine;

namespace Match3.View.TurnStepRenderers
{
    public abstract class TurnStepRenderer : MonoBehaviour
    {
        public abstract bool CanRenderStep(TurnStep step);
        
        public abstract Task RenderTurnStep(
            TurnStep step,
            Grid grid,
            Board board,
            TokenDataViewMap dataViewMap,
            CancellationToken ct
        );
    }
    
    public abstract class TurnStepRenderer<T> : TurnStepRenderer where T : TurnStep
    {
        public override bool CanRenderStep(TurnStep step)
        {
            return step is T;
        }

        public override Task RenderTurnStep(TurnStep step, Grid grid, Board board, TokenDataViewMap dataViewMap, CancellationToken ct)
        {
            return RenderTurnStep((T) step, grid, board, dataViewMap, ct);
        }

        protected abstract Task RenderTurnStep(
            T step,
            Grid grid,
            Board board,
            TokenDataViewMap dataViewMap,
            CancellationToken ct
        );
    }
}