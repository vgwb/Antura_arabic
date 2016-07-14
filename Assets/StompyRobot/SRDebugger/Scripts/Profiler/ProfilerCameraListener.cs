namespace SRDebugger.Profiler
{
    using System;
    using System.Diagnostics;
    using UnityEngine;

    [RequireComponent(typeof (Camera))]
    public class ProfilerCameraListener : MonoBehaviour
    {
        private Camera _camera;
        private Stopwatch _stopwatch;
        public Action<ProfilerCameraListener, double> RenderDurationCallback;

        public Camera Camera
        {
            get { return _camera; }
        }

        private void OnEnable()
        {
            _camera = GetComponent<Camera>();
            _stopwatch = new Stopwatch();
        }

        private void OnPreCull()
        {
            _stopwatch.Start();
        }

        private void OnPostRender()
        {
            var renderTime = _stopwatch.Elapsed.TotalSeconds;

            _stopwatch.Stop();
            _stopwatch.Reset();

            if (RenderDurationCallback == null)
            {
                Destroy(this);
                return;
            }

            RenderDurationCallback(this, renderTime);
        }
    }
}
