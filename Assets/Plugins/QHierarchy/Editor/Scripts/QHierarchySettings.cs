using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace qtools.qhierarchy
{
	public enum QHierarchySetting
	{
		ShowTreeMap = 0,
		ShowVisibilityButton = 1,
		ShowLockButton = 2,
		ShowGameObjectIcon = 3,
		ShowMonoBehaviourIcon = 4,
		ShowTagAndLayerText = 5,
		ShowErrorIcon = 6,
		ShowErrorIconParent = 7,
		ShowErrorIconScriptIsMissing = 8,
		ShowErrorIconReferenceIsNull = 9,
		ShowErrorIconStringIsEmpty = 10,
		TagAndLayerType = 11,
		TagAndLayerSizeType = 12,
		TagAndLayerSizeValue = 13,
        TagAndLayerAligment = 14,
		IconOrder = 15,
        FixedIconWidth = 16,
        Identation = 17,
        ShowCustomTagIcon = 18,
        CustomTagIcon = 19,
        GeneralSettingsFoldout = 20,
        AppearanceSettingsFoldout = 21,
        OrderSettingsFoldout = 22,
        ErrorIconSettingsFoldout = 23,
        TagAndLayerSettingsFoldout = 24,
        CustomTagIconFoldout = 25, 
        PreventSelectionOfLockedObjects = 26,
        LockSettingsFoldout = 27,
        ShowStaticIcon = 28,
        CustomTagIconReplace = 29,
        CustomTagIconTextureListFoldout = 30,
        ShowHiddenQHierarchyObjectList = 31,
        ShowModifierWarning = 32,
        ShowErrorForDisabledComponents = 33,
        IgnoreUnityMonobehaviour = 34,
        MonoBehaviourIconFoldout = 35,
        TagAndLayerSizeValueType = 36,
        TagAndLayerSizeValuePercent = 37,
        ShowMeshButton = 38
	}
	
	public enum QHierarchyTagAndLayerType
	{
		Always = 0,
		OnlyIfNotDefault = 1
	}
	
	public enum QHierarchyTagAndLayerSizeType
	{
		FixedIfNotDefault = 0,
        Fixed = 1,
		Float = 2
	}

    public enum QHierarchyTagAndLayerAligment
    {
        Left = 0,
        Center = 1,
        Right = 2
    }

    public enum QHierarchyTagAndLayerSizeValueType
    {
        Pixel = 0,
        Percent = 1
    }
	
	public enum QHierarchyIconType
	{
        ErrorIcon        = 0,
        TagAndLayer      = 1,
        CustomTagIcon    = 5,
		GameObjectIcon 	 = 2,				
        MeshButton       = 7,
        StaticIcon       = 6,
		VisibilityButton = 3,
		LockButton		 = 4
	}

    public class QTagTexture
    {
        public string tag;
        public Texture2D texture;
        
        public QTagTexture(string tag, Texture2D texture)
        {
            this.tag = tag;
            this.texture = texture;
        }
    }

	public class QHierarchySettings 
	{
		private const string PREFS_PREFIX = "QEditor";
        private const string DEFAULT_ORDER = "01527634";
		private static Dictionary<int, object> settings;

		static QHierarchySettings()
		{
			settings = new Dictionary<int, object>();
			settings[(int)QHierarchySetting.ShowVisibilityButton		] = getEditorSetting(QHierarchySetting.ShowVisibilityButton	 	    , true);
			settings[(int)QHierarchySetting.ShowLockButton				] = getEditorSetting(QHierarchySetting.ShowLockButton				, true);
			settings[(int)QHierarchySetting.ShowGameObjectIcon			] = getEditorSetting(QHierarchySetting.ShowGameObjectIcon		  	, true);
			settings[(int)QHierarchySetting.ShowTreeMap			 		] = getEditorSetting(QHierarchySetting.ShowTreeMap				    , true);
			settings[(int)QHierarchySetting.ShowMonoBehaviourIcon		] = getEditorSetting(QHierarchySetting.ShowMonoBehaviourIcon		, true);
			settings[(int)QHierarchySetting.ShowTagAndLayerText			] = getEditorSetting(QHierarchySetting.ShowTagAndLayerText		    , true);
			settings[(int)QHierarchySetting.ShowErrorIcon				] = getEditorSetting(QHierarchySetting.ShowErrorIcon				, true);
			settings[(int)QHierarchySetting.ShowErrorIconParent			] = getEditorSetting(QHierarchySetting.ShowErrorIconParent		    , true);
			settings[(int)QHierarchySetting.ShowErrorIconScriptIsMissing] = getEditorSetting(QHierarchySetting.ShowErrorIconScriptIsMissing , true);
			settings[(int)QHierarchySetting.ShowErrorIconReferenceIsNull] = getEditorSetting(QHierarchySetting.ShowErrorIconReferenceIsNull , false);
			settings[(int)QHierarchySetting.ShowErrorIconStringIsEmpty	] = getEditorSetting(QHierarchySetting.ShowErrorIconStringIsEmpty	, false);
            settings[(int)QHierarchySetting.FixedIconWidth              ] = getEditorSetting(QHierarchySetting.FixedIconWidth               , true);     
			settings[(int)QHierarchySetting.TagAndLayerType				] = getEditorSetting(QHierarchySetting.TagAndLayerType			    , 1);
			settings[(int)QHierarchySetting.TagAndLayerSizeType			] = getEditorSetting(QHierarchySetting.TagAndLayerSizeType		    , 0);
			settings[(int)QHierarchySetting.TagAndLayerSizeValue		] = getEditorSetting(QHierarchySetting.TagAndLayerSizeValue		    , 50);
            settings[(int)QHierarchySetting.TagAndLayerAligment         ] = getEditorSetting(QHierarchySetting.TagAndLayerAligment          , 0);
            settings[(int)QHierarchySetting.TagAndLayerSizeValueType    ] = getEditorSetting(QHierarchySetting.TagAndLayerSizeValueType     , 0);
            settings[(int)QHierarchySetting.TagAndLayerSizeValuePercent ] = getEditorSetting(QHierarchySetting.TagAndLayerSizeValuePercent  , 0.25f);

            string iconOrder = getEditorSetting(QHierarchySetting.IconOrder, DEFAULT_ORDER);
            if (iconOrder.Length != DEFAULT_ORDER.Length) iconOrder = DEFAULT_ORDER;
            List<QHierarchyIconType> iconOrderList = new List<QHierarchyIconType>();
            int type;
            for (int i = 0; i < iconOrder.Length; i++)     
            {
                if (int.TryParse(iconOrder[i].ToString(), out type))
                {
                    iconOrderList.Add((QHierarchyIconType)type);
                }
                else
                { 
                    iconOrderList.Clear();
                    QHierarchySettings.setSetting(QHierarchySetting.IconOrder, DEFAULT_ORDER);    
                    iconOrder = DEFAULT_ORDER;
                    i = -1;
                }
            }

            settings[(int)QHierarchySetting.IconOrder					] = iconOrderList;
            settings[(int)QHierarchySetting.Identation                  ] = getEditorSetting(QHierarchySetting.Identation          , 0);
            settings[(int)QHierarchySetting.ShowCustomTagIcon           ] = getEditorSetting(QHierarchySetting.ShowCustomTagIcon   , true);

            List<QTagTexture> tagTextureList = new List<QTagTexture>();
            string customTagIcon = getEditorSetting(QHierarchySetting.CustomTagIcon, "");
            string[] customTagIconArray = customTagIcon.Split(new char[]{';'});
            List<string> tags = new List<string>(UnityEditorInternal.InternalEditorUtility.tags);
            for (int i = 0; i < customTagIconArray.Length - 1; i+=2)
            {
                string tag = customTagIconArray[i];
                if (!tags.Contains(tag)) continue;
                string texturePath = customTagIconArray[i+1];

                Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D));
                if (texture != null) 
                { 
                    QTagTexture tagTexture = new QTagTexture(tag, texture);
                    tagTextureList.Add(tagTexture);
                }  
            }
            settings[(int)QHierarchySetting.CustomTagIcon                   ] = tagTextureList;
            settings[(int)QHierarchySetting.GeneralSettingsFoldout          ] = getEditorSetting(QHierarchySetting.GeneralSettingsFoldout           , true);
            settings[(int)QHierarchySetting.AppearanceSettingsFoldout       ] = getEditorSetting(QHierarchySetting.AppearanceSettingsFoldout        , true);
            settings[(int)QHierarchySetting.OrderSettingsFoldout            ] = getEditorSetting(QHierarchySetting.OrderSettingsFoldout             , true);
            settings[(int)QHierarchySetting.ErrorIconSettingsFoldout        ] = getEditorSetting(QHierarchySetting.ErrorIconSettingsFoldout         , true);
            settings[(int)QHierarchySetting.TagAndLayerSettingsFoldout      ] = getEditorSetting(QHierarchySetting.TagAndLayerSettingsFoldout       , true);
            settings[(int)QHierarchySetting.CustomTagIconFoldout            ] = getEditorSetting(QHierarchySetting.CustomTagIconFoldout             , true);
            settings[(int)QHierarchySetting.PreventSelectionOfLockedObjects ] = getEditorSetting(QHierarchySetting.PreventSelectionOfLockedObjects  , false);
            settings[(int)QHierarchySetting.LockSettingsFoldout             ] = getEditorSetting(QHierarchySetting.LockSettingsFoldout              , true);
            settings[(int)QHierarchySetting.ShowStaticIcon                  ] = getEditorSetting(QHierarchySetting.ShowStaticIcon                   , true);
            settings[(int)QHierarchySetting.CustomTagIconReplace            ] = getEditorSetting(QHierarchySetting.CustomTagIconReplace             , true);
            settings[(int)QHierarchySetting.CustomTagIconTextureListFoldout ] = getEditorSetting(QHierarchySetting.CustomTagIconTextureListFoldout  , false);
            settings[(int)QHierarchySetting.ShowHiddenQHierarchyObjectList  ] = getEditorSetting(QHierarchySetting.ShowHiddenQHierarchyObjectList   , true);
            settings[(int)QHierarchySetting.ShowModifierWarning             ] = getEditorSetting(QHierarchySetting.ShowModifierWarning              , true);
            settings[(int)QHierarchySetting.ShowErrorForDisabledComponents  ] = getEditorSetting(QHierarchySetting.ShowErrorForDisabledComponents   , true);
            settings[(int)QHierarchySetting.IgnoreUnityMonobehaviour        ] = getEditorSetting(QHierarchySetting.IgnoreUnityMonobehaviour         , true);
            settings[(int)QHierarchySetting.MonoBehaviourIconFoldout        ] = getEditorSetting(QHierarchySetting.MonoBehaviourIconFoldout         , true);
            settings[(int)QHierarchySetting.ShowMeshButton                  ] = getEditorSetting(QHierarchySetting.ShowMeshButton                   , true);
		} 

        public static T getSetting<T>(QHierarchySetting setting)
        {
            return (T)settings[(int)setting];
        }

		public static void setSetting(QHierarchySetting setting, bool value)
		{
            setEditorSetting(setting, value);
            settings[(int)setting] = value;
            QHierarchy.repaint();
		}

		public static void setSetting(QHierarchySetting setting, int value)
		{
			setEditorSetting(setting, value);
            settings[(int)setting] = value;
            QHierarchy.repaint();
		}

		public static void setSetting(QHierarchySetting setting, string value)
		{
			setEditorSetting(setting, value);
            settings[(int)setting] = value;
            QHierarchy.repaint();
		}

        public static void setSetting(QHierarchySetting setting, float value)
        {
            setEditorSetting(setting, value);
            settings[(int)setting] = value;
            QHierarchy.repaint();
        }

        public static void setSetting(QHierarchySetting setting, List<QHierarchyIconType> value)
        {
            string result = "";
            for (int i = 0; i < value.Count; i++)            
                result += ((int)value[i]).ToString();

            setEditorSetting(setting, result);
            settings[(int)setting] =  value;
            QHierarchy.repaint(); 
        }

        public static void setSetting(QHierarchySetting setting, List<QTagTexture> tagTextureList)
        { 
            string result = "";
            for (int i = 0; i < tagTextureList.Count; i++)            
                result += tagTextureList[i].tag + ";" + AssetDatabase.GetAssetPath(tagTextureList[i].texture.GetInstanceID()) + ";";
            
            setEditorSetting(setting, result);
            settings[(int)setting] =  tagTextureList;
            QHierarchy.repaint(); 
        }

        private static bool getEditorSetting(QHierarchySetting setting, bool defaultValue)
        {
            return EditorPrefs.GetBool(PREFS_PREFIX + setting.ToString("G"), defaultValue);
        }

        private static int getEditorSetting(QHierarchySetting setting, int defaultValue)
        {
            return EditorPrefs.GetInt(PREFS_PREFIX + setting.ToString("G"), defaultValue);
        } 

        private static string getEditorSetting(QHierarchySetting setting, string defaultValue)
        {
            return EditorPrefs.GetString(PREFS_PREFIX + setting.ToString("G"), defaultValue);
        }

        private static float getEditorSetting(QHierarchySetting setting, float defaultValue)
        {
            return EditorPrefs.GetFloat(PREFS_PREFIX + setting.ToString("G"), defaultValue);
        }

        private static void setEditorSetting(QHierarchySetting setting, bool value)
        {
            EditorPrefs.SetBool(PREFS_PREFIX + setting.ToString("G"), value);
        }
        
        private static void setEditorSetting(QHierarchySetting setting, int value)
        {
            EditorPrefs.SetInt(PREFS_PREFIX + setting.ToString("G"), value);
        }
        
        private static void setEditorSetting(QHierarchySetting setting, string value)
        {
            EditorPrefs.SetString(PREFS_PREFIX + setting.ToString("G"), value);
        }

        private static void setEditorSetting(QHierarchySetting setting, float value)
        {
            EditorPrefs.SetFloat(PREFS_PREFIX + setting.ToString("G"), value);
        }
	}
}