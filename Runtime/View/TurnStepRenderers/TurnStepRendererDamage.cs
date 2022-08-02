using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Match3.Core;
using Match3.Core.TurnSteps;
using Match3.View.DestructionSources;
using UnityEngine;

namespace Match3.View.TurnStepRenderers
{
    public class TurnStepRendererDamage : TurnStepRenderer<TurnStepDamageTokens>
    {
        [SerializeField] private int _delayMillisBetweenWaves;
        
        protected override async Task RenderTurnStep(
            TurnStepDamageTokens step,
            Grid grid,
            Board board,
            TokenDataViewMap dataViewMap,
            CancellationToken ct)
        {
            var groupedTokens = step
                .GetAllPositionsTokensDestructionOrders()
                .GroupBy(tuple => tuple.SortOrder)
                .OrderBy(group => group.Key);

            foreach (var tokensDamaged in step.TokensDamaged)
            {
                if (tokensDamaged.Source is ITokenDestructionSourceView view)
                {
                    view.RenderDamage(grid, tokensDamaged.SourcePosition);
                }
            }

            try  // this is mainly to catch TaskCanceledException from "await Task.Delay..." line
            {
                foreach (var group in groupedTokens)
                {
                    foreach (var (position, token, order, damageInfo) in group)
                    {
                        bool destroyed = token.HealthPoints <= 0;
                        if (destroyed)
                        {
                            dataViewMap.RemoveTokenFromMap(token);
                        }
                        else
                        {
                            var tokenView = dataViewMap.GetTokenView(token);
                            tokenView.UpdateHealthChange(damageInfo);
                        }
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