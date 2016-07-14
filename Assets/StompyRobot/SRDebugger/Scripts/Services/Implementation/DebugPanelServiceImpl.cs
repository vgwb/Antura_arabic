namespace SRDebugger.Services.Implementation
{
    using System;
    using Internal;
    using SRF;
    using SRF.Service;
    using UI;
    using UnityEngine;

    [Service(typeof (IDebugPanelService))]
    public class DebugPanelServiceImpl : ScriptableObject, IDebugPanelService
    {
        private bool? _cursorWasVisible;
        private DebugPanelRoot _debugPanelRootObject;
        private bool _isVisible;
        public event Action<IDebugPanelService, bool> VisibilityChanged;

        public bool IsLoaded
        {
            get { return _debugPanelRootObject != null; }
        }

        public bool IsVisible
        {
            get { return IsLoaded && _isVisible; }
            set
            {
                if (_isVisible == value)
                {
                    return;
                }

                if (value)
                {
                    if (!IsLoaded)
                    {
                        Load();
                    }

                    SRDebuggerUtil.EnsureEventSystemExists();

                    _debugPanelRootObject.CanvasGroup.alpha = 1.0f;
                    _debugPanelRootObject.CanvasGroup.interactable = true;
                    _debugPanelRootObject.CanvasGroup.blocksRaycasts = true;

#if UNITY_5
                    _cursorWasVisible = Cursor.visible;
#else
					_cursorWasVisible = Screen.showCursor;
#endif
                }
                else
                {
                    if (IsLoaded)
                    {
                        _debugPanelRootObject.CanvasGroup.alpha = 0.0f;
                        _debugPanelRootObject.CanvasGroup.interactable = false;
                        _debugPanelRootObject.CanvasGroup.blocksRaycasts = false;
                    }

                    if (_cursorWasVisible.HasValue)
                    {
#if UNITY_5
                        Cursor.visible = _cursorWasVisible.Value;
#else
						Screen.showCursor = _cursorWasVisible.Value;
#endif
                        _cursorWasVisible = null;
                    }
                }

                _isVisible = value;

                if (VisibilityChanged != null)
                {
                    VisibilityChanged(this, _isVisible);
                }
            }
        }

        public DefaultTabs? ActiveTab
        {
            get
            {
                if (_debugPanelRootObject == null)
                {
                    return null;
                }

                return _debugPanelRootObject.TabController.ActiveTab;
            }
        }

        public void OpenTab(DefaultTabs tab)
        {
            if (!IsVisible)
            {
                IsVisible = true;
            }

            _debugPanelRootObject.TabController.OpenTab(tab);
        }

        public void Unload()
        {
            if (_debugPanelRootObject == null)
            {
                return;
            }

            IsVisible = false;

            _debugPanelRootObject.CachedGameObject.SetActive(false);
            Destroy(_debugPanelRootObject.CachedGameObject);

            _debugPanelRootObject = null;
        }

        private void Load()
        {
            var prefab = Resources.Load<DebugPanelRoot>(SRDebugPaths.DebugPanelPrefabPath);

            if (prefab == null)
            {
                Debug.LogError("[SRDebugger] Error loading debug panel prefab");
                return;
            }

            _debugPanelRootObject = SRInstantiate.Instantiate(prefab);
            _debugPanelRootObject.name = "Panel";

            DontDestroyOnLoad(_debugPanelRootObject);

            _debugPanelRootObject.CachedTransform.SetParent(Hierarchy.Get("SRDebugger"), true);

            SRDebuggerUtil.EnsureEventSystemExists();
        }
    }
}
