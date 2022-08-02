using System;
using UnityEngine;

namespace Match3.Core.GameActions.TokensDamage
{
    [Serializable]
    public class DamageInfo
    {
        [SerializeField] private int _damage;

        public int Damage => _damage;

        public DamageInfo(int damage)
        {
            _damage = damage;
        }
    }
}