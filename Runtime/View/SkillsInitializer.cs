﻿using System.Collections.Generic;
using Match3.Core.GameActions;
using Match3.View.Interactions;
using Match3.View.SkillCostViews;
using UnityEngine;

namespace Match3.View
{
    public class SkillsInitializer : MonoBehaviour
    {
        [SerializeField] private List<Skill> _skills;
        [SerializeField] private SkillCostViewsProvider _costViewsProvider;
        [SerializeField] private Transform _skillsContainer;
        [SerializeField] private Transform _interactionViewsContainer;

        [SerializeField] private SkillView _skillViewPrefab;
        
        private List<IInteractionView> _interactionViews;
        
        public List<SkillView> InitializeSkills()
        {
            LoadInteractionViews();
            
            var skillViews = _skills.ConvertAll(skill =>
            {
                var action = skill.GameAction;
                var interactionView = _interactionViews.Find(i => i.CanProvideInteractionForAction(action));
                var skillView = Instantiate(_skillViewPrefab, _skillsContainer);
                var skillCostView = _costViewsProvider.TryGetViewForModel(skill, out bool exists);
                skillView.Initialize(skill, interactionView, skillCostView);
                if (exists)
                    skillCostView.transform.SetParent(skillView.transform, false);
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