using System.Threading;
using System.Threading.Tasks;
using Match3.Core.GameActions;
using Match3.Core.GameActions.Actions;
using Match3.Core.GameActions.Interactions;
using Match3.View.Interactions;
using UnityEngine;
using UnityEngine.UI;
using Utils.ModelView;

namespace Match3.View
{
    public class SkillView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _skillImage;
        
        private IInteractionView _interactionView;
        private Skill _skill;
        private ViewBase<Skill> _costView;

        public bool IsSkillUsable => _skill.SkillCost.CanExecuteSkill(_skill);
        
        public void Initialize(Skill skill, IInteractionView interactionView, ViewBase<Skill> costView)
        {
            _interactionView = interactionView;
            _skill = skill;
            _costView = costView;
            _skillImage.sprite = skill.SkillImage;
        }

        public async Task<SkillView> WaitForSkillPressed(
            CancellationToken ct)
        {
            await AsyncUtils.Utils.WaitPressButtonAsync(_button, ct);
            return this;
        }

        public async Task<(IInteraction, IGameAction, bool)> WaitForInteractionAsync(CancellationToken ct)
        {
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            var linkedCt = linkedCts.Token;
            try
            {
                var buttonTask = AsyncUtils.Utils.WaitPressButtonAsync(_button, linkedCt);
                var interactionTask = _interactionView.WaitInteractionAsync(linkedCt);
                var finishedTask = await Task.WhenAny(buttonTask, interactionTask);
                if (finishedTask == interactionTask)
                {
                    var (interaction, success) = await interactionTask;
                    linkedCts.Cancel();
                    var action = _skill.AppliesCostGameAction;
                    action.OnCostApplied += _ => UpdateCostView();
                    return (interaction, action, success);
                }
                else
                {
                    linkedCts.Cancel();
                    return (default, default, false);
                }
            }
            finally
            {
                linkedCts.Dispose();
            }
        }

        public void UpdateCostView()
        {
            _costView.UpdateView(_skill);
        }
    }
}