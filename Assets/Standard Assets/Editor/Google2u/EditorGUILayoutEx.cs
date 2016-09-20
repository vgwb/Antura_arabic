// ----------------------------------------------
//     G2U: Google Spreadsheet Unity integration
//          Copyright © 2015 Litteratus
// ----------------------------------------------

namespace Google2u
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    #endregion

    public class EditorGUILayoutEx
    {
        public static int MaxWidth = 230;
        public static int ButtonWidth = 32;
        public static int ButtonHeight = 32;
        public static int GoogleButtonWidth = 393;
        public static int GoogleButtonHeight = 86;
        public static int CredentialButtonWidth = 128;
        public static int CredentialButtonHeight = 128;

        #region Icons

        public Texture UnityLogo;
        public Texture LitteratusLogo;
        public Texture HelpButton;
        public Texture EditButton;
        public Texture SaveButton;
        public Texture DeleteButton;
        public Texture RefreshButton;
        public Texture GoogleButton;
        public Texture BrowseButton;
        public Texture HideButton;

        public Texture UploadButton;
        public Texture GoogleAuthButton;
        public Texture LoginButton;
        public Texture LogoutButton;
        public Texture ClearButton;
        public Texture AddButton;
        public Texture ValidateButtonGreen;
        public Texture ValidateButtonRed;
        public Texture PlusIcon;
        public Texture MinusIcon;


        public void LoadIcons()
        {
            UnityLogo = (Texture2D)EditorGUIUtility.Load("g2u/Pwrd_By_Unity_Pri_In_sm.png");
            LitteratusLogo = (Texture2D)EditorGUIUtility.Load("g2u/Litteratus_Logo_sm.png");
            EditButton = (Texture2D)EditorGUIUtility.Load("g2u/Google2u_edit.png");
            SaveButton = (Texture2D)EditorGUIUtility.Load("g2u/Google2u_save.png");
            AddButton = (Texture2D)EditorGUIUtility.Load("g2u/Google2u_add.png");
            DeleteButton = (Texture2D)EditorGUIUtility.Load("g2u/Google2u_delete.png");
            RefreshButton = (Texture2D)EditorGUIUtility.Load("g2u/Google2u_refresh.png");
            GoogleButton = (Texture2D)EditorGUIUtility.Load("g2u/Google2u_google.png");
            HelpButton = (Texture2D)EditorGUIUtility.Load("g2u/Google2u_help.png");
            BrowseButton = (Texture2D)EditorGUIUtility.Load("g2u/Google2u_folder.png");
            HideButton = (Texture2D)EditorGUIUtility.Load("g2u/Google2u_hide.png");
            UploadButton = (Texture2D)EditorGUIUtility.Load("g2u/Google2u_upload.png");
            GoogleAuthButton = (Texture2D)EditorGUIUtility.Load("g2u/Google2u_signin.png");
            LoginButton = (Texture2D)EditorGUIUtility.Load("g2u/Google2u_login.png");
            LogoutButton = (Texture2D)EditorGUIUtility.Load("g2u/Google2u_logout.png");
            ClearButton = (Texture2D)EditorGUIUtility.Load("g2u/Google2u_clear.png");
            ValidateButtonGreen = (Texture2D)EditorGUIUtility.Load("g2u/Google2u_validate_green.png");
            ValidateButtonRed = (Texture2D)EditorGUIUtility.Load("g2u/Google2u_validate_red.png");
            PlusIcon = (Texture2D)EditorGUIUtility.Load("g2u/Google2u_plus.png");
            MinusIcon = (Texture2D)EditorGUIUtility.Load("g2u/Google2u_minus.png");
        }

        #endregion

        #region Docs

        public Texture DocAccountWorkbooks;
        public Texture DocBtnActive;
        public Texture DocBtnExport;
        public Texture DocBtnExportAs;
        public Texture DocBtnOpen;
        public Texture DocBtnRefresh;
        public Texture DocBtnValidate;
        public Texture DocDelimiters;
        public Texture DocEditCells;
        public Texture DocEditCellsType;
        public Texture DocEditCellsUpdate;
        public Texture DocExportOptions;
        public Texture DocExportTypes;
        public Texture DocFullView;
        public Texture DocGooglePublishing1;
        public Texture DocGooglePublishing2;
        public Texture DocGoogleSecurity;
        public Texture DocGoogleSharing1;
        public Texture DocGoogleSharing2;
        public Texture DocGoogleSharing3;
        public Texture DocIcons0;
        public Texture DocInvalidCell;
        public Texture DocListView;
        public Texture DocLoginInterface1;
        public Texture DocLoginInterface2;
        public Texture DocMainInterface;
        public Texture DocManualWorkbooks;
        public Texture DocManualWorkbooks2;
        public Texture DocOptionsCSV;
        public Texture DocOptionsJSON;
        public Texture DocOptionsObjDB;
        public Texture DocOptionsStaticDB;
        public Texture DocOptionsXML;
        public Texture DocPreviewCSV;
        public Texture DocPreviewJSONClass;
        public Texture DocPreviewJSONObj;
        public Texture DocPreviewXML;
        public Texture DocSpreadsheetFormat;
        public Texture DocUploadWorkbook;
        public Texture DocWhiteSpace;


        public void LoadDocImages()
        {
            DocAccountWorkbooks = (Texture2D) EditorGUIUtility.Load("g2u/Docs/google2u_AccountWorkbooks.jpg");
            DocBtnActive = (Texture2D) EditorGUIUtility.Load("g2u/Docs/google2u_BtnActive.jpg");
            DocBtnExport = (Texture2D) EditorGUIUtility.Load("g2u/Docs/google2u_BtnExport.jpg");
            DocBtnExportAs = (Texture2D) EditorGUIUtility.Load("g2u/Docs/google2u_BtnExportAs.jpg");
            DocBtnOpen = (Texture2D) EditorGUIUtility.Load("g2u/Docs/google2u_BtnOpen.jpg");
            DocBtnRefresh = (Texture2D) EditorGUIUtility.Load("g2u/Docs/google2u_BtnRefresh.jpg");
            DocBtnValidate = (Texture2D) EditorGUIUtility.Load("g2u/Docs/google2u_BtnValidate.jpg");
            DocDelimiters = (Texture2D) EditorGUIUtility.Load("g2u/Docs/google2u_Delimiters.jpg");
            DocEditCells = (Texture2D) EditorGUIUtility.Load("g2u/Docs/google2u_EditCells.jpg");
            DocEditCellsType = (Texture2D) EditorGUIUtility.Load("g2u/Docs/google2u_EditCellsType.jpg");
            DocEditCellsUpdate = (Texture2D) EditorGUIUtility.Load("g2u/Docs/google2u_EditCellsUpdate.jpg");
            DocExportOptions = (Texture2D) EditorGUIUtility.Load("g2u/Docs/google2u_ExportOptions.jpg");
            DocExportTypes = (Texture2D) EditorGUIUtility.Load("g2u/Docs/google2u_ExportTypes.jpg");
            DocFullView = (Texture2D) EditorGUIUtility.Load("g2u/Docs/google2u_FullView.jpg");
            DocGooglePublishing1 = (Texture2D) EditorGUIUtility.Load("g2u/Docs/google2u_GooglePublishing1.jpg");
            DocGooglePublishing2 = (Texture2D) EditorGUIUtility.Load("g2u/Docs/google2u_GooglePublishing2.jpg");
            DocGoogleSecurity = (Texture2D) EditorGUIUtility.Load("g2u/Docs/google2u_GoogleSecurity.jpg");
            DocGoogleSharing1 = (Texture2D) EditorGUIUtility.Load("g2u/Docs/google2u_GoogleSharing1.jpg");
            DocGoogleSharing2 = (Texture2D) EditorGUIUtility.Load("g2u/Docs/google2u_GoogleSharing2.jpg");
            DocGoogleSharing3 = (Texture2D) EditorGUIUtility.Load("g2u/Docs/google2u_GoogleSharing3.jpg");
            DocIcons0 = (Texture2D) EditorGUIUtility.Load("g2u/Docs/google2u_Icons0.jpg");
            DocInvalidCell = (Texture2D) EditorGUIUtility.Load("g2u/Docs/google2u_InvalidCell.jpg");
            DocListView = (Texture2D) EditorGUIUtility.Load("g2u/Docs/google2u_ListView.jpg");
            DocLoginInterface1 = (Texture2D) EditorGUIUtility.Load("g2u/Docs/google2u_LoginInterface.jpg");
            DocLoginInterface2 = (Texture2D) EditorGUIUtility.Load("g2u/Docs/google2u_LoginInterface2.jpg");
            DocMainInterface = (Texture2D) EditorGUIUtility.Load("g2u/Docs/google2u_MainInterface.jpg");
            DocManualWorkbooks = (Texture2D) EditorGUIUtility.Load("g2u/Docs/google2u_ManualWorkbooks.jpg");
            DocManualWorkbooks2 = (Texture2D) EditorGUIUtility.Load("g2u/Docs/google2u_ManualWorkbooks2.jpg");
            DocOptionsCSV = (Texture2D) EditorGUIUtility.Load("g2u/Docs/google2u_OptionsCSV.jpg");
            DocOptionsJSON = (Texture2D) EditorGUIUtility.Load("g2u/Docs/google2u_OptionsJSON.jpg");
            DocOptionsObjDB = (Texture2D) EditorGUIUtility.Load("g2u/Docs/google2u_OptionsObjDB.jpg");
            DocOptionsStaticDB = (Texture2D) EditorGUIUtility.Load("g2u/Docs/google2u_OptionsStaticDB.jpg");
            DocOptionsXML = (Texture2D) EditorGUIUtility.Load("g2u/Docs/google2u_OptionsXML.jpg");
            DocPreviewCSV = (Texture2D) EditorGUIUtility.Load("g2u/Docs/google2u_PreviewCSV.jpg");
            DocPreviewJSONClass = (Texture2D) EditorGUIUtility.Load("g2u/Docs/google2u_PreviewJSONClass.jpg");
            DocPreviewJSONObj = (Texture2D) EditorGUIUtility.Load("g2u/Docs/google2u_PreviewJSONObj.jpg");
            DocPreviewXML = (Texture2D) EditorGUIUtility.Load("g2u/Docs/google2u_PreviewXML.jpg");
            DocSpreadsheetFormat = (Texture2D) EditorGUIUtility.Load("g2u/Docs/google2u_SpreadsheetFormat.jpg");
            DocUploadWorkbook = (Texture2D) EditorGUIUtility.Load("g2u/Docs/google2u_UploadWorkbook.jpg");
            DocWhiteSpace = (Texture2D) EditorGUIUtility.Load("g2u/Docs/google2u_WhiteSpace.jpg");
        }

        #endregion

        #region GUI Styles

        public GUIStyle OuterBoxHeader;
        public GUIStyle OuterBox;
        public GUIStyle InnerBoxHeader;
        public GUIStyle InnerBox;
        public GUIStyle CellButton;
        public GUIStyle CellButtonActive;
        public GUIStyle CellHeader;
        public GUIStyle CellTypeButton;
        public GUIStyle CellInvalidButton;
        public GUIStyle PlusButton;
        public GUIStyle MinusButton;
        public GUIStyle VisibilityButton;
        public GUIStyle VisibilityHiddenButton;
        public GUIStyle HelpButtonStyle;
        public GUIStyle InvalidWorksheetStyle;

        #endregion

        #region GUI Colors

        public static Color PrevCol;

        public static void SetColor(Color in_col)
        {
            PrevCol = GUI.color;
            GUI.color = in_col;
        }

        public static void ResetColor()
        {
            GUI.color = PrevCol;
        }

        public static Color PrevBgColor;

        public static void SetBackgroundColor(Color in_col)
        {
            PrevBgColor = GUI.backgroundColor;
            GUI.backgroundColor = in_col;
        }

        public static void ResetBackgroundColor()
        {
            GUI.backgroundColor = PrevBgColor;
        }

        #endregion GUI Colors

        #region GUI Fade

        public class FadeArea
        {
            private bool _VisibleInLayout;
            public Rect CurrentRect;
            public Rect LastRect;
            public float LastUpdate;
            public bool Open;
            public Color PreFadeColor;
            public float Value;

            public FadeArea(bool in_open)
            {
                Value = in_open ? 1 : 0;
            }

            public void Switch()
            {
                LastRect = CurrentRect;
            }

            public bool Show()
            {
                var v = Open || Value > 0F;
                if (Event.current.type == EventType.Layout)
                {
                    _VisibleInLayout = v;
                }

                return _VisibleInLayout;
            }

            public static implicit operator bool(FadeArea in_o)
            {
                return in_o.Open;
            }
        }

        public Dictionary<string, FadeArea> FadeAreas;

        public static int CurrentDepth = 0;
        public static int CurrentIndex = 0;

        public static EditorWindow GuiEditor;

        public static GUIStyle DefaultAreaStyle;
        public static GUIStyle DefaultLabelStyle;

        public static bool Fade = true;

        public Stack<FadeArea> Stack;

        public void RemoveID(string in_id)
        {
            if (FadeAreas == null)
                return;

            FadeAreas.Remove(in_id);
        }

        public bool DrawID(string in_id)
        {
            if (FadeAreas == null)
            {
                return false;
            }

            return FadeAreas[in_id].Show();
        }


        public FadeArea BeginFadeArea(bool in_open, string in_label, string in_id, GUIStyle in_areaStyle,
            GUIStyle in_labelStyle)
        {
            var tmp1 = GUI.color;

            var fadeArea = BeginFadeArea(in_open, in_id, 26, in_areaStyle);

            var tmp2 = GUI.color;
            GUI.color = tmp1;

            EditorGUILayout.BeginHorizontal();

            if (!in_open)
            {
                if (GUILayout.Button(string.Empty, PlusButton))
                {
                    fadeArea.Open = !fadeArea.Open;
                    GuiEditor.Repaint();
                }
            }
            else
            {
                if (GUILayout.Button(string.Empty, MinusButton))
                {
                    fadeArea.Open = !fadeArea.Open;
                    GuiEditor.Repaint();
                }
            }

            if (in_label != "")
            {
                if (GUILayout.Button(in_label, in_labelStyle))
                {
                    fadeArea.Open = !fadeArea.Open;
                    GuiEditor.Repaint();
                }
            }

            EditorGUILayout.EndHorizontal();

            GUI.color = tmp2;
            return fadeArea;
        }

        public FadeArea BeginFadeArea(bool in_open, string in_label, string in_id, GUIStyle in_areaStyle,
            GUIStyle in_labelStyle, string in_WorkbookVisibilityString)
        {
            var tmp1 = GUI.color;

            var fadeArea = BeginFadeArea(in_open, in_id, 26, in_areaStyle);

            var tmp2 = GUI.color;
            GUI.color = tmp1;

            EditorGUILayout.BeginHorizontal();

            if (!in_open)
            {
                if (GUILayout.Button(string.Empty, PlusButton))
                {
                    fadeArea.Open = !fadeArea.Open;
                    GuiEditor.Repaint();
                }
            }
            else
            {
                if (GUILayout.Button(string.Empty, MinusButton))
                {
                    fadeArea.Open = !fadeArea.Open;
                    GuiEditor.Repaint();
                }
            }

            if (in_label != "")
            {
                if (GUILayout.Button(in_label, in_labelStyle))
                {
                    fadeArea.Open = !fadeArea.Open;
                    GuiEditor.Repaint();
                }
            }

            GUILayout.FlexibleSpace();

            var spreadsheetVisible = true;
            if (!string.IsNullOrEmpty(in_WorkbookVisibilityString))
                spreadsheetVisible = Google2uGUIUtil.GetBool(in_WorkbookVisibilityString, true);
            if (spreadsheetVisible)
            {
                if (GUILayout.Button(string.Empty, VisibilityButton))
                {
                    Google2uGUIUtil.SetBool(in_WorkbookVisibilityString, false);
                }
            }
            else
            {
                if (GUILayout.Button(string.Empty, VisibilityHiddenButton))
                {
                    Google2uGUIUtil.SetBool(in_WorkbookVisibilityString, true);
                }
            }

            EditorGUILayout.EndHorizontal();

            GUI.color = tmp2;
            return fadeArea;
        }

        public FadeArea BeginFadeArea(bool in_open, string in_id, float in_minHeight, GUIStyle in_areaStyle)
        {
            if (GuiEditor == null)
            {
                Debug.LogError("You need to set the 'Google2uGUI.Editor' variable before calling this function");
                return null;
            }

            if (Stack == null)
            {
                Stack = new Stack<FadeArea>();
            }

            if (FadeAreas == null)
            {
                FadeAreas = new Dictionary<string, FadeArea>();
            }

            if (!FadeAreas.ContainsKey(in_id))
            {
                FadeAreas.Add(in_id, new FadeArea(in_open));
            }

            var fadeArea = FadeAreas[in_id];

            Stack.Push(fadeArea);
            fadeArea.Open = in_open;
            in_areaStyle.stretchWidth = true;

            var lastRect = fadeArea.LastRect;

            lastRect.height = lastRect.height < in_minHeight ? in_minHeight : lastRect.height;
            lastRect.height -= in_minHeight;
            var faded = DoLerp(0F, 1F, fadeArea.Value);
            lastRect.height *= faded;
            lastRect.height += in_minHeight;
            lastRect.height = Mathf.Round(lastRect.height);

            var gotLastRect = GUILayoutUtility.GetRect(new GUIContent(), in_areaStyle, GUILayout.Height(lastRect.height));

            GUILayout.BeginArea(lastRect, in_areaStyle);

            var newRect = EditorGUILayout.BeginVertical();

            if (Event.current.type == EventType.Repaint || Event.current.type == EventType.ScrollWheel)
            {
                newRect.x = gotLastRect.x;
                newRect.y = gotLastRect.y;
                newRect.width = gotLastRect.width;
                newRect.height += in_areaStyle.padding.top + in_areaStyle.padding.bottom;
                fadeArea.CurrentRect = newRect;

                if (fadeArea.LastRect != newRect)
                {
                    GuiEditor.Repaint();
                }

                fadeArea.Switch();
            }
            if (Event.current.type == EventType.Repaint)
            {
                var value = fadeArea.Value;
                var targetValue = in_open ? 1F : 0F;

                var newRectHeight = fadeArea.LastRect.height;
                var deltaHeight = 400F/newRectHeight;

                var deltaTime = Mathf.Clamp(Time.realtimeSinceStartup - FadeAreas[in_id].LastUpdate, 0.00001F, 0.05F);

                deltaTime *= Mathf.Lerp(deltaHeight*deltaHeight*0.01F, 0.8F, 0.9F);

                FadeAreas[in_id].LastUpdate = Time.realtimeSinceStartup;

                if (Event.current.shift)
                {
                    deltaTime *= 0.05F;
                }

                if (Mathf.Abs(targetValue - value) > 0.001F)
                {
                    var time = Mathf.Clamp01(deltaTime*6);
                    value += time*Mathf.Sign(targetValue - value);
                    GuiEditor.Repaint();
                }
                else
                {
                    value = Mathf.Round(value);
                }

                fadeArea.Value = Mathf.Clamp01(value);
            }

            if (Fade)
            {
                var c = GUI.color;
                fadeArea.PreFadeColor = c;
                c.a *= fadeArea.Value;
                GUI.color = c;
            }

            fadeArea.Open = in_open;

            return fadeArea;
        }

        public void EndFadeArea()
        {
            if (Stack.Count <= 0)
            {
                Debug.LogError("You are popping more Fade Areas than you are pushing, make sure they are balanced");
                return;
            }

            var fadeArea = Stack.Pop();
            EditorGUILayout.EndVertical();
            GUILayout.EndArea();

            if (Fade)
            {
                GUI.color = fadeArea.PreFadeColor;
            }
        }

        public void ClearStack()
        {
            if (Stack != null) Stack.Clear();
        }

        public static float DoLerp(float in_start, float in_end, float in_value)
        {
            return Mathf.Lerp(in_start, in_end, in_value*in_value*(3.0f - 2.0f*in_value));
        }

        #endregion

        #region InputWrappers

        public static string StringInput(string in_label, string in_currentValue, string in_savedName)
        {
            return StringInput(in_label, in_currentValue, in_savedName, true);
        }

        public static string StringInput(string in_label, string in_currentValue, string in_savedName, bool in_doStore)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(in_label + ": ", GUILayout.MaxWidth(MaxWidth));
            var tmpvar = EditorGUILayout.TextField(in_currentValue);
            if (!string.IsNullOrEmpty(in_savedName))
            {
                if (in_doStore)
                {
                    if (tmpvar != in_currentValue)
                        Google2uGUIUtil.SetString(in_savedName, tmpvar);
                }
                else
                {
                    Google2uGUIUtil.SetString(in_savedName, string.Empty);
                }
            }
            EditorGUILayout.EndHorizontal();
            return tmpvar;
        }

        public static void StringInput(string in_label, ref string in_currentValue, string in_savedName)
        {
            StringInput(in_label, ref in_currentValue, in_savedName, true);
        }

        public static void StringInput(string in_label, ref string in_currentValue, string in_savedName, bool in_doStore)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(in_label + ": ", GUILayout.MaxWidth(MaxWidth));
            var tmpvar = EditorGUILayout.TextField(in_currentValue);
            if (!string.IsNullOrEmpty(in_savedName))
            {
                if (in_doStore)
                {
                    if (tmpvar != in_currentValue)
                        Google2uGUIUtil.SetString(in_savedName, tmpvar);
                }
                else
                {
                    Google2uGUIUtil.SetString(in_savedName, string.Empty);
                }
            }
            in_currentValue = tmpvar;
            EditorGUILayout.EndHorizontal();
        }

        public static string PasswordInput(string in_label, string in_currentValue, string in_savedName)
        {
            return PasswordInput(in_label, in_currentValue, in_savedName, true);
        }

        public static string PasswordInput(string in_label, string in_currentValue, string in_savedName, bool in_doStore)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(in_label + ": ", GUILayout.MaxWidth(MaxWidth));
            var tmpvar = EditorGUILayout.PasswordField(in_currentValue);
            if (!string.IsNullOrEmpty(in_savedName))
            {
                if (in_doStore)
                {
                    if (tmpvar != in_currentValue)
                        Google2uGUIUtil.SetString(in_savedName, tmpvar);
                }
                else
                {
                    Google2uGUIUtil.SetString(in_savedName, string.Empty);
                }
            }
            EditorGUILayout.EndHorizontal();
            return tmpvar;
        }

        public static void PasswordInput(string in_label, ref string in_currentValue, string in_savedName)
        {
            PasswordInput(in_label, ref in_currentValue, in_savedName, true);
        }

        public static void PasswordInput(string in_label, ref string in_currentValue, string in_savedName,
            bool in_doStore)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(in_label + ": ", GUILayout.MaxWidth(MaxWidth));
            var tmpvar = EditorGUILayout.PasswordField(in_currentValue);
            if (!string.IsNullOrEmpty(in_savedName))
            {
                if (in_doStore)
                {
                    if (tmpvar != in_currentValue)
                        Google2uGUIUtil.SetString(in_savedName, tmpvar);
                }
                else
                {
                    Google2uGUIUtil.SetString(in_savedName, string.Empty);
                }
            }
            in_currentValue = tmpvar;
            EditorGUILayout.EndHorizontal();
        }

        public static bool ToggleInput(string in_label, bool in_currentValue, string in_savedName)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(in_label + ": ", GUILayout.MaxWidth(MaxWidth));
            var tmpvar = EditorGUILayout.Toggle(in_currentValue);
            if (!string.IsNullOrEmpty(in_savedName) && tmpvar != in_currentValue)
                Google2uGUIUtil.SetBool(in_savedName, tmpvar);
            EditorGUILayout.EndHorizontal();
            return tmpvar;
        }

        public static void ToggleInput(string in_label, ref bool in_currentValue, string in_savedName)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(in_label + ": ", GUILayout.MaxWidth(MaxWidth));
            var tmpvar = EditorGUILayout.Toggle(in_currentValue);
            if (!string.IsNullOrEmpty(in_savedName) && tmpvar != in_currentValue)
                Google2uGUIUtil.SetBool(in_savedName, tmpvar);
            in_currentValue = tmpvar;
            EditorGUILayout.EndHorizontal();
        }

        public static float FloatInput(string in_label, float in_currentValue, string in_savedName)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(in_label + ": ", GUILayout.MaxWidth(MaxWidth));
            var tmpvar = EditorGUILayout.FloatField(in_currentValue);
            if (!string.IsNullOrEmpty(in_savedName) && Math.Abs(tmpvar - in_currentValue) > float.Epsilon)
                Google2uGUIUtil.SetFloat(in_savedName, tmpvar);
            EditorGUILayout.EndHorizontal();
            return tmpvar;
        }

        public static void FloatInput(string in_label, ref float in_currentValue, string in_savedName)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(in_label + ": ", GUILayout.MaxWidth(MaxWidth));
            var tmpvar = EditorGUILayout.FloatField(in_currentValue);
            if (!string.IsNullOrEmpty(in_savedName) && Math.Abs(tmpvar - in_currentValue) > float.Epsilon)
                Google2uGUIUtil.SetFloat(in_savedName, tmpvar);
            in_currentValue = tmpvar;
            EditorGUILayout.EndHorizontal();
        }

        public static int IntInput(string in_label, int in_currentValue, string in_savedName)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(in_label + ": ", GUILayout.MaxWidth(MaxWidth));
            var tmpvar = EditorGUILayout.IntField(in_currentValue);
            if (!string.IsNullOrEmpty(in_savedName) && tmpvar != in_currentValue)
                Google2uGUIUtil.SetInt(in_savedName, tmpvar);
            EditorGUILayout.EndHorizontal();
            return tmpvar;
        }

        public static void IntInput(string in_label, ref int in_currentValue, string in_savedName)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(in_label + ": ", GUILayout.MaxWidth(MaxWidth));
            var tmpvar = EditorGUILayout.IntField(in_currentValue);
            if (!string.IsNullOrEmpty(in_savedName) && tmpvar != in_currentValue)
                Google2uGUIUtil.SetInt(in_savedName, tmpvar);
            in_currentValue = tmpvar;
            EditorGUILayout.EndHorizontal();
        }

        #endregion
    }
}