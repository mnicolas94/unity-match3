using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Match3.Core;
using Match3.Core.TurnSteps;
using Match3.View.DestructionSources;
using UnityEngine;

namespace Match3.View.TurnStepRenderers
{
    public class TurnStepRendererDestroy : TurnStepRenderer<TurnStepDestroyTokens>
    {
        [SerializeField] private int _delayMillisBetweenWaves;
        
        protected override async Task RenderTurnStep(
            TurnStepDestroyTokens step,
            Grid grid,
            Board board,
            TokenDataViewMap dataViewMap,
            CancellationToken ct)
        {
            var groupedTokens = step
                .GetAllPositionsTokensDestructionOrders()
                .GroupBy(tuple => tuple.SortOrder)
                .OrderBy(group => group.Key);

            foreach (var destruction in step.TokensDestructions)
            {
                if (destruction.Source is ITokenDestructionSourceView view)
                {
                    view.RenderDestruction(grid, destruction.SourcePosition);
                }
            }

            try  // this is mainly to catch TaskCanceledException from "await Task.Delay..." line
            {
                foreach (var group in groupedTokens)
                {
                    foreach (var (position, token, order) in group)
                    {
                        dataViewMap.RemoveTokenFromMap(token);
                    }

                    await Task.Delay(_delayMillisBetweenWaves, ct);
                }
            }
            catch
            {
                // ignored
            }

            await Task.Yield();
        }
    }
}