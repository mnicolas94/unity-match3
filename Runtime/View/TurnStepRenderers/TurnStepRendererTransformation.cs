using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Match3.Core;
using Match3.Core.SerializableTuples;
using Match3.Core.TurnSteps;
using UnityEngine;

namespace Match3.View.TurnStepRenderers
{
    public class TurnStepRendererTransformation : TurnStepRenderer<TurnStepTransformations>
    {
        [SerializeField] private float animationTime;
        
        protected override async Task RenderTurnStep(
            TurnStepTransformations step,
            Grid grid,
            Board board,
            TokenDataViewMap dataViewMap,
            CancellationToken ct)
        {
            var movements = step.Transformations.SelectMany(transformation =>
            {
                return transformation.TransformedTokens.ConvertAll(posToken =>
                {
                    var fromPosition = posToken.Position;
                    var toPosition = transformation.SpawnedToken.Position;
                    var token = posToken.Token;
                    return new TokenMovement(token, fromPosition, toPosition);
                });
            });
            
            await TurnStepRendererGravity.AnimateTokensMovements(
                movements.ToList(),
                grid,
                animationTime,
                dataViewMap,
                ct
            );

            foreach (var transformation in step.Transformations)
            {
                var token = transformation.SpawnedToken.Token;
                var position = transformation.SpawnedToken.Position;
                var tokenView = dataViewMap.AddTokenToMap(token, 0);
                var worldPosition = grid.GetCellCenterWorld((Vector3Int) position);
                tokenView.transform.position = worldPosition;
            }
        }
    }
}