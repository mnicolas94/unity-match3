using Match3.Core.TurnSteps.TokensDestructionSource;
using UnityEngine;

namespace Match3.View.DestructionSources
{
    public interface ITokenDestructionSourceView : ITokenDamageSource
    {
        void RenderDamage(Grid grid, Vector2Int destructionPosition);
    }
}