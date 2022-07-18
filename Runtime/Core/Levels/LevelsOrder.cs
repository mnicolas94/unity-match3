using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utils;
#if UNITY_EDITOR

#endif

namespace Match3.Core.Levels
{
    [CreateAssetMenu(fileName = "LevelsOrder", menuName = "Facticus/Match3/Levels/LevelsOrder", order = 0)]
    public class LevelsOrder : ScriptableObjectSingleton<LevelsOrder>
    {
        [SerializeField] private List<Level> _levels;

        public int MaxLevelIndex => _levels.Count - 1;
        
        public Level LevelAt(int index)
        {
            int clampedIndex = Mathf.Clamp(index, 0, MaxLevelIndex);
            return _levels[clampedIndex];
        }
        
        public int IndexOf(Level level)
        {
            return _levels.IndexOf(level);
        }

#if UNITY_EDITOR
        [ContextMenu("Get from folder")]
        private void GetFromFolder()
        {
            _levels.Clear();
            
            var foldersPaths = new []{ "Assets/Data/Levels/" };
            var guids = AssetDatabase.FindAssets("t:Level", foldersPaths);
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var level = AssetDatabase.LoadAssetAtPath<Level>(path);
                _levels.Add(level);
            }
        }
#endif
    }
}