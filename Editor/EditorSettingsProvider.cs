using Match3.Settings;
using UnityEditor;
using UnityEngine;
using Utils.Editor;

namespace Match3.Editor
{
    public static class EditorSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider GetSettings()
        {
            bool existsSettings = SimulationSettings.Instance != null;
            var so = existsSettings ? new SerializedObject(SimulationSettings.Instance) : null;
            var keywords = existsSettings ? SettingsProvider.GetSearchKeywordsFromSerializedObject(so) : new string[0];
            var provider = new SettingsProvider("Project/Facticus/Match3", SettingsScope.Project)
            {
                guiHandler = searchContext =>
                {
                    EditorGUILayout.Space(12);
                    
                    if (existsSettings)
                        PropertiesUtils.DrawSerializedObject(so);
                    else
                    {
                        var r = EditorGUILayout.GetControlRect();
                        if (GUI.Button(r, "Create settings"))
                        {
                            var settings = ScriptableObject.CreateInstance<SimulationSettings>();
                            AssetDatabase.CreateAsset(settings, "Assets/SimulationSettings.asset");
                        }
                    }
                },
                
                keywords = keywords
            };

            return provider;
        }
    }
}