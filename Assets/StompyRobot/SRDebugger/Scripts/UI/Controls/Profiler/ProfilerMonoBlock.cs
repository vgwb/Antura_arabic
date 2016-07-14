namespace SRDebugger.UI.Controls.Profiler
{
    using System;
    using SRF;
    using UnityEngine;
    using UnityEngine.UI;

    public class ProfilerMonoBlock : SRMonoBehaviourEx
    {
        private float _lastRefresh;

        [RequiredField] public Text CurrentUsedText;

        [RequiredField] public GameObject NotSupportedMessage;

        [RequiredField] public Slider Slider;

        [RequiredField] public Text TotalAllocatedText;
        private bool _isSupported;

        protected override void OnEnable()
        {
            base.OnEnable();

            _isSupported = Profiler.GetMonoUsedSize() > 0;
            NotSupportedMessage.SetActive(!_isSupported);
            CurrentUsedText.gameObject.SetActive(_isSupported);

            TriggerRefresh();
        }

        protected override void Update()
        {
            base.Update();

            if (SRDebug.Instance.IsDebugPanelVisible && (Time.realtimeSinceStartup - _lastRefresh > 1f))
            {
                TriggerRefresh();
                _lastRefresh = Time.realtimeSinceStartup;
            }
        }

        public void TriggerRefresh()
        {
            var max = _isSupported ? UnityEngine.Profiler.GetMonoHeapSize() : GC.GetTotalMemory(false);
            var current = UnityEngine.Profiler.GetMonoUsedSize();

            Slider.maxValue = max;
            Slider.value = current;

            var maxMb = (max >> 10);
            maxMb /= 1024; // On new line to workaround IL2CPP bug

            var currentMb = (current >> 10);
            currentMb /= 1024;

            TotalAllocatedText.text = "Total: <color=#FFFFFF>{0}</color>MB".Fmt(maxMb);

            if (currentMb > 0)
            {
                CurrentUsedText.text = "<color=#FFFFFF>{0}</color>MB".Fmt(currentMb);
            }
        }

        public void TriggerCollection()
        {
            GC.Collect();
            TriggerRefresh();
        }
    }
}
