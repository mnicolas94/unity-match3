using System;
using UnityEngine;

namespace Match3.Core.GameActions.TokensDamage
{
    [Serializable]
    public class DefaultDamageController : IDamageController
    {
        public DamageInfo DamageToken(GameContext context, Board board, Token token, Vector2Int position)
        {
            int damage = 1;
            var damageDone = token.ApplyDamage(damage);
            return new DamageInfo(damageDone);
        }
    }
}