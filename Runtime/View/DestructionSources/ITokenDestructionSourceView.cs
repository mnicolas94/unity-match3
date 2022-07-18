using Match3.Core.TurnSteps.TokensDestructionSource;
using UnityEngine;

namespace Match3.View.DestructionSources
{
    public interface ITokenDestructionSourceView : ITokenDestructionSource
    {
        void RenderDestruction(Grid grid, Vector2Int destructionPosition);
    }
}