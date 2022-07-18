using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Match3.View.DestructionSources
{
    [Serializable]
    public class DestructionSourceParticles : ITokenDestructionSourceView
    {
        [SerializeField] private ParticleSystem _particlesPrefab;
        
        public void RenderDestruction(Grid grid, Vector2Int destructionPosition)
        {
            var position = grid.GetCellCenterWorld((Vector3Int) destructionPosition);
            Object.Instantiate(_particlesPrefab, position, Quaternion.identity);
        }
    }
}