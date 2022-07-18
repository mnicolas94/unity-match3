using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Match3.Core;
using Match3.Core.TurnSteps;
using UnityEngine;

namespace Match3.View.TurnStepRenderers
{
    public class TurnStepRendererShuffle : TurnStepRenderer<TurnStepShuffle>
    {
        [SerializeField] private float shuffleTime;

        protected override async Task RenderTurnStep(
            TurnStepShuffle step,
            Grid grid,
            Board board,
            TokenDataViewMap dataViewMap,
            CancellationToken ct)
        {
            await TurnStepRendererGravity.AnimateTokensMovements(
                step.ShuffleMovements.ToList(),
                grid,
                shuffleTime,
                dataViewMap,
                ct
            );
        }
    }
}