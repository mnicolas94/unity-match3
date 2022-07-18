using UnityEngine;
using Utils.Serializables;

namespace Match3.Core.Levels.TokensCreationConditions
{
    public interface ITokenCreationPredicate : ISerializablePredicate<(GameController, TokenSource, Vector2Int)>
    {
    }
}