using System.Linq;
using Match3.Core.LevelEditor;
using Match3.Core.Levels;
using Match3.View;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Match3.Editor
{
    public static class LevelUtils
    {
        [MenuItem("Assets/Facticus/Match3/Levels/Overwrite from level editor", true)]
        [MenuItem("Assets/Facticus/Match3/Levels/Edit level %#e", true)]
        [MenuItem("Assets/Facticus/Match3/Levels/Populate board with level %#w", true)]
        [MenuItem("Assets/Facticus/Match3/Levels/Play level %#d", true)]
        [MenuItem("Assets/Facticus/Match3/Simulation/Simulate games in level", true)]
        [MenuItem("Assets/Facticus/Match3/Simulation/Compute match statistics", true)]
        public static bool EditLevelValidator()
        {
            var selectedLevels = Selection.GetFiltered<Level>(SelectionMode.Assets);
            return selectedLevels.Length == 1;
        }
        
        [MenuItem("Tools/Facticus/Match3/Levels/Create from level editor")]
        public static void CreateLevelFromEditor()
        {
            var levelEditor = GetLevelEditor();
            if (levelEditor)
            {
                var level = levelEditor.CreateLevel();
                CreateLevelAsset(level);
            }
        }
        
        [MenuItem("Assets/Facticus/Match3/Levels/Overwrite from level editor", false)]
        public static void OverwriteLevel()
        {
            var (level, isSelected) = TryGetSelectedLevel();
            if (isSelected)
            {
                var levelEditor = GetLevelEditor();
                levelEditor.OverwriteLevel(level);
            }
        }
        
        [MenuItem("Assets/Facticus/Match3/Levels/Edit level %#e", false)]
        public static void EditLevel()
        {
            var (level, isSelected) = TryGetSelectedLevel();
            if (isSelected)
            {
                var levelEditor = GetLevelEditor();
                levelEditor.InitializeFromLevel(level);
            }
        }
        
        [MenuItem("Assets/Facticus/Match3/Levels/Populate board with level %#w", false, 11)]
        public static void PopulateInBoard()
        {
            var (level, isSelected) = TryGetSelectedLevel();
            if (isSelected)
            {
                var controller = GetGameControllerView();
                controller.StartGameInLevel(level);
            }
        }
        
        [MenuItem("Assets/Facticus/Match3/Levels/Play level %#d", false, 11)]
        public static void PlayLevel()
        {
            var (level, isSelected) = TryGetSelectedLevel();
            if (isSelected)
            {
                if (Application.isPlaying)
                {
                    var controller = GetGameControllerView();
                    controller.StartGameInLevel(level);
                }
                else
                {
                    EditorApplication.EnterPlaymode();
                    var initializer = GetBoardInitializer();
                    initializer.Level = level;
                    EditorUtility.SetDirty(initializer);
                    AssetDatabase.SaveAssets();
                }
            }
        }
        
        [MenuItem("Tools/Facticus/Match3/Levels/Find similar levels")]
        public static void FindSimilarLevels()
        {
            float threshold = 0.1f;
            var paths = new[]
            {
                "Assets/Data/Levels/",
                "Assets/Data/Levels-test/",
            };
            var levelsGuids = AssetDatabase.FindAssets("t:Level", paths);
            var levels = levelsGuids.Select(guid =>
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var level = AssetDatabase.LoadAssetAtPath<Level>(path);
                return level;
            }).ToList();
            for (int i = 0; i < levels.Count - 1; i++)
            {
                var levelA = levels[i];
                for (int j = i + 1; j < levels.Count; j++)
                {
                    var levelB = levels[j];
                    float similarity = Level.Similarity(levelA, levelB);
                    if (similarity > threshold)
                    {
                        Debug.LogWarning(
                            $"<color=#FFA100>{levelA.name}</color> y <color=#00A1FF>{levelB.name}</color>" +
                            $" tienen un {similarity * 100}% de parecido");
                    }
                }
            }
        }
        
        internal static (Level, bool) TryGetSelectedLevel()
        {
            var selectedLevels = Selection.GetFiltered<Level>(SelectionMode.Assets);
            if (selectedLevels.Length > 0)
            {
                return (selectedLevels[0], true);
            }

            return (default, false);
        }
        
        private static void CreateLevelAsset(Level level)
        {
            string path = "Assets/Data/Levels/level.asset";
            ProjectWindowUtil.CreateAsset(level, path);
            EditorUtility.SetDirty(level);
            AssetDatabase.SaveAssets();
        }

        private static LevelEditor GetLevelEditor()
        {
            return Object.FindObjectOfType<LevelEditor>();
        }

        private static GameControllerView GetGameControllerView()
        {
            return Object.FindObjectOfType<GameControllerView>();
        }
        
        private static BoardLevelInitializer GetBoardInitializer()
        {
            return Object.FindObjectOfType<BoardLevelInitializer>();
        }
    }
}