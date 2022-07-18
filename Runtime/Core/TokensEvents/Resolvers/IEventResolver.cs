using Match3.Core.TokensEvents.Events;
using Match3.Core.TokensEvents.Outputs;

namespace Match3.Core.TokensEvents.Resolvers
{
    public interface IEventResolver
    {
        TokenEventOutput OnEvent(TokenEventInput @event);
    }
}