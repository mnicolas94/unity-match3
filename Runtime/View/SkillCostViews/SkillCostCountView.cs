using Match3.Core.GameActions;
using ModelView;
using TMPro;
using UnityEngine;

namespace Match3.View.SkillCostViews
{
    public class SkillCostCountView : ViewBaseBehaviour<Skill>
    {
        [SerializeField] private TextMeshProUGUI _countText;
        
        public override bool CanRenderModel(Skill model)
        {
            return model.SkillCost is SkillCostCount;
        }

        public override void Initialize(Skill model)
        {
            UpdateView(model);
        }

        public override void UpdateView(Skill model)
        {
            int remaining = GetCost(model).GetRemainingCount(model);
            _countText.text = remaining.ToString();
        }

        private SkillCostCount GetCost(Skill skill)
        {
            return (SkillCostCount) skill.SkillCost;
        }
    }
}