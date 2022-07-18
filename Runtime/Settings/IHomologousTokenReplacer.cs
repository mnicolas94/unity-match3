using Match3.Core;

namespace Match3.Settings
{
    public interface IHomologousTokenReplacer
    {
        void ReplaceToken(TokenData toReplace, TokenData replacement);
    }
}