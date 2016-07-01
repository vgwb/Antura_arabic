using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System;

namespace qtools.qhierarchy
{
    [InitializeOnLoad]
    public class QHierarchy
    {
        private const string ObjectListName = "QHierarchyObjectList";

        private static GUIStyle smallLabelStyle;
        
        private static string theme;
        private static Color backgroundColor;
        private static Color greyColor;
        private static Color greyLightColor;

        private static bool inited = false;
        private static Dictionary<int, bool> wasError = new Dictionary<int, bool>();        
        private static QObjectList objectList;

        private static int targetVisibilityState = -1;
        private static int targetLockState       = -1;
        private static int targetStaticState     = -1;
        private static int targetMeshState       = -1;

        private static GameObject lastSelectionGameObject = null;
        private static int lastSelectionCount = 0;

        static QHierarchy()
        {
            EditorApplication.update += Update;
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(UndoRedoPerformed));
        } 
        
        static void hierarchyWindowChanged()
        {
            if (objectList != null) objectList.checkIntegrity();
        }

        static void UndoRedoPerformed()
        {
            EditorApplication.RepaintHierarchyWindow();          
        }

        static void hierarchyWindowItemOnGUIHandler(int instanceId, Rect selectionRect)
        {
            try
            {
                DrawHierarchyItemIcon(instanceId, selectionRect);
                wasError[instanceId] = false; 
            }
            catch (Exception exception)
            {
                if (!wasError.ContainsKey(instanceId) || !wasError[instanceId])
                {
                    Debug.LogError(exception.ToString());
                    wasError[instanceId] = true;
                }
            }
        }        

        public static void repaint()
        {
            EditorApplication.RepaintHierarchyWindow();
        }

        static void Update()
        {
            try
            {
                if (!inited) init();
                 
                if (objectList == null) 
                {
                    objectList = getObjectList(false);
                    if (objectList == null) return;
                }
                    
                HideFlags hideFlag = QHierarchySettings.getSetting<bool>(QHierarchySetting.ShowHiddenQHierarchyObjectList) ? HideFlags.None : HideFlags.HideInHierarchy;
                if (objectList.gameObject.hideFlags != hideFlag) objectList.gameObject.hideFlags = hideFlag;   

                if (QHierarchySettings.getSetting<bool>(QHierarchySetting.ShowLockButton) &&
                    QHierarchySettings.getSetting<bool>(QHierarchySetting.PreventSelectionOfLockedObjects) && 
                    checkSelectionChanged())
                {
                    GameObject[] selections = Selection.gameObjects;
                    List<GameObject> actual = new List<GameObject>();
                    bool found = false;
                    for (int i = 0; i < selections.Length; i++)
                    {
                        bool isLock = objectList.lockedObjects.Contains(selections[i]);
                        if (!isLock) actual.Add(selections[i]);
                        else found = true;
                    }
                    if (found) Selection.objects = actual.ToArray();
                }
            }
            catch
            {

            }
        }

        static bool checkSelectionChanged()
        {
            if (lastSelectionGameObject != Selection.activeGameObject ||
                lastSelectionCount != Selection.gameObjects.Length)
            {
                lastSelectionGameObject = Selection.activeGameObject;
                lastSelectionCount = Selection.gameObjects.Length;
                return true;
            }
            return false;
        }

        static void init()
        {       
            inited = true;
            backgroundColor = EditorGUIUtility.isProSkin ? new Color(0.22f, 0.22f, 0.22f) : new Color(0.761f, 0.761f, 0.761f);
            
            greyColor = EditorGUIUtility.isProSkin ? new Color(0.6f, 0.6f, 0.6f) : new Color(0.3f, 0.3f, 0.3f);
            greyLightColor = EditorGUIUtility.isProSkin ? new Color(0.8f, 0.8f, 0.8f) : new Color(0.1f, 0.1f, 0.1f);

            smallLabelStyle = new GUIStyle();
            smallLabelStyle.normal.textColor = greyColor;
            smallLabelStyle.fontSize = 8;
            smallLabelStyle.clipping = TextClipping.Clip;  
            
            QHierarchySettingsWindow[] settingsWindow = Resources.FindObjectsOfTypeAll<QHierarchySettingsWindow>();
            if (settingsWindow != null && settingsWindow.Length > 0) settingsWindow[0].Repaint();  

            EditorApplication.hierarchyWindowItemOnGUI += hierarchyWindowItemOnGUIHandler;
            EditorApplication.hierarchyWindowChanged += hierarchyWindowChanged;
        } 
        
