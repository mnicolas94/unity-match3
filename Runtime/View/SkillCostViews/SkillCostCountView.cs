using System;
using Match3.Core.GameActions;
using TMPro;
using UnityAtoms;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Match3.View.SkillCostViews
{
    public class SkillCostCountView : SkillCostViewBase, IAtomListener
    {
        [SerializeField] private TextMeshProUGUI _countText;

        private Skill _skill;
        private VoidEvent ChangeEvent => ((SkillCostCount) _skill.SkillCost).Storage.OnStorageChange;
        
        public override bool CanRenderModel(Skill model)
        {
            return model.SkillCost is SkillCostCount;
        }

        public override void Initialize(Skill model)
        {
            _skill = model;
            ChangeEvent.RegisterListener(this);
            UpdateView(model);
        }

        private void OnDisable()
        {
            ChangeEvent.UnregisterListener(this);
        }

        public override void UpdateView(Skill model)
        {
            int remaining = GetCost(model).GetRemainingCount(model);
            var text = remaining == 0 ? "+" : remaining.ToString();
            _countText.text = text;
        }

        private SkillCostCount GetCost(Skill skill)
        {
            return (SkillCostCount) skill.SkillCost;
        }

        public void OnEventRaised()
        {
            UpdateView(_skill);
        }
    }
}