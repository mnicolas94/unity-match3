namespace Match3.Core.TokenProviders
{
    public interface ITokenProvider
    {
        TokenData GetToken();
    }
    
    public interface ITokenProvider<T>
    {
        TokenData GetToken(T context);
    }
}