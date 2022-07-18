using System.IO;
using Match3.Core;
using Match3.Core.Tiles;
using UnityEditor;
using UnityEngine;

namespace Match3.Editor
{
    public static class TokenCreationUtils
    {
        [MenuItem("Assets/Create/Facticus/Match3/Token")]
        public static void CreateToken()
        {
            var tokenName = EditorInputDialog.Show(
                "Token creation",
                "Enter the token name:",
                "Token");
            var obj = Selection.activeObject;
            var path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path))
            {
                if (File.Exists(path))
                    path = Path.GetDirectoryName(path);
                CreateToken(path, tokenName, out _, out _);
            }
        }

        public static bool CreateToken(string parentFolder, string tokenName, out TokenData token, out TileToken tileToken)
        {
            string folder = Path.Combine(parentFolder, tokenName);
            // create paths
            string tokenPath = $"{Path.Combine(folder, tokenName)}.asset";
            string tileTokenPath = $"{Path.Combine(folder, $"tile{tokenName}")}.asset";
            
            if (Directory.Exists(folder))
            {
                Debug.LogWarning($"A token with name {tokenName} already exists in folder {parentFolder}");
                token = AssetDatabase.LoadAssetAtPath<TokenData>(tokenPath);
                tileToken = AssetDatabase.LoadAssetAtPath<TileToken>(tileTokenPath);
                return false;
            }
            Directory.CreateDirectory(folder);
            
            // create objects
            token = ScriptableObject.CreateInstance<TokenData>();
            tileToken = TileToken.CreateFromTokenData(token);
            
            // create assets
            AssetDatabase.CreateAsset(token, tokenPath);
            AssetDatabase.CreateAsset(tileToken, tileTokenPath);
            
            AssetDatabase.SaveAssets();
            Selection.activeObject = token;

            return true;
        }
    }
}