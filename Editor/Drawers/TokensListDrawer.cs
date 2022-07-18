using System.Collections.Generic;
using System.Linq;
using Match3.Core;
using Match3.Core.Collections;
using UnityAtoms.BaseAtoms;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Match3.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(SelectableTokensList))]
    public class TokensListDrawer : PropertyDrawer
    {
        private float _currentWidth;
        private readonly Color _selectedColor = new Color(1,1,0.7f, 0.4f);
        private readonly Color _highlightedColor = new Color(1,1,1, 0.2f);
        
        class TokensGroupInfo
        {
            public string Name;
            public IList<TokenData> Tokens;
            public bool FoldOpen;

            public TokensGroupInfo(string name, IList<TokenData> tokens)
            {
                Name = name;
                Tokens = tokens;
            }
        }
        
        private static readonly List<TokensGroupInfo> TokensGroups = LoadTokens();
        
        private static List<TokensGroupInfo> LoadTokens()
        {
            var groups = new List<TokensGroupInfo>();
            var groupsAssets = AssetDatabase.FindAssets("t:TokenDataValueList")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<TokenDataValueList>)
                .ToList();

            var tokens = new List<TokenData>();
            foreach (var groupAsset in groupsAssets)
            {
                var group  = new TokensGroupInfo(groupAsset.name, groupAsset);
                groups.Add(group);
                tokens.AddRange(groupAsset);
            }
            
            var allTokens = AssetDatabase.FindAssets("t:TokenData")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<TokenData>)
                .ToList();
            var others = allTokens.FindAll(token => !tokens.Contains(token));
            groups.Add(new TokensGroupInfo("Others", others));
            
            return groups;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = 0;
            foreach (var group in TokensGroups)
            {
                height += GetGroupHeight(group, property, label);
            }
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // label
            var controlRect = EditorGUI.PrefixLabel(position, label);
            _currentWidth = controlRect.width > 0 ? controlRect.width : _currentWidth;
            float y = controlRect.y;

            // draw tokens groups
            foreach (var groupInfo in TokensGroups)
            {
                float height = GetGroupHeight(groupInfo, property, label);
                float tokensHeight = GetGroupTokensHeight(groupInfo, property, label);
                float foldoutHeaderHeight = base.GetPropertyHeight(null, null);
                var rect = new Rect(controlRect.x, y, controlRect.width, foldoutHeaderHeight);
                var tokensRect = new Rect(controlRect.x, y + foldoutHeaderHeight, controlRect.width, tokensHeight);

                int containsCount = GetIntersectionCount(groupInfo.Tokens, property);
                string name = $"{groupInfo.Name} ({containsCount})";
                groupInfo.FoldOpen = EditorGUI.BeginFoldoutHeaderGroup(rect, groupInfo.FoldOpen, name);

                if (groupInfo.FoldOpen)
                {
                    // selectable tokens
                    var (rows, cols) = GetRowsAndColumns(property, label, groupInfo.Tokens.Count);
                    var tokenSize = GetTokenSize(property, label);
                    DrawTokensList(property, groupInfo.Tokens, cols, tokenSize, tokensRect);
                }
                
                EditorGUI.EndFoldoutHeaderGroup();

                y += height;
            }
        }
        
        private void DrawTokensList(SerializedProperty property, IList<TokenData> tokens, int cols, float tokenSize, Rect controlRect)
        {
            // box
            GUI.Box(controlRect, "", GUI.skin.textArea);
            var @event = Event.current;
            var mousePos = @event.mousePosition;
            for (int i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];

                // draw token's sprite
                int col = i % cols;
                int row = i / cols;
                float x = col * tokenSize + controlRect.position.x;
                float y = row * tokenSize + controlRect.position.y;
                var rect = new Rect(x, y, tokenSize, tokenSize);
                var texture = token.TokenSprite == null
                    ? Texture2D.grayTexture
                    : AssetPreview.GetAssetPreview(token.TokenSprite);
                EditorGUI.DrawTextureTransparent(rect, texture, ScaleMode.ScaleToFit, 1);
                GUI.Box(rect, new GUIContent{tooltip = $"{token.name}"});

                // add or remove
                bool inside = rect.Contains(mousePos);
                bool mouseDown = @event.type == EventType.MouseDown;
                if (mouseDown && inside)
                {
                    ToggleTokenContainState(property, token);
                }

                // draw selected highlight
                if (ContainsToken(property, token, out int index))
                {
                    EditorGUI.DrawRect(rect, _selectedColor);
                }
                else if (inside)
                {
                    // TODO OnGUI doesn't get called when move mouse over control, just on enter and exit
//                    EditorGUI.DrawRect(rect, _highlightedColor);
                }
            }
        }

        private (int, int) GetRowsAndColumns(SerializedProperty property, GUIContent label, int tokensCount)
        {
            float tokenSize = GetTokenSize(property, label);
            float minSize = tokenSize * 2;
            float width = Mathf.Max(minSize, _currentWidth);
            int columns = (int) (width / tokenSize);
            int rows = Mathf.CeilToInt((float) tokensCount / columns);
            return (rows, columns);
        }

        private float GetTokenSize(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) * 2;
        }

        private float GetGroupHeight(TokensGroupInfo group, SerializedProperty property, GUIContent label)
        {
            float height = base.GetPropertyHeight(property, label);
            if (group.FoldOpen)
            {
                height += GetGroupTokensHeight(@group, property, label);
            }
            return height;
        }

        private float GetGroupTokensHeight(TokensGroupInfo @group, SerializedProperty property, GUIContent label)
        {
            int tokensCount = @group.Tokens.Count;
            var (rows, _) = GetRowsAndColumns(property, label, tokensCount);
            float height = GetTokenSize(property, label) * rows;
            return height;
        }

        private bool ContainsToken(SerializedProperty property, TokenData tokenData, out int index)
        {
            var arrayProp = property.FindPropertyRelative("_data");
            int len = arrayProp.arraySize;
            for (int i = 0; i < len; i++)
            {
                var tokenProperty = arrayProp.GetArrayElementAtIndex(i);
                if (tokenProperty.objectReferenceValue == tokenData)
                {
                    index = i;
                    return true;
                }
            }

            index = -1;
            return false;
        }

        private int GetIntersectionCount(IList<TokenData> tokensGroup, SerializedProperty property)
        {
            var arrayProp = property.FindPropertyRelative("_data");
            int len = arrayProp.arraySize;
            int count = 0;
            for (int i = 0; i < len; i++)
            {
                var tokenProperty = arrayProp.GetArrayElementAtIndex(i);
                if (tokensGroup.Contains(tokenProperty.objectReferenceValue))
                {
                    count++;
                }
            }

            return count;
        }
        
        private void ToggleTokenContainState(SerializedProperty property, TokenData token)
        {
            var arrayProp = property.FindPropertyRelative("_data");
            bool exists = ContainsToken(property, token, out int index);
            if (exists)  // remove token
            {
                arrayProp.DeleteArrayElementAtIndex(index);
            }
            else  // add token
            {
                arrayProp.InsertArrayElementAtIndex(arrayProp.arraySize);
                var element = arrayProp.GetArrayElementAtIndex(arrayProp.arraySize - 1);
                element.objectReferenceValue = token;
            }

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}