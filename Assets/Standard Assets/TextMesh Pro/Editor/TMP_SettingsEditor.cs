// Copyright (C) 2014 - 2016 Stephan Bouchard - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms


using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;



namespace TMPro.EditorUtilities
{

    [CustomEditor(typeof(TMP_Settings))]
    public class TMP_SettingsEditor : Editor
    {
        //private struct UI_PanelState
        //{

        //}

        //private string[] uiStateLabel = new string[] { "<i>(Click to expand)</i>", "<i>(Click to collapse)</i>" };
        //private GUIStyle _Label;


        private SerializedProperty prop_FontAsset;
        private SerializedProperty prop_SpriteAsset;
        private SerializedProperty prop_StyleSheet;
        private ReorderableList m_list;

        private SerializedProperty prop_WordWrapping;
        private SerializedProperty prop_Kerning;
        private SerializedProperty prop_ExtraPadding;
        private SerializedProperty prop_TintAllSprites;
        private SerializedProperty prop_ParseEscapeCharacters;
        private SerializedProperty prop_MissingGlyphCharacter;

        private SerializedProperty prop_WarningsDisabled;

        private SerializedProperty prop_LeadingCharacters;
        private SerializedProperty prop_FollowingCharacters;



        public void OnEnable()
        {
            prop_FontAsset = serializedObject.FindProperty("m_defaultFontAsset");
            prop_SpriteAsset = serializedObject.FindProperty("m_defaultSpriteAsset");
            prop_StyleSheet = serializedObject.FindProperty("m_defaultStyleSheet");

            m_list = new ReorderableList(serializedObject, serializedObject.FindProperty("m_fallbackFontAssets"), true, true, true, true);

            m_list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = m_list.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                EditorGUI.PropertyField( new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
            };

            m_list.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "<b>Fallback Font Asset List</b>", TMP_UIStyleManager.Label);
            };

            prop_WordWrapping = serializedObject.FindProperty("m_enableWordWrapping");
            prop_Kerning = serializedObject.FindProperty("m_enableKerning");
            prop_ExtraPadding = serializedObject.FindProperty("m_enableExtraPadding");
            prop_TintAllSprites = serializedObject.FindProperty("m_enableTintAllSprites");
            prop_ParseEscapeCharacters = serializedObject.FindProperty("m_enableParseEscapeCharacters");
            prop_MissingGlyphCharacter = serializedObject.FindProperty("m_missingGlyphCharacter");

            prop_WarningsDisabled = serializedObject.FindProperty("m_warningsDisabled");

            prop_LeadingCharacters = serializedObject.FindProperty("m_leadingCharacters");
            prop_FollowingCharacters = serializedObject.FindProperty("m_followingCharacters");

            // Get the UI Skin and Styles for the various Editors
            TMP_UIStyleManager.GetUIStyles();
        }

        public override void OnInspectorGUI()
        {
            //Event evt = Event.current;

            serializedObject.Update();

            GUILayout.Label("<b>TEXTMESH PRO - SETTINGS</b>", TMP_UIStyleManager.Section_Label);

            // TextMeshPro Font Info Panel
            EditorGUI.indentLevel = 0;

            //GUI.enabled = false; // Lock UI

            EditorGUIUtility.labelWidth = 135;
            //EditorGUIUtility.fieldWidth = 80;

            // FONT ASSET
            EditorGUILayout.BeginVertical(TMP_UIStyleManager.SquareAreaBox85G);
            GUILayout.Label("<b>Default Font Asset</b>", TMP_UIStyleManager.Label);
            GUILayout.Label("Select the Font Asset that will be assigned by default to newly created text objects when no Font Asset is specified.", TMP_UIStyleManager.Label);
            GUILayout.Space(5f);
            EditorGUILayout.PropertyField(prop_FontAsset);
            EditorGUILayout.EndVertical();


            // FALLBACK FONT ASSETs
            EditorGUILayout.BeginVertical(TMP_UIStyleManager.SquareAreaBox85G);
            GUILayout.Label("<b>Fallback Font Assets</b>", TMP_UIStyleManager.Label);
            GUILayout.Label("Select the Font Assets that will be searched to locate and replace missing characters from a given Font Asset.", TMP_UIStyleManager.Label);
            GUILayout.Space(5f);
            m_list.DoLayoutList();
            EditorGUILayout.EndVertical();


            // TEXT OBJECT DEFAULT PROPERTIES
            EditorGUILayout.BeginVertical(TMP_UIStyleManager.SquareAreaBox85G);
            GUILayout.Label("<b>New Text Object Default Settings</b>", TMP_UIStyleManager.Label);
            GUILayout.Label("Default settings used by all new text objects.", TMP_UIStyleManager.Label);
            GUILayout.Space(5f);
            EditorGUI.BeginChangeCheck();

            EditorGUIUtility.labelWidth = 170;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(prop_WordWrapping);
            EditorGUILayout.PropertyField(prop_Kerning);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(prop_ExtraPadding);
            EditorGUILayout.PropertyField(prop_TintAllSprites);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(prop_ParseEscapeCharacters, new GUIContent("Parse Escape Sequence"));
            EditorGUIUtility.fieldWidth = 10;
            EditorGUILayout.PropertyField(prop_MissingGlyphCharacter, new GUIContent("Missing Glyph Replacement"));
            EditorGUILayout.EndHorizontal();

            EditorGUIUtility.labelWidth = 135;
            GUILayout.Space(10f);
            GUILayout.Label("<b>Disable warnings for missing glyphs on text objects.</b>", TMP_UIStyleManager.Label);
            EditorGUILayout.PropertyField(prop_WarningsDisabled, new GUIContent("Disable warnings"));
            EditorGUILayout.EndVertical();


            // SPRITE ASSET
            EditorGUILayout.BeginVertical(TMP_UIStyleManager.SquareAreaBox85G);
            GUILayout.Label("<b>Default Sprite Asset</b>", TMP_UIStyleManager.Label);
            GUILayout.Label("Select the Sprite Asset that will be assigned by default when using the <sprite> tag when no Sprite Asset is specified.", TMP_UIStyleManager.Label);
            GUILayout.Space(5f);
            EditorGUILayout.PropertyField(prop_SpriteAsset);
            EditorGUILayout.EndVertical();


            // STYLE SHEET
            EditorGUILayout.BeginVertical(TMP_UIStyleManager.SquareAreaBox85G);
            GUILayout.Label("<b>Default Style Sheet</b>", TMP_UIStyleManager.Label);
            GUILayout.Label("Select the Style Sheet that will be used for all text objects in this project.", TMP_UIStyleManager.Label);
            GUILayout.Space(5f);
            EditorGUILayout.PropertyField(prop_StyleSheet);
            EditorGUILayout.EndVertical();


            // LINE BREAKING RULE
            EditorGUILayout.BeginVertical(TMP_UIStyleManager.SquareAreaBox85G);
            GUILayout.Label("<b>Line Breaking Resources for Asian languages</b>", TMP_UIStyleManager.Label);
            GUILayout.Label("Select the text assets that contain the Leading and Following characters which define the rules for line breaking with Asian languages.", TMP_UIStyleManager.Label);
            GUILayout.Space(5f);
            EditorGUILayout.PropertyField(prop_LeadingCharacters);
            EditorGUILayout.PropertyField(prop_FollowingCharacters);
            EditorGUILayout.EndVertical();


            if (serializedObject.ApplyModifiedProperties())
            {
                EditorUtility.SetDirty(target);
                TMPro_EventManager.ON_TMP_SETTINGS_CHANGED();
            }

        }
    }
}