using System.Threading;
using System.Threading.Tasks;
using Match3.Core.GameActions.Actions;
using Match3.Core.GameActions.Interactions;
using UnityEngine;

namespace Match3.View.Interactions
{
    public interface IInteractionView
    {
        void Initialize();
        bool CanProvideInteractionForAction(IGameAction action);
        Task<(IInteraction, bool)> WaitInteractionAsync(CancellationToken ct);
    }

    public abstract class InteractionViewBase<T> : MonoBehaviour, IInteractionView where T : IInteraction
    {
        public abstract void Initialize();

        public bool CanProvideInteractionForAction(IGameAction action)
        {
            var type = typeof(T);
            return action.CanExecuteFromInteractionType(type);
        }

        public async Task<(IInteraction, bool)> WaitInteractionAsync(CancellationToken ct)
        {
            var interaction = await WaitInteractionBaseAsync(ct);
            return interaction;
        }

        protected abstract Task<(T, bool)> WaitInteractionBaseAsync(CancellationToken ct);
    }
}