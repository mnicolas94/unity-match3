using System.Collections.Generic;
using Match3.Core.TokenProviders;
using UnityEngine;

namespace Match3.Core.Levels.TokenProviders
{
    public interface ITokenCreationProvider : ITokenProvider<(GameController controller, TokenSource tokenSource, Vector2Int position)>
    {
        IEnumerable<TokenData> GetAvailableTokens();
    }
}