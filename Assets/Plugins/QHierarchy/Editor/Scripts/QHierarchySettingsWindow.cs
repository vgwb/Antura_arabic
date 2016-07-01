using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace qtools.qhierarchy
{
	public class QHierarchySettingsWindow : EditorWindow 
	{	
        private Vector2 scrollPosition = new Vector2();

		[MenuItem ("Window/QTools/QHierarchy/Settings")]	
		public static void ShowWindow () 
		{ 
			EditorWindow window = EditorWindow.GetWindow(typeof(QHierarchySettingsWindow));           
            #if UNITY_4_6 || UNITY_4_7 || UNITY_5_0
                window.title = "QHierarchy";
            #else
                window.titleContent = new GUIContent("QHierarchy");
            #endif
		}

		void OnGUI()
		{
            bool fixedIconWidth = QHierarchySettings.getSetting<bool>(QHierarchySetting.FixedIconWidth);
            bool showErrorIcon = QHierarchySettings.getSetting<bool>(QHierarchySetting.ShowErrorIcon);
            bool showGameObjectIcon = QHierarchySettings.getSetting<bool>(QHierarchySetting.ShowGameObjectIcon);
            bool showTagAndLayerText = QHierarchySettings.getSetting<bool>(QHierarchySetting.ShowTagAndLayerText);
            bool showCustomTagIcon = QHierarchySettings.getSetting<bool>(QHierarchySetting.ShowCustomTagIcon);
            bool showLockButton = QHierarchySettings.getSetting<bool>(QHierarchySetting.ShowLockButton);
            bool showStaticIcon = QHierarchySettings.getSetting<bool>(QHierarchySetting.ShowStaticIcon);
            bool showMonoBehaviourIcon = QHierarchySettings.getSetting<bool>(QHierarchySetting.ShowMonoBehaviourIcon);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            {
                GUILayout.Space(5);

                if (drawTitle(QHierarchySetting.GeneralSettingsFoldout, " General Settings"))
                {
                    bool showVisibilityButton = QHierarchySettings.getSetting<bool>(QHierarchySetting.ShowVisibilityButton);
                    if (EditorGUILayout.ToggleLeft(" Show Visibility Button", showVisibilityButton) != showVisibilityButton)
                    {
                        QHierarchySettings.setSetting(QHierarchySetting.ShowVisibilityButton, !showVisibilityButton);   
                    }

                    if (EditorGUILayout.ToggleLeft(" Show Lock Button", showLockButton) != showLockButton)
                    {
                        QHierarchySettings.setSetting(QHierarchySetting.ShowLockButton, !showLockButton);                       
                        if (showLockButton) QHierarchy.unlockAll();                        
                    }

                    bool showMeshButton = QHierarchySettings.getSetting<bool>(QHierarchySetting.ShowMeshButton);
                    if (EditorGUILayout.ToggleLeft(" Show Renderer Button", showMeshButton) != showMeshButton)
                    {
                        QHierarchySettings.setSetting(QHierarchySetting.ShowMeshButton, !showMeshButton);   
                    }

                    if (EditorGUILayout.ToggleLeft(" Show MonoBehavior Icon", showMonoBehaviourIcon) != showMonoBehaviourIcon)
                    {
                        QHierarchySettings.setSetting(QHierarchySetting.ShowMonoBehaviourIcon, !showMonoBehaviourIcon);
                    }

                    if (EditorGUILayout.ToggleLeft(" Show Error Icon", showErrorIcon) != showErrorIcon)
                    {
                        QHierarchySettings.setSetting(QHierarchySetting.ShowErrorIcon, !showErrorIcon); 
                    }

        			if (EditorGUILayout.ToggleLeft(" Show Game Object icon", showGameObjectIcon) != showGameObjectIcon)
        			{
        				QHierarchySettings.setSetting(QHierarchySetting.ShowGameObjectIcon, !showGameObjectIcon);	
        			}

                    if (EditorGUILayout.ToggleLeft(" Show Tag And Layer", showTagAndLayerText) != showTagAndLayerText)
                    {
                        QHierarchySettings.setSetting(QHierarchySetting.ShowTagAndLayerText, !showTagAndLayerText); 
                    }

                    if (EditorGUILayout.ToggleLeft(" Show Custom Tag Icon", showCustomTagIcon) != showCustomTagIcon)
                    {
                        QHierarchySettings.setSetting(QHierarchySetting.ShowCustomTagIcon, !showCustomTagIcon); 
                    }

                    if (EditorGUILayout.ToggleLeft(" Show Static Button", showStaticIcon) != showStaticIcon)
                    {
                        QHierarchySettings.setSetting(QHierarchySetting.ShowStaticIcon, !showStaticIcon); 
                    }
                }
                                  
                if (drawTitle(QHierarchySetting.AppearanceSettingsFoldout, " Appearance Settings"))
                {
                    bool showTreeMap = QHierarchySettings.getSetting<bool>(QHierarchySetting.ShowTreeMap);
                    if (EditorGUILayout.ToggleLeft(" Show Hierarchy Tree", showTreeMap) != showTreeMap)
                    {
                        QHierarchySettings.setSetting(QHierarchySetting.ShowTreeMap, !showTreeMap);
                    }

                    bool showHiddenObjectList = QHierarchySettings.getSetting<bool>(QHierarchySetting.ShowHiddenQHierarchyObjectList);
                    if (EditorGUILayout.ToggleLeft(" Show QHierarchy GameObject", showHiddenObjectList) != showHiddenObjectList)
                    {
                        QHierarchySettings.setSetting(QHierarchySetting.ShowHiddenQHierarchyObjectList, !showHiddenObjectList);
                    }

                    bool showModifierWarning = QHierarchySettings.getSetting<bool>(QHierarchySetting.ShowModifierWarning);
                    if (EditorGUILayout.ToggleLeft(" Show Warning When Using Modifier + Click", showModifierWarning) != showModifierWarning)
                    {
                        QHierarchySettings.setSetting(QHierarchySetting.ShowModifierWarning, !showModifierWarning);
                    }

                    if (EditorGUILayout.ToggleLeft(" Fixed Icons Width", fixedIconWidth) != fixedIconWidth)
                    {
                        fixedIconWidth = !fixedIconWidth;
                        QHierarchySettings.setSetting(QHierarchySetting.FixedIconWidth, fixedIconWidth); 
                        if (fixedIconWidth) QHierarchySettings.setSetting(QHierarchySetting.TagAndLayerSizeType, (int)QHierarchyTagAndLayerSizeType.Fixed);  
                    }

                    int identation = QHierarchySettings.getSetting<int>(QHierarchySetting.Identation); 
                    int newIdentation = EditorGUILayout.IntSlider("    Indentation",  identation, 0, 200);
                    if (newIdentation != identation)
                    {
                        QHierarchySettings.setSetting(QHierarchySetting.Identation, newIdentation); 
                    }
                }

                if (drawTitle(QHierarchySetting.OrderSettingsFoldout, " Order Settings"))
                {
                    List<QHierarchyIconType> iconOrder = QHierarchySettings.getSetting<List<QHierarchyIconType>>(QHierarchySetting.IconOrder);
        			for (int i = 0; i < iconOrder.Count; i++)
        			{
                        QHierarchyIconType type = iconOrder[i];
        				EditorGUILayout.BeginHorizontal();
        				{
                            EditorGUILayout.LabelField((i + 1).ToString() + ".", GUILayout.Width(15));
        					Rect rect = EditorGUILayout.GetControlRect(GUILayout.Width(14), GUILayout.Height(14));

        					if (i > 0)
        					{
        						if (GUI.Button(rect, QHierarchyResource.getTexture(QHierarchyTexture.ButtonUp), GUIStyle.none))
        						{
                                    List<QHierarchyIconType> newIconOrder = new List<QHierarchyIconType>();
                                    for (int j = 0; j < iconOrder.Count; j++)
                                    {
                                        if (j == i - 1) newIconOrder.Add(iconOrder[i]);
                                        else if (j == i) newIconOrder.Add(iconOrder[i-1]);
                                        else newIconOrder.Add(iconOrder[j]);
                                    }
                                    QHierarchySettings.setSetting(QHierarchySetting.IconOrder, newIconOrder);
        						}
        					}

        					rect.x += 18;

                            if (i < iconOrder.Count - 1)
        					{
        						if (GUI.Button(rect, QHierarchyResource.getTexture(QHierarchyTexture.ButtonDown), GUIStyle.none))
        						{
                                    List<QHierarchyIconType> newIconOrder = new List<QHierarchyIconType>();
                                    for (int j = 0; j < iconOrder.Count; j++)
                                    {
                                        if (j == i) newIconOrder.Add(iconOrder[i+1]);
                                        else if (j == i + 1) newIconOrder.Add(iconOrder[i]);
                                        else newIconOrder.Add(iconOrder[j]);
                                    }
                                    QHierarchySettings.setSetting(QHierarchySetting.IconOrder, newIconOrder);
        						}
        					}
        	
        					rect.x += 16;
        					rect.width = 200; 
        					rect.height = 20;
                            GUI.Label(rect, addSpaces(type.ToString()));
        				} 
        				EditorGUILayout.EndHorizontal();
        			}
                }

                if (showLockButton)
                {
                    if (drawTitle(QHierarchySetting.LockSettingsFoldout, " Lock Settings"))
                    {
                        bool blockLockSelection = QHierarchySettings.getSetting<bool>(QHierarchySetting.PreventSelectionOfLockedObjects);
                        if (EditorGUILayout.ToggleLeft(" Prevent selection of locked objects", blockLockSelection) != blockLockSelection)
                        {
                            QHierarchySettings.setSetting(QHierarchySetting.PreventSelectionOfLockedObjects, !blockLockSelection);
                        }
                    }
                }

                if (showMonoBehaviourIcon)
                {
                    if (drawTitle(QHierarchySetting.MonoBehaviourIconFoldout, " MonoBehaviour Icon Settings"))
                    {
                        bool ignoreUnityMonobehaviour = QHierarchySettings.getSetting<bool>(QHierarchySetting.IgnoreUnityMonobehaviour);
                        if (EditorGUILayout.ToggleLeft(" Ignore UnityEngine MonoBehaviours", ignoreUnityMonobehaviour) != ignoreUnityMonobehaviour)
                        {
                            QHierarchySettings.setSetting(QHierarchySetting.IgnoreUnityMonobehaviour, !ignoreUnityMonobehaviour);
                        }
                    }
                }

                if (showErrorIcon)
                {
                    if (drawTitle(QHierarchySetting.ErrorIconSettingsFoldout, " Error Icon Settings"))
                    {
                        bool showErrorIconParent = QHierarchySettings.getSetting<bool>(QHierarchySetting.ShowErrorIconParent);
                        if (EditorGUILayout.ToggleLeft(" Show error icon on parent game object", showErrorIconParent) != showErrorIconParent)
                        {
                            QHierarchySettings.setSetting(QHierarchySetting.ShowErrorIconParent, !showErrorIconParent);                         
                        }

                        bool showErrorForDisabledComponents = QHierarchySettings.getSetting<bool>(QHierarchySetting.ShowErrorForDisabledComponents);
                        if (EditorGUILayout.ToggleLeft(" Show error for disabled components", showErrorForDisabledComponents) != showErrorForDisabledComponents)
                        {
                            QHierarchySettings.setSetting(QHierarchySetting.ShowErrorForDisabledComponents, !showErrorForDisabledComponents);                         
                        }

                        EditorGUILayout.Space();

                        EditorGUILayout.LabelField("Show the following error types:");
                        bool showErrorIconTypeScriptMissing = QHierarchySettings.getSetting<bool>(QHierarchySetting.ShowErrorIconScriptIsMissing);
                        if (EditorGUILayout.ToggleLeft(" Script is missing", showErrorIconTypeScriptMissing) != showErrorIconTypeScriptMissing)
                        {
                            QHierarchySettings.setSetting(QHierarchySetting.ShowErrorIconScriptIsMissing, !showErrorIconTypeScriptMissing); 
                        }
                        
                        bool showErrorIconTypeReferenceIsNull = QHierarchySettings.getSetting<bool>(QHierarchySetting.ShowErrorIconReferenceIsNull);
                        if (EditorGUILayout.ToggleLeft(" Reference is null", showErrorIconTypeReferenceIsNull) != showErrorIconTypeReferenceIsNull)
                        {
                            QHierarchySettings.setSetting(QHierarchySetting.ShowErrorIconReferenceIsNull, !showErrorIconTypeReferenceIsNull);   
                        }
                        
                        bool showErrorIconTypeStringIsEmpty = QHierarchySettings.getSetting<bool>(QHierarchySetting.ShowErrorIconStringIsEmpty);
                        if (EditorGUILayout.ToggleLeft(" String is empty", showErrorIconTypeStringIsEmpty) != showErrorIconTypeStringIsEmpty)
                        {
                            QHierarchySettings.setSetting(QHierarchySetting.ShowErrorIconStringIsEmpty, !showErrorIconTypeStringIsEmpty);   
                        }
                    }
                }
                
                if (showTagAndLayerText)
                {
                    if (drawTitle(QHierarchySetting.TagAndLayerSettingsFoldout, " Tag And Layer Settings"))
                    {                    
                        QHierarchyTagAndLayerType tagAndLayerType = (QHierarchyTagAndLayerType)QHierarchySettings.getSetting<int>(QHierarchySetting.TagAndLayerType);
                        QHierarchyTagAndLayerType newTagAndLayerType;
                        if ((newTagAndLayerType = (QHierarchyTagAndLayerType)EditorGUILayout.EnumPopup("Show", tagAndLayerType)) != tagAndLayerType)
                        {
                            QHierarchySettings.setSetting(QHierarchySetting.TagAndLayerType, (int)newTagAndLayerType);  
                        }
                        
                        if (fixedIconWidth) GUI.enabled = false;
                        
                        QHierarchyTagAndLayerSizeType tagAndLayerSizeType = (QHierarchyTagAndLayerSizeType)QHierarchySettings.getSetting<int>(QHierarchySetting.TagAndLayerSizeType);
                        QHierarchyTagAndLayerSizeType newTagAndLayerSizeType;
                        if ((newTagAndLayerSizeType = (QHierarchyTagAndLayerSizeType)EditorGUILayout.EnumPopup("Layout", tagAndLayerSizeType)) != tagAndLayerSizeType)
                        {
                            QHierarchySettings.setSetting(QHierarchySetting.TagAndLayerSizeType, (int)newTagAndLayerSizeType);  
                        }
                        
                        if (fixedIconWidth) GUI.enabled = true;

                        if (newTagAndLayerSizeType != QHierarchyTagAndLayerSizeType.Float)
                        {
                            QHierarchyTagAndLayerSizeValueType tagAndLayerSizeValueType = (QHierarchyTagAndLayerSizeValueType)QHierarchySettings.getSetting<int>(QHierarchySetting.TagAndLayerSizeValueType);
                            QHierarchyTagAndLayerSizeValueType newTagAndLayerSizeValueType;
                            if ((newTagAndLayerSizeValueType = (QHierarchyTagAndLayerSizeValueType)EditorGUILayout.EnumPopup("Type", tagAndLayerSizeValueType)) != tagAndLayerSizeValueType)
                            {
                                QHierarchySettings.setSetting(QHierarchySetting.TagAndLayerSizeValueType, (int)newTagAndLayerSizeValueType);  
                            }

                            if (newTagAndLayerSizeValueType == QHierarchyTagAndLayerSizeValueType.Pixel)
                            {
                                int tagAndLayerSizeValue = QHierarchySettings.getSetting<int>(QHierarchySetting.TagAndLayerSizeValue);
                                int newLayerSizeValue = EditorGUILayout.IntSlider("Pixel Width", tagAndLayerSizeValue, 1, 250);
                                if (newLayerSizeValue != tagAndLayerSizeValue)
                                {
                                    QHierarchySettings.setSetting(QHierarchySetting.TagAndLayerSizeValue, newLayerSizeValue);   
                                }
                            }
                            else
                            {
                                float tagAndLayerSizeValuePercent = QHierarchySettings.getSetting<float>(QHierarchySetting.TagAndLayerSizeValuePercent);
                                float newtagAndLayerSizeValuePercent = EditorGUILayout.Slider("Percent Width", tagAndLayerSizeValuePercent, 0, 0.5f);
                                if (tagAndLayerSizeValuePercent != newtagAndLayerSizeValuePercent)
                                {
                                    QHierarchySettings.setSetting(QHierarchySetting.TagAndLayerSizeValuePercent, newtagAndLayerSizeValuePercent);   
                                }
                            }

                            QHierarchyTagAndLayerAligment tagAndLayerAligment = (QHierarchyTagAndLayerAligment)QHierarchySettings.getSetting<int>(QHierarchySetting.TagAndLayerAligment);
                            QHierarchyTagAndLayerAligment newTagAndLayerAligment;
                            if ((newTagAndLayerAligment = (QHierarchyTagAndLayerAligment)EditorGUILayout.EnumPopup("Alignment", tagAndLayerAligment)) != tagAndLayerAligment)
                            {
                                QHierarchySettings.setSetting(QHierarchySetting.TagAndLayerAligment, (int)newTagAndLayerAligment);  
                            }
                        }
                    }
                }

                if (showCustomTagIcon)
                {
                    if (drawTitle(QHierarchySetting.CustomTagIconFoldout, " Custom Tag Icon"))
                    {
                        bool replaceGameObjectIcon = QHierarchySettings.getSetting<bool>(QHierarchySetting.CustomTagIconReplace);
                        if (EditorGUILayout.ToggleLeft(" Replace Game Object Icon", replaceGameObjectIcon) != replaceGameObjectIcon)
                        {
                            QHierarchySettings.setSetting(QHierarchySetting.CustomTagIconReplace, !replaceGameObjectIcon); 
                        }

                        bool customTagIconTextureListFoldout = QHierarchySettings.getSetting<bool>(QHierarchySetting.CustomTagIconTextureListFoldout);
                        bool customTagIconTextureListFoldoutNew = EditorGUILayout.Foldout(customTagIconTextureListFoldout, " Custom Tag Icon Texture List");

                        if (customTagIconTextureListFoldoutNew != customTagIconTextureListFoldout)
                            QHierarchySettings.setSetting(QHierarchySetting.CustomTagIconTextureListFoldout, customTagIconTextureListFoldoutNew);

                        if (customTagIconTextureListFoldoutNew)
                        {
                            EditorGUI.indentLevel++;
                            drawTagTextureList();
                            EditorGUI.indentLevel--;
                        }
                    }
                }
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndScrollView();
		}

        private bool drawTitle(QHierarchySetting foldoutSetting, string title)
        {
            bool foldout = QHierarchySettings.getSetting<bool>(foldoutSetting);

            GUILayout.Space(5);
            Rect rect = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true), GUILayout.Height(24));
            rect.width += rect.x * 2 + 1;
            rect.x = 0;
            GUI.Box(rect, "");             
            GUILayout.Space(5);

            rect.x = 5;
            rect.y += 4;
            bool newFoldout = EditorGUI.Foldout(rect, foldout, title);
            if (newFoldout != foldout)
            {
                QHierarchySettings.setSetting(foldoutSetting, newFoldout);
            }
            return newFoldout;
        }

        private void drawTagTextureList()
        {
            bool changed = false;

            List<QTagTexture> tagTextureList = QHierarchySettings.getSetting<List<QTagTexture>>(QHierarchySetting.CustomTagIcon);
            for (int i = 0; i < UnityEditorInternal.InternalEditorUtility.tags.Length; i++) 
            {
                string tag = UnityEditorInternal.InternalEditorUtility.tags[i];
                QTagTexture tagTexture = tagTextureList.Find(t => t.tag == tag);
                Texture2D newTexture = (Texture2D)EditorGUILayout.ObjectField(tag, tagTexture == null ? null : tagTexture.texture, typeof(Texture2D), false, GUILayout.MaxHeight(16));
                if (newTexture != null && tagTexture == null)
                {
                    QTagTexture newTagTexture = new QTagTexture(tag, newTexture);
                    tagTextureList.Add(newTagTexture);

                    changed = true;
                }
                else if (newTexture == null && tagTexture != null)
                {
                    tagTextureList.Remove(tagTexture);

                    changed = true;
                }
                else if (tagTexture != null && tagTexture.texture != newTexture)
                {
                    tagTexture.texture = newTexture;
                    changed = true;
                }
            }

            if (changed) 
            {
                QHierarchySettings.setSetting(QHierarchySetting.CustomTagIcon, tagTextureList);
                EditorApplication.RepaintHierarchyWindow();
            }
        }

        private static string addSpaces(string text)
        {
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]) && text[i - 1] != ' ')                
                    newText.Append(' ');                
                newText.Append(text[i]);                
            }
            return newText.ToString();
        }
	}
}