﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Match3.Core.GameActions;
using Match3.Core.GameActions.Actions;
using Match3.Core.GameActions.Interactions;
using Match3.View.Interactions;
using Match3.View.SkillCostViews;
using ModelView;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.View
{
    public class SkillView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _skillImage;

        public Action<SkillView> OnInteractionStarted;
        public Action<SkillView> OnInteractionEnded;
        
        private IInteractionView _interactionView;
        private Skill _skill;
        private SkillCostViewBase _costView;

        public SkillCostViewBase CostView => _costView;

        public Skill Skill => _skill;

        public bool IsSkillUsable => _skill.SkillCost.CanExecuteSkill(_skill);
        
        public void Initialize(Skill skill, IInteractionView interactionView, SkillCostViewBase costView)
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
            OnInteractionStarted?.Invoke(this);
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
                OnInteractionEnded?.Invoke(this);
            }
        }

        public void UpdateCostView()
        {
            if (_costView)
            {
                _costView.UpdateView(_skill);
            }
        }
    }
}