        public static QObjectList getObjectList(bool createIfNotExist = true)
        { 
            if (objectList ==  null)
            {
                QObjectList[] objectListArray = (QObjectList[])GameObject.FindObjectsOfType<QObjectList>();
                for (int i = 0; i < objectListArray.Length; i++)
                {
                    if (objectListArray[i].name == ObjectListName)
                    {
                        if (objectList == null)
                        {
                            objectList = objectListArray[i];
                        }
                        else
                        {
                            objectList.merge(objectListArray[i]);
                            GameObject.DestroyImmediate(objectListArray[i].gameObject);
                        }
                    }
                }               
                
                if (objectList == null && createIfNotExist)
                {
                    GameObject gameObjectList = new GameObject();
                    gameObjectList.name = ObjectListName;
                    objectList = gameObjectList.AddComponent<QObjectList>();


                }

                objectList.tag = null;
                MonoScript monoScript = MonoScript.FromMonoBehaviour(objectList);
                if (MonoImporter.GetExecutionOrder(monoScript) != -10000)                    
                    MonoImporter.SetExecutionOrder(monoScript, -10000);

                if (objectList != null)
                {
                    List<GameObject> wireframeHiddenObjects = objectList.wireframeHiddenObjects;
                    foreach (GameObject gameObject in wireframeHiddenObjects)
                    {
                        Renderer renderer = gameObject.GetComponent<Renderer>();
                        if (renderer != null)
                        {
                            EditorUtility.SetSelectedWireframeHidden(renderer, true);
                        }
                    }
                }
            }
            return objectList;
        }

        static void DrawHierarchyItemIcon(int instanceId, Rect selectionRect)
        {   
            try
            {
                GameObject itemGameObject = (GameObject)EditorUtility.InstanceIDToObject(instanceId);
                if (itemGameObject == null) return;
                
                if (QHierarchySettings.getSetting<bool>(QHierarchySetting.ShowTreeMap) || 
                    QHierarchySettings.getSetting<bool>(QHierarchySetting.ShowMonoBehaviourIcon))
                    drawTreeMap(itemGameObject, selectionRect);
                
                Rect rect = new Rect(selectionRect);
                rect.width = 16;
                rect.x += selectionRect.width - QHierarchySettings.getSetting<int>(QHierarchySetting.Identation);
                
                List<QHierarchyIconType> iconOrder = QHierarchySettings.getSetting<List<QHierarchyIconType>>(QHierarchySetting.IconOrder);
                for (int i = iconOrder.Count - 1; i >= 0; i--)
                {
                    switch (iconOrder[i])
                    {               
                        case QHierarchyIconType.LockButton:
                        {
                            if (QHierarchySettings.getSetting<bool>(QHierarchySetting.ShowLockButton))
                            {
                                rect.x -= drawLockButton(itemGameObject, rect);
                            }
                            break;
                        }
                        case QHierarchyIconType.VisibilityButton:
                        {
                            if (QHierarchySettings.getSetting<bool>(QHierarchySetting.ShowVisibilityButton))
                            {
                                rect.x -= drawVisibilityButton(itemGameObject, rect);               
                            }
                            break;
                        }
                        case QHierarchyIconType.MeshButton:
                        {
                            if (QHierarchySettings.getSetting<bool>(QHierarchySetting.ShowMeshButton))
                            {
                                rect.x -= drawWireframeButton(itemGameObject, rect);               
                            }
                            break;
                        }
                        case QHierarchyIconType.TagAndLayer:
                        {
                            if (QHierarchySettings.getSetting<bool>(QHierarchySetting.ShowTagAndLayerText)) 
                            {
                                rect.x -= drawTagLayerText(itemGameObject, rect);
                            }
                            break;
                        }
                        case QHierarchyIconType.ErrorIcon:
                        {
                            if (QHierarchySettings.getSetting<bool>(QHierarchySetting.ShowErrorIcon)) 
                            {  
                                rect.x -= drawErrorIcon(itemGameObject, rect);
                            }
                            break;
                        }
                        case QHierarchyIconType.GameObjectIcon:
                        {
                            if (QHierarchySettings.getSetting<bool>(QHierarchySetting.ShowGameObjectIcon))
                            {
                                float offset = drawGameObjectIcon(itemGameObject, rect);

                                if (QHierarchySettings.getSetting<bool>(QHierarchySetting.CustomTagIconReplace))
                                    drawCustomTagIcon(itemGameObject, rect);
                                
                                rect.x -= offset;
                            }
                            break;
                        }
                        case QHierarchyIconType.CustomTagIcon:
                        {
                            if (QHierarchySettings.getSetting<bool>(QHierarchySetting.ShowCustomTagIcon) && 
                                !QHierarchySettings.getSetting<bool>(QHierarchySetting.CustomTagIconReplace))
                            {
                                rect.x -= drawCustomTagIcon(itemGameObject, rect);
                            }
                            break;
                        }
                        case QHierarchyIconType.StaticIcon:
                        {
                            if (QHierarchySettings.getSetting<bool>(QHierarchySetting.ShowStaticIcon))
                            {
                                rect.x -= drawStaticIcon(itemGameObject, rect);
                            }
                            break;
                        }
                    }
                }
            }
            catch
            {
            }
        }

