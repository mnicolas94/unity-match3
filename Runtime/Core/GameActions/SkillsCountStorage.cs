using System.Linq;
using Match3.Core.SerializableDictionaries;
using UnityEngine;
using Utils;

namespace Match3.Core.GameActions
{
    [CreateAssetMenu(fileName = "SkillsCountPersistent", menuName = "Facticus/Match3/Persistent/SkillsCountStorage", order = 0)]
    public class SkillsCountStorage : ScriptableObject
    {
        [SerializeField] private SkillCountDictionary _skillsCount;

        public int GetSkillCount(Skill skill)
        {
            var dict = _skillsCount;
            return dict.ContainsKey(skill) ? dict[skill] : 0;
        }

        public void AddSkillCount(Skill skill, int count = 1)
        {
            var dict = _skillsCount;
            if (dict.ContainsKey(skill))
                dict[skill] += count;
            else
                dict.Add(skill, count);
        }
        
        public void AddAllSkillsCount(int count)
        {
            var dict = _skillsCount;
            foreach (var skill in dict.Keys.ToList())
            {
                AddSkillCount(skill, count);
            }
        }

        public bool ConsumeSkill(Skill skill)
        {
            if (GetSkillCount(skill) > 0)
            {
                _skillsCount[skill]--;
                return true;
            }

            return false;
        }
    }
}