using System;
using System.Collections.Generic;
using Match3.Core.GameActions;
using Match3.View.Interactions;
using ModelView;
using TNRD;
using UnityEngine;
using UnityEngine.Events;

namespace Match3.View
{
    public class SkillsInitializer : MonoBehaviour
    {
        [SerializeField] private List<Skill> _skills;
        [SerializeField] private SerializableInterface<IViewProvider> _costViewsProvider;
        [SerializeField] private Transform _skillsContainer;
        [SerializeField] private Transform _interactionViewsContainer;

        [SerializeField] private SkillView _skillViewPrefab;
        
        [SerializeField] public UnityEvent<SkillView> OnSkillPressed;
        [SerializeField] public UnityEvent<SkillView> OnInteractionEnded;
        
        private List<IInteractionView> _interactionViews;
        
        public List<SkillView> InitializeSkills()
        {
            LoadInteractionViews();
            
            var skillViews = _skills.ConvertAll(skill =>
            {
                var action = skill.GameAction;
                var interactionView = _interactionViews.Find(i => i.CanProvideInteractionForAction(action));
                var skillView = Instantiate(_skillViewPrefab, _skillsContainer);
                bool exists = _costViewsProvider.Value.TryGetViewForModel(skill, out var skillCostView);

                var costView = skillCostView as ViewBaseBehaviour<Skill>;
                skillView.Initialize(skill, interactionView, costView);
                if (exists)
                    costView.transform.SetParent(skillView.transform, false);
                
                // register events
                skillView.OnInteractionStarted += view => OnSkillPressed?.Invoke(view);
                skillView.OnInteractionEnded += view => OnInteractionEnded?.Invoke(view);
                
                return skillView;
            });

            return skillViews;
        }

        private void LoadInteractionViews()
        {
            var children = _interactionViewsContainer.GetComponentsInChildren<IInteractionView>();
            _interactionViews = new List<IInteractionView>(children);
            _interactionViews.ForEach(interactionView => interactionView.Initialize());
        }
    }
}