        static void getGameObjectListRecursive(ref List<GameObject>result, GameObject gameObject, int maxDepth = int.MaxValue)
        {
            result.Add(gameObject);
            if (maxDepth > 0)
            {
                Transform transform = gameObject.transform;
                for (int i = transform.childCount - 1; i >= 0; i--)      
                    getGameObjectListRecursive(ref result, transform.GetChild(i).gameObject, maxDepth - 1);     
            }
        }

        #region TreeMap
        static void drawTreeMap(GameObject gameObject, Rect selectionRect)
        {
            Transform gameObjectTransform = gameObject.transform;
            
            bool hasChild = false;
            hasChild = gameObjectTransform.childCount > 0;
            
            bool foundCustomComponent = false;
           
            if (QHierarchySettings.getSetting<bool>(QHierarchySetting.IgnoreUnityMonobehaviour))
            {
                Component[] components = gameObject.GetComponents<MonoBehaviour>();

                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i] != null)
                    {
                        string fullName = components[i].GetType().FullName;
                        if (fullName.Contains("UnityEngine")) continue;
                        foundCustomComponent = true;
                        break;
                    }
                }

            }
            else
            {
                foundCustomComponent = gameObject.GetComponent<MonoBehaviour>() != null;
            }
            
            Rect treeMapRect = new Rect(selectionRect);
            treeMapRect.width = treeMapRect.x + 28;
            treeMapRect.x = 0;
            
            int ident = Mathf.FloorToInt(treeMapRect.width / 14) - 1;
            treeMapRect.width = (ident) * 14; 
            float stepX = 1.0f / (896.0f / 14.0f);
            float rectX = (hasChild ? 1.0f : 0.5f) - ident * stepX;
            float scaleX = ident * stepX;
            
            bool showMonoBehaviourIcon = QHierarchySettings.getSetting<bool>(QHierarchySetting.ShowMonoBehaviourIcon);
            
            float stepY = 1.0f / (64.0f / 16.0f);
            float rectY = 1.0f - stepY * (QHierarchySettings.getSetting<bool>(QHierarchySetting.ShowTreeMap) ? 
                                          (showMonoBehaviourIcon ? (foundCustomComponent == false ? 1 : 2) : 1) : 
                                          (showMonoBehaviourIcon ? (foundCustomComponent == false ? 3 : 4) : 3));
            float scaleY = 0.25f;
            
            GUI.DrawTextureWithTexCoords(treeMapRect, QHierarchyResource.getTexture(QHierarchyTexture.BackgroundTreeMap), new Rect(rectX,rectY,scaleX,scaleY));
        }
        #endregion

        #region GameObject icon
        static MethodInfo GetIconForObject = typeof(EditorGUIUtility).GetMethod( "GetIconForObject", BindingFlags.NonPublic | BindingFlags.Static );

        static float drawGameObjectIcon(GameObject gameObject, Rect rect)
        {
            bool fixedIconWidth = QHierarchySettings.getSetting<bool>(QHierarchySetting.FixedIconWidth);            
            Texture2D icon = (Texture2D)GetIconForObject.Invoke( null, new object[] { gameObject } );
            rect.x -= 18;
            rect.width = 18;
            if (icon != null)
            {               
                EditorGUI.DrawRect(rect, backgroundColor);
                rect.width = 16;
                GUI.DrawTexture(rect, icon, ScaleMode.ScaleToFit, true);
                return 18;
            }
            else if (fixedIconWidth)
            {
                EditorGUI.DrawRect(rect, backgroundColor);
                return 18;
            }
            return 0;
        }
        #endregion

        #region Custom tag icon
        static float drawCustomTagIcon(GameObject gameObject, Rect rect)
        {
            bool fixedIconWidth = QHierarchySettings.getSetting<bool>(QHierarchySetting.FixedIconWidth);            
            
            rect.x -= 18;
            rect.width = 18;
            
            string gameObjectTag = gameObject.tag;
            QTagTexture tagTexture = QHierarchySettings.getSetting<List<QTagTexture>>(QHierarchySetting.CustomTagIcon).Find(t => t.tag == gameObjectTag);
            
            if (tagTexture != null)
            {
                EditorGUI.DrawRect(rect, backgroundColor);
                rect.width = 16;
                GUI.DrawTexture(rect, tagTexture.texture, ScaleMode.ScaleToFit, true);
                return 18;
            }
            else if (fixedIconWidth)
            {
                if (!QHierarchySettings.getSetting<bool>(QHierarchySetting.CustomTagIconReplace))
                    EditorGUI.DrawRect(rect, backgroundColor);
                return 18;
            }
            return 0;
        }
        #endregion

        #region Error icon
        static float drawErrorIcon(GameObject gameObject, Rect rect)
        {
            bool fixedIconWidth = QHierarchySettings.getSetting<bool>(QHierarchySetting.FixedIconWidth);
            bool errorFound = findError(gameObject.GetComponents<MonoBehaviour>());
            if (errorFound)
            {           
                rect.x -= 16;
                rect.width = 16;
                EditorGUI.DrawRect(rect, backgroundColor);
                GUI.DrawTexture(rect, QHierarchyResource.getTexture(QHierarchyTexture.IconError));
            }
            else if (fixedIconWidth)
            {
                rect.x -= 16;
                rect.width = 16;
                EditorGUI.DrawRect(rect, backgroundColor);
                rect.x += 16;
            }
            
            if (QHierarchySettings.getSetting<bool>(QHierarchySetting.ShowErrorIconParent) && !errorFound)
            {
                bool childErrorFound = findError(gameObject.GetComponentsInChildren<MonoBehaviour>(true));
                if (childErrorFound)
                {
                    rect.x -= 16;
                    rect.width = 16;
                    GUI.DrawTexture(rect, QHierarchyResource.getTexture(QHierarchyTexture.IconErrorChild));
                }
                return childErrorFound ? 16 : (fixedIconWidth ? 16 : 0);
            }
            else
            {
                return errorFound ? 16 : (fixedIconWidth ? 16 : 0);
            } 
        }
        
        static bool findError(MonoBehaviour[] components)
        {
            bool showErrorTypeReferenceIsNull = QHierarchySettings.getSetting<bool>(QHierarchySetting.ShowErrorIconReferenceIsNull);
            bool showErrorTypeStringIsEmpty = QHierarchySettings.getSetting<bool>(QHierarchySetting.ShowErrorIconStringIsEmpty);
            bool showErrorIconScriptIsMissing = QHierarchySettings.getSetting<bool>(QHierarchySetting.ShowErrorIconScriptIsMissing);
            bool showErrorForDisabledComponents = QHierarchySettings.getSetting<bool>(QHierarchySetting.ShowErrorForDisabledComponents);

            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null)
                {
                    if (showErrorIconScriptIsMissing)
                    {
                        return true;
                    }
                }
                else
                {
                    if (showErrorTypeReferenceIsNull || showErrorTypeStringIsEmpty)
                    {                       
                        MonoBehaviour monoBehaviour = components[i];
                        if (!(monoBehaviour.enabled && monoBehaviour.gameObject.activeSelf) && !showErrorForDisabledComponents) continue;

                        FieldInfo[] fieldArray = monoBehaviour.GetType().GetFields();
                        for (int j = 0; j < fieldArray.Length; j++)
                        {
                            FieldInfo field = fieldArray[j];

                            try
                            {
                                if (System.Attribute.IsDefined(field, typeof(HideInInspector)) || field.IsStatic) continue;

                                object value = field.GetValue(monoBehaviour);

                                if (showErrorTypeReferenceIsNull && value != null && value.Equals(null))
                                {                        
                                    return true;
                                }
                                else if (field.FieldType == typeof(string))
                                {                                
                                    if (showErrorTypeStringIsEmpty && value != null && ((string)value).Equals(""))
                                    {
                                        return true;                                 
                                    }
                                }
                                else 
                                {
                                    if (showErrorTypeReferenceIsNull && (value is IEnumerable))
                                    {
                                        foreach (var item in (IEnumerable)value)
                                        {
                                            if (item == null) return true;
                                        }
                                    }
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
            return false;
        }
        #endregion

        #region Visibility button
        static float drawVisibilityButton(GameObject gameObject, Rect rect)
        {
            rect.x -= 18;
            rect.width = 18;
            
            int visibility = gameObject.activeSelf ? 1 : 0;
            
            bool editModeVisibleObjectsContains = objectList == null ? false : objectList.editModeVisibileObjects.Contains(gameObject);
            bool editModeInvisibleObjectsContains = objectList == null ? false : objectList.editModeInvisibleObjects.Contains(gameObject);
            
            if (!EditorApplication.isPlayingOrWillChangePlaymode && 
                ((!gameObject.activeSelf && editModeVisibleObjectsContains) || 
                (gameObject.activeSelf && editModeInvisibleObjectsContains)))
            {
                objectList.editModeVisibileObjects.Remove(gameObject);
                objectList.editModeInvisibleObjects.Remove(gameObject);
                editModeVisibleObjectsContains = false;
                editModeInvisibleObjectsContains = false;
            }
            
            if (visibility == 1)
            {
                Transform transform = gameObject.transform;
                while (transform.parent != null)
                {
                    transform = transform.parent;
                    if (!transform.gameObject.activeSelf) 
                    {
                        visibility = 2;
                        break;
                    }
                }
            }
            
            Texture2D visibilityIcon;
            if (!EditorApplication.isPlayingOrWillChangePlaymode && (editModeVisibleObjectsContains || editModeInvisibleObjectsContains))
                visibilityIcon = visibility == 0 ? QHierarchyResource.getTexture(QHierarchyTexture.ButtonVisibilityOffEdit) : visibility == 1 ? QHierarchyResource.getTexture(QHierarchyTexture.ButtonVisibilityOnEdit) : QHierarchyResource.getTexture(QHierarchyTexture.ButtonVisibilityOffParentEdit);
            else
                visibilityIcon = visibility == 0 ? QHierarchyResource.getTexture(QHierarchyTexture.ButtonVisibilityOff) : visibility == 1 ? QHierarchyResource.getTexture(QHierarchyTexture.ButtonVisibilityOn) : QHierarchyResource.getTexture(QHierarchyTexture.ButtonVisibilityOffParent);

            GUI.DrawTexture(rect, visibilityIcon);

            if (Event.current.isMouse && Event.current.button == 0 && rect.Contains(Event.current.mousePosition))
            {
                if (Event.current.type == EventType.MouseDown)
                {
                    targetVisibilityState = ((!gameObject.activeSelf) == true ? 1 : 0);
                }
                else if (Event.current.type == EventType.MouseDrag && targetVisibilityState != -1)
                {
                    if (targetVisibilityState == (gameObject.activeSelf == true ? 1 : 0)) return rect.width;
                } 
                else
                {
                    targetVisibilityState = -1;
                    return rect.width;
                }

                Event.current.Use();               
                bool showWarning = QHierarchySettings.getSetting<bool>(QHierarchySetting.ShowModifierWarning);

                List<GameObject> targetGameObjects = new List<GameObject>();
                if (Event.current.control || Event.current.command) 
                {
                    if (Event.current.shift)
                    {
                        if (!showWarning || EditorUtility.DisplayDialog("Change edit-time visibility", "Are you sure you want to turn " + (gameObject.activeSelf ? "off" : "on") + " the edit-time visibility of this GameObject and all its children? (You can disable this warning in the settings)", "Yes", "Cancel"))
                        {
                            getGameObjectListRecursive(ref targetGameObjects, gameObject);
                        }
                    }
                    else if (Event.current.alt)
                    {
                        if (gameObject.transform.parent != null)
                        {
                            if (!showWarning || EditorUtility.DisplayDialog("Change edit-time visibility", "Are you sure you want to turn " + (gameObject.activeSelf ? "off" : "on") + " the edit-time visibility this GameObject and its siblings? (You can disable this warning in the settings)", "Yes", "Cancel"))
                            {
                                getGameObjectListRecursive(ref targetGameObjects, gameObject.transform.parent.gameObject, 1);
                                targetGameObjects.Remove(gameObject.transform.parent.gameObject);
                            }
                        }
                        else
                        {
                            Debug.Log("This action for root objects is supported only for Unity3d 5.3.3 and above");
                        }
                    }
                    else
                    {
                        getGameObjectListRecursive(ref targetGameObjects, gameObject, 0);
                    }
                }
                else if (Event.current.shift)
                {
                    if (!showWarning || EditorUtility.DisplayDialog("Change visibility", "Are you sure you want to turn " + (gameObject.activeSelf ? "off" : "on") + " the visibility of this GameObject and all its children? (You can disable this warning in the settings)", "Yes", "Cancel"))
                    {
                        getGameObjectListRecursive(ref targetGameObjects, gameObject);           
                    }
                }
                else if (Event.current.alt) 
                {
                    if (gameObject.transform.parent != null)
                    {
                        if (!showWarning || EditorUtility.DisplayDialog("Change visibility", "Are you sure you want to turn " + (gameObject.activeSelf ? "off" : "on") + " the visibility this GameObject and its siblings? (You can disable this warning in the settings)", "Yes", "Cancel"))
                        {
                            getGameObjectListRecursive(ref targetGameObjects, gameObject.transform.parent.gameObject, 1);
                            targetGameObjects.Remove(gameObject.transform.parent.gameObject);
                        }
                    }
                    else
                    {
                        Debug.Log("This action for root objects is supported only for Unity3d 5.3.3 and above");
                    }
                }
                else 
                {
                    if (Selection.Contains(gameObject))
                    {
                        targetGameObjects.AddRange(Selection.gameObjects);
                    }
                    else
                    {
                        getGameObjectListRecursive(ref targetGameObjects, gameObject, 0);
                    };
                }

                setVisibility(targetGameObjects, !gameObject.activeSelf, Event.current.control || Event.current.command);
            } 
            
            return rect.width;
        }

        static void setVisibility(List<GameObject> gameObjects, bool targetVisibility, bool editMode)
        {
            if (objectList != null || editMode)
                Undo.RecordObject(getObjectList(true), "visibility change");

            for (int i = gameObjects.Count - 1; i >= 0; i--)
            {        
                GameObject curGameObject = gameObjects[i];
                Undo.RecordObject(curGameObject, "visibility change");

                if (editMode)
                {
                    if (!targetVisibility)
                    {
                        getObjectList(true).editModeVisibileObjects.Remove(curGameObject);        
                        if (!objectList.editModeInvisibleObjects.Contains(curGameObject))
                            getObjectList().editModeInvisibleObjects.Add(curGameObject);
                    }
                    else
                    {
                        getObjectList(true).editModeInvisibleObjects.Remove(curGameObject);                            
                        if (!objectList.editModeVisibileObjects.Contains(curGameObject))
                            getObjectList().editModeVisibileObjects.Add(curGameObject);
                    }
                }
                else if (objectList != null)
                {
                    objectList.editModeVisibileObjects.Remove(curGameObject);
                    objectList.editModeInvisibleObjects.Remove(curGameObject);
                }

                curGameObject.SetActive(targetVisibility);
                EditorUtility.SetDirty(curGameObject);
            }
        }
        #endregion

        #region Lock button
        static float drawLockButton(GameObject gameObject, Rect rect)
        {  
            rect.x -= 13;
            rect.width = 13;
            
            bool isLock = objectList == null ? false : objectList.lockedObjects.Contains(gameObject);
            if (isLock == true && (gameObject.hideFlags & HideFlags.NotEditable) != HideFlags.NotEditable)
            {
                gameObject.hideFlags |= HideFlags.NotEditable;
                EditorUtility.SetDirty(gameObject);
            }
            else if (isLock == false && (gameObject.hideFlags & HideFlags.NotEditable) == HideFlags.NotEditable)
            {
                gameObject.hideFlags ^= HideFlags.NotEditable;
                EditorUtility.SetDirty(gameObject);
            }
            
            GUI.DrawTexture(rect, isLock ? QHierarchyResource.getTexture(QHierarchyTexture.ButtonLockOn) : QHierarchyResource.getTexture(QHierarchyTexture.ButtonLockOff));
            
            if (Event.current.isMouse && Event.current.button == 0 && rect.Contains(Event.current.mousePosition))
            {
                if (Event.current.type == EventType.MouseDown)
                {
                    targetLockState = ((!isLock) == true ? 1 : 0);
                }
                else if (Event.current.type == EventType.MouseDrag && targetLockState != -1)
                {
                    if (targetLockState == (isLock == true ? 1 : 0)) return rect.width;
                } 
                else
                {
                    targetLockState = -1;
                    return rect.width;
                }
                
                Event.current.Use();
                bool showWarning = QHierarchySettings.getSetting<bool>(QHierarchySetting.ShowModifierWarning);

                List<GameObject> targetGameObjects = new List<GameObject>();
                if (Event.current.shift) 
                {
                    if (!showWarning || EditorUtility.DisplayDialog("Change locking", "Are you sure you want to " + (isLock ? "unlock" : "lock") + " this GameObject and all its children? (You can disable this warning in the settings)", "Yes", "Cancel"))
                    {
                        getGameObjectListRecursive(ref targetGameObjects, gameObject);           
                    }
                }
                else if (Event.current.alt)
                {
                    if (gameObject.transform.parent != null)
                    {
                        if (!showWarning || EditorUtility.DisplayDialog("Change locking", "Are you sure you want to " + (isLock ? "unlock" : "lock") + " this GameObject and its siblings? (You can disable this warning in the settings)", "Yes", "Cancel"))
                        {
                            getGameObjectListRecursive(ref targetGameObjects, gameObject.transform.parent.gameObject, 1);
                            targetGameObjects.Remove(gameObject.transform.parent.gameObject);
                        }
                    }
                    else
                    {
                        Debug.Log("This action for root objects is supported only for Unity3d 5.3.3 and above");
                    }
                }
                else 
                {
                    if (Selection.Contains(gameObject))
                    {
                        targetGameObjects.AddRange(Selection.gameObjects);
                    }
                    else
                    {
                        getGameObjectListRecursive(ref targetGameObjects, gameObject, 0);
                    };
                }

                setLock(targetGameObjects, !isLock);
            }
            return rect.width; 
        }   

        static void setLock(List<GameObject> gameObjects, bool targetLock)
        {
            Undo.RecordObject(getObjectList(true), targetLock ? "lock" : "unlock");

            for (int i = gameObjects.Count - 1; i >= 0; i--)
            {     
                GameObject curGameObject = gameObjects[i];
                Undo.RecordObject(curGameObject, targetLock ? "lock" : "unlock");

                if (targetLock)
                {
                    curGameObject.hideFlags |= HideFlags.NotEditable;
                    if (!objectList.lockedObjects.Contains(curGameObject))
                        objectList.lockedObjects.Add(curGameObject);
                }
                else
                {
                    curGameObject.hideFlags &= ~HideFlags.NotEditable;
                    objectList.lockedObjects.Remove(curGameObject);
                }

                EditorUtility.SetDirty(curGameObject);
            }
        }

        public static void unlockAll()
        {
            if (objectList != null)
            {
                for (int i = objectList.lockedObjects.Count - 1; i >= 0; i--)
                { 
                    GameObject curGameObject = objectList.lockedObjects[i];
                    curGameObject.hideFlags &= ~HideFlags.NotEditable;
                    EditorUtility.SetDirty(curGameObject);
                }
            }
        }
        #endregion

        #region Static button
        static float drawStaticIcon(GameObject gameObject, Rect rect)
        {  
            rect.x -= 14;
            rect.width = 14;
            
            bool isStatic = gameObject.isStatic;
            StaticEditorFlags staticFlags = GameObjectUtility.GetStaticEditorFlags(gameObject);
            
            Texture2D buttonTexture = isStatic ? 
                ((int)staticFlags == -1 ?  
                 QHierarchyResource.getTexture(QHierarchyTexture.ButtonStaticOn) 
                 : QHierarchyResource.getTexture(QHierarchyTexture.ButtonStaticHalf))
                    : QHierarchyResource.getTexture(QHierarchyTexture.ButtonStaticOff); 
            
            GUI.DrawTexture(rect, buttonTexture);
            
            if (Event.current.isMouse && Event.current.button == 0 && rect.Contains(Event.current.mousePosition))
            {
                if (Event.current.type == EventType.MouseDown)
                {
                    targetStaticState = ((!isStatic) == true ? 1 : 0);
                }
                else if (Event.current.type == EventType.MouseDrag && targetStaticState != -1)
                {
                    if (targetStaticState == (isStatic == true ? 1 : 0)) return rect.width;
                } 
                else
                {
                    targetStaticState = -1;
                    return rect.width;
                }
                
                Event.current.Use();
                bool showWarning = QHierarchySettings.getSetting<bool>(QHierarchySetting.ShowModifierWarning);

                List<GameObject> targetGameObjects = new List<GameObject>();
                if (Event.current.shift)
                {
                    if (!showWarning || EditorUtility.DisplayDialog("Change static", "Are you sure you want to " + (isStatic ? "disable" : "enable") + " Static of this GameObject and all its children? (You can disable this warning in the settings)", "Yes", "Cancel"))
                    {
                        getGameObjectListRecursive(ref targetGameObjects, gameObject);        
                    }
                }
                else if (Event.current.alt) 
                {
                    if (gameObject.transform.parent != null)
                    {
                        if (!showWarning || EditorUtility.DisplayDialog("Change static", "Are you sure you want to " + (isStatic ? "disable" : "enable") + " Static of this GameObject and all its siblings? (You can disable this warning in the settings)", "Yes", "Cancel"))
                        {
                            getGameObjectListRecursive(ref targetGameObjects, gameObject.transform.parent.gameObject, 1);
                            targetGameObjects.Remove(gameObject.transform.parent.gameObject);
                        }
                    }
                    else
                    {
                        Debug.Log("This action for root objects is supported only for Unity3d 5.3.3 and above");
                    }
                }
                else 
                {
                    if (Selection.Contains(gameObject))
                    {
                        targetGameObjects.AddRange(Selection.gameObjects);
                    }
                    else
                    {
                        getGameObjectListRecursive(ref targetGameObjects, gameObject, 0);
                    }
                }

                setStatic(targetGameObjects, !isStatic);
            }
            return rect.width; 
        }

        public static void setStatic(List<GameObject> gameObjects, bool targetStatic)
        {
            for (int i = gameObjects.Count - 1; i >= 0; i--)
            {     
                GameObject curGameObject = gameObjects[i];
                Undo.RecordObject(curGameObject, "static change");

                curGameObject.isStatic = targetStatic;

                EditorUtility.SetDirty(curGameObject);
            }
        }
        #endregion

        #region Tag & Layer
        private static float drawTagLayerText(GameObject gameObject, Rect rect)
        {
            string tag = gameObject.tag;
            int layer = gameObject.layer;
            
            bool showAlways = (QHierarchySettings.getSetting<int>(QHierarchySetting.TagAndLayerType) == (int)QHierarchyTagAndLayerType.Always);

            float textWidth = 0;
            if (QHierarchySettings.getSetting<int>(QHierarchySetting.TagAndLayerSizeValueType) == (int)QHierarchyTagAndLayerSizeValueType.Pixel)
                textWidth = QHierarchySettings.getSetting<int>(QHierarchySetting.TagAndLayerSizeValue);
            else
                textWidth = QHierarchySettings.getSetting<float>(QHierarchySetting.TagAndLayerSizeValuePercent) * rect.x;

            if (!showAlways && tag == "Untagged" && layer == 0)
            {
                if (QHierarchySettings.getSetting<int>(QHierarchySetting.TagAndLayerSizeType) == (int)QHierarchyTagAndLayerSizeType.Fixed)
                {
                    rect.width = 4 + textWidth;
                    rect.x -= rect.width;
                    EditorGUI.DrawRect(rect, backgroundColor);
                    rect.x += rect.width;
                }
                else
                {
                    rect.width = 0;
                }
                
                return rect.width;
            }
            else
            {
                string layerName = LayerMask.LayerToName(layer);
                
                if (QHierarchySettings.getSetting<int>(QHierarchySetting.TagAndLayerSizeType) == (int)QHierarchyTagAndLayerSizeType.Float)
                {
                    float tagWidth = (tag == "Untagged" && !showAlways ? 0 : smallLabelStyle.CalcSize(new GUIContent(tag)).x);
                    float layerWidth = (layer == 0 && !showAlways ? 0 : smallLabelStyle.CalcSize(new GUIContent(layerName)).x);
                    rect.width = tagWidth > layerWidth ? tagWidth : layerWidth;                 
                }
                else
                {
                    rect.width = textWidth;
                }
                
                rect.width += 4;
                rect.x -= rect.width;
                rect.height = 17;
                EditorGUI.DrawRect(rect, backgroundColor);
                rect.y -= 1;
                rect.width -= 4;
                rect.x += 2;
                
                int aligment = QHierarchySettings.getSetting<int>(QHierarchySetting.TagAndLayerAligment);
                if (aligment == (int)QHierarchyTagAndLayerAligment.Left)
                    smallLabelStyle.alignment = TextAnchor.UpperLeft;
                else if (aligment == (int)QHierarchyTagAndLayerAligment.Center)
                    smallLabelStyle.alignment = TextAnchor.UpperCenter;
                else if (aligment == (int)QHierarchyTagAndLayerAligment.Right)
                    smallLabelStyle.alignment = TextAnchor.UpperRight;
                
                if (layer == 0 && tag != "Untagged" && !showAlways)
                {
                    rect.y += 4;
                    smallLabelStyle.normal.textColor = greyLightColor;
                    EditorGUI.LabelField(rect, tag, smallLabelStyle);
                }
                else if (layer != 0 && tag == "Untagged" && !showAlways)
                {
                    rect.y += 4;
                    smallLabelStyle.normal.textColor = greyColor;
                    EditorGUI.LabelField(rect, layerName, smallLabelStyle);
                }
                else
                {
                    smallLabelStyle.normal.textColor = greyLightColor;
                    EditorGUI.LabelField(rect, tag, smallLabelStyle);
                    rect.y += 8;
                    smallLabelStyle.normal.textColor = greyColor;
                    EditorGUI.LabelField(rect, layerName, smallLabelStyle);
                    rect.y -= 7;
                }
                
                return rect.width + 4;
            }
        }       
        #endregion

        #region Wireframe button
        static float drawWireframeButton(GameObject gameObject, Rect rect)
        {
            Renderer renderer = gameObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                bool wireframeHiddenObjectsContains = objectList == null ? false : objectList.wireframeHiddenObjects.Contains(gameObject);

                rect.x -= 14;
                rect.width = 14;
                GUI.DrawTexture(rect, wireframeHiddenObjectsContains ? QHierarchyResource.getTexture(QHierarchyTexture.ButtonMeshWireframe) :
                                      (renderer.enabled ?
                                            QHierarchyResource.getTexture(QHierarchyTexture.ButtonMeshOn) : 
                                            QHierarchyResource.getTexture(QHierarchyTexture.ButtonMeshOff)));

                if (Event.current.isMouse && Event.current.button == 0 && rect.Contains(Event.current.mousePosition))
                {
                    bool isEnabled = renderer.enabled;

                    if (Event.current.type == EventType.MouseDown)
                    {
                        targetMeshState = ((!isEnabled) == true ? 1 : 0);
                    }
                    else if (Event.current.type == EventType.MouseDrag && targetMeshState != -1)
                    {
                        if (targetMeshState == (isEnabled == true ? 1 : 0)) return rect.width;
                    } 
                    else
                    {
                        targetMeshState = -1;
                        return rect.width;
                    }
                    
                    Event.current.Use();
                    
                    Undo.RecordObject(renderer, "renderer visibility change");


                    if (Event.current.control || Event.current.command)
                    {
                        if (!wireframeHiddenObjectsContains)
                        {
                            EditorUtility.SetSelectedWireframeHidden(renderer, true);
                            SceneView.RepaintAll();

                            Undo.RecordObject(getObjectList(), "renderer visibility change");
                            objectList.wireframeHiddenObjects.Add(gameObject);
                            EditorUtility.SetDirty(objectList);
                        }
                    }
                    else
                    {
                        if (wireframeHiddenObjectsContains)
                        {
                            EditorUtility.SetSelectedWireframeHidden(renderer, false);
                            SceneView.RepaintAll();

                            if (objectList != null)
                            {
                                Undo.RecordObject(getObjectList(), "renderer visibility change");
                                objectList.wireframeHiddenObjects.Remove(gameObject);
                                EditorUtility.SetDirty(objectList);
                            }
                        }
                        else
                        {
                            renderer.enabled = !isEnabled;
                        }
                    }

                    EditorUtility.SetDirty(gameObject);
                }
                return rect.width;
            }
            else
            {
                bool fixedIconWidth = QHierarchySettings.getSetting<bool>(QHierarchySetting.FixedIconWidth);
                if (fixedIconWidth)
                {
                    rect.x -= 14;
                    rect.width = 14;
                    EditorGUI.DrawRect(rect, backgroundColor);
                    return rect.width;
                }
            } 
            return 0;
        }
        #endregion
    }
}