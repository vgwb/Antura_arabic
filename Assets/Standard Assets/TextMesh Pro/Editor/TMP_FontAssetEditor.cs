// Copyright (C) 2014 - 2016 Stephan Bouchard - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms


using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;



namespace TMPro.EditorUtilities
{

    [CustomPropertyDrawer(typeof(TMP_FontWeights))]
    public class FontWeightDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty prop_regular = property.FindPropertyRelative("regularTypeface");
            SerializedProperty prop_italic = property.FindPropertyRelative("italicTypeface");

            float width = position.width;

            position.width = 125;
            EditorGUI.LabelField(position, label);

            // NORMAL FACETYPE
            if (label.text[0] == '4') GUI.enabled = false;
            position.x = 140; position.width = (width - 140) / 2;
            EditorGUI.PropertyField(position, prop_regular, GUIContent.none);

            // ITALIC FACETYPE
            GUI.enabled = true;
            position.x += position.width + 17;
            EditorGUI.PropertyField(position, prop_italic, GUIContent.none);
        }
    }



    [CustomEditor(typeof(TMP_FontAsset))]
    public class TMP_FontAssetEditor : Editor
    {
        private struct UI_PanelState
        {
            public static bool fontInfoPanel = true;
            public static bool fontWeightPanel = true;
            public static bool fallbackFontAssetPanel = true;
            public static bool glyphInfoPanel = false;
            public static bool kerningInfoPanel = false;
        }

        private int m_GlyphPage = 0;
        private int m_KerningPage = 0;


        private const string k_UndoRedo = "UndoRedoPerformed";

        private SerializedProperty font_atlas_prop;
        private SerializedProperty font_material_prop;

        private SerializedProperty fontWeights_prop;

        //private SerializedProperty fallbackFontAssets_prop;
        private ReorderableList m_list;

        private SerializedProperty font_normalStyle_prop;
        private SerializedProperty font_normalSpacing_prop;

        private SerializedProperty font_boldStyle_prop;
        private SerializedProperty font_boldSpacing_prop;

        private SerializedProperty font_italicStyle_prop;
        private SerializedProperty font_tabSize_prop;

        private SerializedProperty m_fontInfo_prop;
        private SerializedProperty m_glyphInfoList_prop;

        private SerializedProperty m_kerningInfo_prop;
        private KerningTable m_kerningTable;

        private SerializedProperty m_kerningPair_prop;


        private TMP_FontAsset m_fontAsset;

        private bool isAssetDirty = false;

        private int errorCode;

        private System.DateTime timeStamp;

        private string[] uiStateLabel = new string[] { "<i>(Click to expand)</i>", "<i>(Click to collapse)</i>" };

        public void OnEnable()
        {
            font_atlas_prop = serializedObject.FindProperty("atlas");
            font_material_prop = serializedObject.FindProperty("material");

            fontWeights_prop = serializedObject.FindProperty("fontWeights");

            m_list = new ReorderableList(serializedObject, serializedObject.FindProperty("fallbackFontAssets"), true, true, true, true);

            m_list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = m_list.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
            };

            m_list.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "<b>Fallback Font Asset List</b>", TMP_UIStyleManager.Label);
            };

            font_normalStyle_prop = serializedObject.FindProperty("normalStyle");
            font_normalSpacing_prop = serializedObject.FindProperty("normalSpacingOffset");

            font_boldStyle_prop = serializedObject.FindProperty("boldStyle");
            font_boldSpacing_prop = serializedObject.FindProperty("boldSpacing");

            font_italicStyle_prop = serializedObject.FindProperty("italicStyle");
            font_tabSize_prop = serializedObject.FindProperty("tabSize");

            m_fontInfo_prop = serializedObject.FindProperty("m_fontInfo");
            m_glyphInfoList_prop = serializedObject.FindProperty("m_glyphInfoList");
            m_kerningInfo_prop = serializedObject.FindProperty("m_kerningInfo");
            m_kerningPair_prop = serializedObject.FindProperty("m_kerningPair");

            //m_isGlyphInfoListExpanded_prop = serializedObject.FindProperty("isGlyphInfoListExpanded");
            //m_isKerningTableExpanded_prop = serializedObject.FindProperty("isKerningTableExpanded");

            m_fontAsset = target as TMP_FontAsset;
            m_kerningTable = m_fontAsset.kerningInfo;

            // Get the UI Skin and Styles for the various Editors
            TMP_UIStyleManager.GetUIStyles();
        }

        public override void OnInspectorGUI()
        {

            //Debug.Log("OnInspectorGUI Called.");
            Event evt = Event.current;

            serializedObject.Update();

            GUILayout.Label("<b>TextMesh Pro! Font Asset</b>", TMP_UIStyleManager.Section_Label);

            // TextMeshPro Font Info Panel
            GUILayout.Label("Face Info", TMP_UIStyleManager.Section_Label);
            EditorGUI.indentLevel = 1;

            GUI.enabled = false; // Lock UI

            float labelWidth = EditorGUIUtility.labelWidth = 150f;
            float fieldWidth = EditorGUIUtility.fieldWidth;

            EditorGUILayout.PropertyField(m_fontInfo_prop.FindPropertyRelative("Name"), new GUIContent("Font Source"));
            EditorGUILayout.PropertyField(m_fontInfo_prop.FindPropertyRelative("PointSize"));

            GUI.enabled = true;
            EditorGUILayout.PropertyField(m_fontInfo_prop.FindPropertyRelative("Scale"));
            EditorGUILayout.PropertyField(m_fontInfo_prop.FindPropertyRelative("LineHeight"));

            EditorGUILayout.PropertyField(m_fontInfo_prop.FindPropertyRelative("Baseline"));
            EditorGUILayout.PropertyField(m_fontInfo_prop.FindPropertyRelative("Ascender"));
            EditorGUILayout.PropertyField(m_fontInfo_prop.FindPropertyRelative("Descender"));
            EditorGUILayout.PropertyField(m_fontInfo_prop.FindPropertyRelative("Underline"));
            //EditorGUILayout.PropertyField(m_fontInfo_prop.FindPropertyRelative("UnderlineThickness"));
            EditorGUILayout.PropertyField(m_fontInfo_prop.FindPropertyRelative("SuperscriptOffset"));
            EditorGUILayout.PropertyField(m_fontInfo_prop.FindPropertyRelative("SubscriptOffset"));
            
            SerializedProperty subSize_prop = m_fontInfo_prop.FindPropertyRelative("SubSize");
            EditorGUILayout.PropertyField(subSize_prop, new GUIContent("Super / Subscript Size"));
            subSize_prop.floatValue = Mathf.Clamp(subSize_prop.floatValue, 0.25f, 1f);
            

            GUI.enabled = false;
            //EditorGUILayout.PropertyField(m_fontInfo_prop.FindPropertyRelative("Padding"));

            //GUILayout.Label("Atlas Size");
            EditorGUI.indentLevel = 1;
            GUILayout.Space(18);
            EditorGUILayout.PropertyField(m_fontInfo_prop.FindPropertyRelative("Padding"));
            EditorGUILayout.PropertyField(m_fontInfo_prop.FindPropertyRelative("AtlasWidth"), new GUIContent("Width"));
            EditorGUILayout.PropertyField(m_fontInfo_prop.FindPropertyRelative("AtlasHeight"), new GUIContent("Height"));

            GUI.enabled = true;
            EditorGUI.indentLevel = 0;
            GUILayout.Space(20);
            GUILayout.Label("Font Sub-Assets", TMP_UIStyleManager.Section_Label);
                  
            GUI.enabled = false;
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(font_atlas_prop, new GUIContent("Font Atlas:"));
            EditorGUILayout.PropertyField(font_material_prop, new GUIContent("Font Material:"));

            GUI.enabled = true;

            string evt_cmd = Event.current.commandName; // Get Current Event CommandName to check for Undo Events

            // FONT SETTINGS
            EditorGUI.indentLevel = 0;
            if (GUILayout.Button("Font Weights\t" + (UI_PanelState.fontWeightPanel ? uiStateLabel[1] : uiStateLabel[0]), TMP_UIStyleManager.Section_Label))
                UI_PanelState.fontWeightPanel = !UI_PanelState.fontWeightPanel;


            if (UI_PanelState.fontWeightPanel)
            {
                EditorGUIUtility.labelWidth = 120;
                EditorGUILayout.BeginVertical(TMP_UIStyleManager.SquareAreaBox85G);
                EditorGUI.indentLevel = 0;
                GUILayout.Label("Select the Font Assets that will be used for the following font weights.", TMP_UIStyleManager.Label);
                GUILayout.Space(10f);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("<b>Font Weight</b>", TMP_UIStyleManager.Label, GUILayout.Width(117));
                GUILayout.Label("<b>Normal Style</b>", TMP_UIStyleManager.Label);
                GUILayout.Label("<b>Italic Style</b>", TMP_UIStyleManager.Label);
                EditorGUILayout.EndHorizontal();

                //EditorGUILayout.PropertyField(fontWeights_prop.GetArrayElementAtIndex(1), new GUIContent("100 - Thin"));
                //EditorGUILayout.PropertyField(fontWeights_prop.GetArrayElementAtIndex(2), new GUIContent("200 - Extra-Light"));
                //EditorGUILayout.PropertyField(fontWeights_prop.GetArrayElementAtIndex(3), new GUIContent("300 - Light"));
                EditorGUILayout.PropertyField(fontWeights_prop.GetArrayElementAtIndex(4), new GUIContent("400 - Regular"));
                //EditorGUILayout.PropertyField(fontWeights_prop.GetArrayElementAtIndex(5), new GUIContent("500 - Medium"));
                //EditorGUILayout.PropertyField(fontWeights_prop.GetArrayElementAtIndex(6), new GUIContent("600 - Demi-Bold"));
                EditorGUILayout.PropertyField(fontWeights_prop.GetArrayElementAtIndex(7), new GUIContent("700 - Bold"));
                //EditorGUILayout.PropertyField(fontWeights_prop.GetArrayElementAtIndex(8), new GUIContent("800 - Heavy"));
                //EditorGUILayout.PropertyField(fontWeights_prop.GetArrayElementAtIndex(9), new GUIContent("900 - Black"));

                EditorGUILayout.EndVertical();

                //EditorGUI.indentLevel = 1;
                EditorGUIUtility.labelWidth = 120f;
                EditorGUILayout.BeginVertical(TMP_UIStyleManager.SquareAreaBox85G);
                GUILayout.Label("Settings used to simulate a typeface when no font asset is available.", TMP_UIStyleManager.Label);
                GUILayout.Space(5f);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(font_normalStyle_prop, new GUIContent("Normal Weight"));
                font_normalStyle_prop.floatValue = Mathf.Clamp(font_normalStyle_prop.floatValue, -3.0f, 3.0f);
                if (GUI.changed || evt_cmd == k_UndoRedo)
                {
                    GUI.changed = false;
                    Material mat = font_material_prop.objectReferenceValue as Material;
                    mat.SetFloat("_WeightNormal", font_normalStyle_prop.floatValue);
                }


                EditorGUILayout.PropertyField(font_boldStyle_prop, new GUIContent("Bold Weight"), GUILayout.MinWidth(100));
                font_boldStyle_prop.floatValue = Mathf.Clamp(font_boldStyle_prop.floatValue, -3.0f, 3.0f);
                if (GUI.changed || evt_cmd == k_UndoRedo)
                {
                    GUI.changed = false;
                    Material mat = font_material_prop.objectReferenceValue as Material;
                    mat.SetFloat("_WeightBold", font_boldStyle_prop.floatValue);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(font_normalSpacing_prop, new GUIContent("Spacing Offset"));
                font_normalSpacing_prop.floatValue = Mathf.Clamp(font_normalSpacing_prop.floatValue, -100, 100);
                if (GUI.changed || evt_cmd == k_UndoRedo)
                {
                    GUI.changed = false;
                }

                EditorGUILayout.PropertyField(font_boldSpacing_prop, new GUIContent("Bold Spacing"));
                font_boldSpacing_prop.floatValue = Mathf.Clamp(font_boldSpacing_prop.floatValue, 0, 100);
                if (GUI.changed || evt_cmd == k_UndoRedo)
                {
                    GUI.changed = false;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(font_italicStyle_prop, new GUIContent("Italic Style: "));
                font_italicStyle_prop.intValue = Mathf.Clamp(font_italicStyle_prop.intValue, 15, 60);

                EditorGUILayout.PropertyField(font_tabSize_prop, new GUIContent("Tab Multiple: "));

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }

            GUILayout.Space(5);

            // FALLBACK FONT ASSETS
            EditorGUI.indentLevel = 0;
            if (GUILayout.Button("Fallback Font Assets\t" + (UI_PanelState.fallbackFontAssetPanel ? uiStateLabel[1] : uiStateLabel[0]), TMP_UIStyleManager.Section_Label))
                UI_PanelState.fallbackFontAssetPanel = !UI_PanelState.fallbackFontAssetPanel;


            if (UI_PanelState.fallbackFontAssetPanel)
            {
                EditorGUIUtility.labelWidth = 120;
                EditorGUILayout.BeginVertical(TMP_UIStyleManager.SquareAreaBox85G);
                EditorGUI.indentLevel = 0;
                GUILayout.Label("Select the Font Assets that will be searched and used as fallback when characters are missing from this font asset.", TMP_UIStyleManager.Label);
                GUILayout.Space(10f);

                m_list.DoLayoutList();

                EditorGUILayout.EndVertical();
            }


                EditorGUIUtility.labelWidth = labelWidth;
            EditorGUIUtility.fieldWidth = fieldWidth;
            GUILayout.Space(5);
            EditorGUI.indentLevel = 0;

            if (GUILayout.Button("Glyph Info\t" + (UI_PanelState.glyphInfoPanel ? uiStateLabel[1] : uiStateLabel[0]), TMP_UIStyleManager.Section_Label))
                UI_PanelState.glyphInfoPanel = !UI_PanelState.glyphInfoPanel;


            if (UI_PanelState.glyphInfoPanel)
            {
                int arraySize = m_glyphInfoList_prop.arraySize;
                int itemsPerPage = 15;


                if (arraySize > 0)
                {
                    // Display each GlyphInfo entry using the GlyphInfo property drawer.
                    for (int i = itemsPerPage * m_GlyphPage; i < arraySize && i < itemsPerPage * (m_GlyphPage + 1); i++)
                    {
                        SerializedProperty glyphInfo = m_glyphInfoList_prop.GetArrayElementAtIndex(i);

                        EditorGUILayout.BeginVertical(TMP_UIStyleManager.Group_Label);

                        EditorGUILayout.PropertyField(glyphInfo);

                        EditorGUILayout.EndVertical();
                    }
                }

                Rect pagePos = EditorGUILayout.GetControlRect(false, 20);
                pagePos.width /= 2;

                int shiftMultiplier = evt.shift ? 10 : 1;

                if (m_GlyphPage > 0) GUI.enabled = true;
                else GUI.enabled = false;

                if (GUI.Button(pagePos, "Previous Page"))
                    m_GlyphPage -= 1 * shiftMultiplier;

                pagePos.x += pagePos.width;
                if (itemsPerPage * (m_GlyphPage + 1) < arraySize) GUI.enabled = true;
                else GUI.enabled = false;

                if (GUI.Button(pagePos, "Next Page"))
                    m_GlyphPage += 1 * shiftMultiplier;

                m_GlyphPage = Mathf.Clamp(m_GlyphPage, 0, arraySize / itemsPerPage);
            }


            // KERNING TABLE PANEL
            GUILayout.Space(5);
            if (GUILayout.Button("Kerning Table Info\t" + (UI_PanelState.kerningInfoPanel ? uiStateLabel[1] : uiStateLabel[0]), TMP_UIStyleManager.Section_Label))
                UI_PanelState.kerningInfoPanel = !UI_PanelState.kerningInfoPanel;


            if (UI_PanelState.kerningInfoPanel)
            {
                
                Rect pos;

                SerializedProperty kerningPairs_prop = m_kerningInfo_prop.FindPropertyRelative("kerningPairs");

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Left Char", TMP_UIStyleManager.TMP_GUISkin.label);
                GUILayout.Label("Right Char", TMP_UIStyleManager.TMP_GUISkin.label);
                GUILayout.Label("Offset Value", TMP_UIStyleManager.TMP_GUISkin.label);
                GUILayout.Label(GUIContent.none, GUILayout.Width(20));
                EditorGUILayout.EndHorizontal();

                GUILayout.BeginVertical(TMP_UIStyleManager.TMP_GUISkin.label);

                int arraySize = kerningPairs_prop.arraySize;
                int itemsPerPage = 25;

                if (arraySize > 0)
                {
                    // Display each GlyphInfo entry using the GlyphInfo property drawer.
                    for (int i = itemsPerPage * m_KerningPage; i < arraySize && i < itemsPerPage * (m_KerningPage + 1); i++)
                    {
                        SerializedProperty kerningPair_prop = kerningPairs_prop.GetArrayElementAtIndex(i);

                        pos = EditorGUILayout.BeginHorizontal();

                        EditorGUI.PropertyField(new Rect(pos.x, pos.y, pos.width - 20f, pos.height), kerningPair_prop, GUIContent.none);

                        // Button to Delete Kerning Pair
                        if (GUILayout.Button("-", GUILayout.ExpandWidth(false)))
                        {
                            m_kerningTable.RemoveKerningPair(i);
                            m_fontAsset.ReadFontDefinition(); // Reload Font Definition.
                            serializedObject.Update(); // Get an updated version of the SerializedObject.
                            isAssetDirty = true;
                            break;
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                }

                Rect pagePos = EditorGUILayout.GetControlRect(false, 20);
                pagePos.width /= 3;

                int shiftMultiplier = evt.shift ? 10 : 1;

                // Previous Page
                if (m_KerningPage > 0) GUI.enabled = true;
                else GUI.enabled = false;

                if (GUI.Button(pagePos, "Previous Page"))
                    m_KerningPage -= 1 * shiftMultiplier;

                // Page Counter
                GUI.enabled = true;
                pagePos.x += pagePos.width;
                int totalPages = (int)(arraySize / (float)itemsPerPage + 0.999f);
                GUI.Label(pagePos, "Page " + (m_KerningPage + 1) + " / " + totalPages, GUI.skin.button);

                // Next Page
                pagePos.x += pagePos.width;
                if (itemsPerPage * (m_GlyphPage + 1) < arraySize) GUI.enabled = true;
                else GUI.enabled = false;

                if (GUI.Button(pagePos, "Next Page"))
                    m_KerningPage += 1 * shiftMultiplier;

                m_KerningPage = Mathf.Clamp(m_KerningPage, 0, arraySize / itemsPerPage);

                GUILayout.EndVertical();

                GUILayout.Space(10);


                // Add New Kerning Pair Section
                GUILayout.BeginVertical(TMP_UIStyleManager.SquareAreaBox85G);

                pos = EditorGUILayout.BeginHorizontal();

                // Draw Empty Kerning Pair 
                EditorGUI.PropertyField(new Rect(pos.x, pos.y, pos.width - 20f, pos.height), m_kerningPair_prop);
                GUILayout.Label(GUIContent.none, GUILayout.Height(19));

                EditorGUILayout.EndHorizontal();

                GUILayout.Space(5);

                if (GUILayout.Button("Add New Kerning Pair"))
                {
                    int asci_left = m_kerningPair_prop.FindPropertyRelative("AscII_Left").intValue;
                    int asci_right = m_kerningPair_prop.FindPropertyRelative("AscII_Right").intValue;
                    float xOffset = m_kerningPair_prop.FindPropertyRelative("XadvanceOffset").floatValue;

                    errorCode = m_kerningTable.AddKerningPair(asci_left, asci_right, xOffset);

                    // Sort Kerning Pairs & Reload Font Asset if new kerning pair was added.
                    if (errorCode != -1)
                    {
                        m_kerningTable.SortKerningPairs();
                        m_fontAsset.ReadFontDefinition(); // Reload Font Definition.
                        serializedObject.Update(); // Get an updated version of the SerializedObject.
                        isAssetDirty = true;
                    }
                    else
                    {
                        timeStamp = System.DateTime.Now.AddSeconds(5);
                    }
                }

                if (errorCode == -1)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("Kerning Pair already <color=#ffff00>exists!</color>", TMP_UIStyleManager.Label);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    if (System.DateTime.Now > timeStamp)
                        errorCode = 0;
                }

                GUILayout.EndVertical();
            }


            if (serializedObject.ApplyModifiedProperties() || evt_cmd == k_UndoRedo || isAssetDirty)
            {
                //Debug.Log("Serialized properties have changed.");
                TMPro_EventManager.ON_FONT_PROPERTY_CHANGED(true, m_fontAsset);

                isAssetDirty = false;
                EditorUtility.SetDirty(target);
                //TMPro_EditorUtility.RepaintAll(); // Consider SetDirty
            }

        }
    }
}