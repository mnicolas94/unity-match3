using System;

namespace Match3.Core
{
    [Serializable]
    public class TokenSource
    {
        public bool ProvidesToken()
        {
            return false;
        }

        public TokenData GetToken()
        {
            return null;
        }
    }
}