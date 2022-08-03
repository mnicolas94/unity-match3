using UnityEngine;

namespace Match3.Core.GameActions.TokensDamage
{
    public interface IDamageController
    {
        DamageInfo DamageToken(GameContext context, Board board, Token token, Vector2Int position);
    }
}