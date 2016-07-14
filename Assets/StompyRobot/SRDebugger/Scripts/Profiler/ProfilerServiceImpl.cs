namespace SRDebugger.Profiler
{
    using System.Diagnostics;
    using CircularBuffer;
    using Services;
    using SRF;
    using SRF.Service;
    using UnityEngine;

    [Service(typeof (IProfilerService))]
    public class ProfilerServiceImpl : SRServiceBase<IProfilerService>, IProfilerService
    {
        private const int FrameBufferSize = 400;
        private readonly SRList<ProfilerCameraListener> _cameraListeners = new SRList<ProfilerCameraListener>();
        private readonly CircularBuffer<ProfilerFrame> _frameBuffer = new CircularBuffer<ProfilerFrame>(FrameBufferSize);
        private Camera[] _cameraCache = new Camera[6];
        private int _expectedCameraCount;
        //private double _lateUpdateDuration;

        private ProfilerLateUpdateListener _lateUpdateListener;
        private double _renderDuration;
        private int _reportedCameras;

        private Stopwatch _stopwatch = new Stopwatch();
        private double _updateDuration;
        private double _updateToRenderDuration;
        public float AverageFrameTime { get; private set; }
        public float LastFrameTime { get; private set; }

        public CircularBuffer<ProfilerFrame> FrameBuffer
        {
            get { return _frameBuffer; }
        }

        protected void PushFrame(double totalTime, double updateTime, double renderTime)
        {
            //UnityEngine.Debug.Log("Frame: u: {0} r: {1}".Fmt(updateTime, renderTime));

            _frameBuffer.PushBack(new ProfilerFrame
            {
                OtherTime = totalTime - updateTime - renderTime,
                UpdateTime = updateTime,
                RenderTime = renderTime
            });
        }

        protected override void Awake()
        {
            base.Awake();
            _lateUpdateListener = gameObject.AddComponent<ProfilerLateUpdateListener>();
            _lateUpdateListener.OnLateUpdate = OnLateUpdate;

            CachedGameObject.hideFlags = HideFlags.NotEditable;

            CachedTransform.SetParent(Hierarchy.Get("SRDebugger"), true);

            //StartCoroutine(EndFrameCoroutine());
        }

        protected override void Update()
        {
            base.Update();

            // Set the frame time for the last frame
            if (FrameBuffer.Size > 0)
            {
                var frame = FrameBuffer.Back();
                frame.FrameTime = Time.deltaTime;
                FrameBuffer[FrameBuffer.Size - 1] = frame;
            }

            LastFrameTime = Time.deltaTime;

            var frameCount = Mathf.Min(20, FrameBuffer.Size);

            var f = 0d;
            for (var i = 0; i < frameCount; i++)
            {
                f += FrameBuffer[i].FrameTime;
            }

            AverageFrameTime = (float) f/frameCount;

            // Detect frame skip
            if (_reportedCameras != _cameraListeners.Count)
            {
                //Debug.LogWarning("[SRDebugger] Some cameras didn't report render time last frame");
            }

            if (_stopwatch.IsRunning)
            {
                //PushFrame(_stopwatch.Elapsed.TotalSeconds, _updateDuration, _renderDuration);
                _stopwatch.Stop();
                _stopwatch.Reset();
            }

            _updateDuration = _renderDuration = _updateToRenderDuration = 0;
            _reportedCameras = 0;

            CameraCheck();

            _expectedCameraCount = 0;

            for (var i = 0; i < _cameraListeners.Count; i++)
            {
                if (!_cameraListeners[i].isActiveAndEnabled || !_cameraListeners[i].Camera.isActiveAndEnabled)
                {
                    continue;
                }

                _expectedCameraCount++;
            }

            _stopwatch.Start();
        }

        private void OnLateUpdate()
        {
            _updateDuration = _stopwatch.Elapsed.TotalSeconds;
        }

        /*private IEnumerator EndFrameCoroutine()
		{

			var endFrame = new WaitForEndOfFrame();

			while (true) {

				yield return endFrame;
				EndFrame();

			}
			
		}*/

        private void EndFrame()
        {
            if (_stopwatch.IsRunning)
            {
                PushFrame(_stopwatch.Elapsed.TotalSeconds, _updateDuration, _renderDuration);

                _stopwatch.Reset();
                _stopwatch.Start();
            }
        }

        private void CameraDurationCallback(ProfilerCameraListener listener, double duration)
        {
            /*// Time to first camera
			if (_reportedCameras == 0) {
				_updateToRenderDuration = _stopwatch.Elapsed.TotalSeconds - duration - _updateDuration;
			}*/

            //_renderDuration += duration;
            _reportedCameras++;

            _renderDuration = _stopwatch.Elapsed.TotalSeconds - _updateDuration - _updateToRenderDuration;

            if (_reportedCameras >= _expectedCameraCount)
            {
                EndFrame();
            }
        }

        private void CameraCheck()
        {
            // Check for cameras which have been destroyed
            for (var i = _cameraListeners.Count - 1; i >= 0; i--)
            {
                if (_cameraListeners[i] == null)
                {
                    _cameraListeners.RemoveAt(i);
                }
            }

            // If camera count has not changed, return
            if (Camera.allCamerasCount == _cameraListeners.Count)
            {
                return;
            }

            // If cache is not large enough to contain current camera count, resize
            if (Camera.allCamerasCount > _cameraCache.Length)
            {
                _cameraCache = new Camera[Camera.allCamerasCount];
            }

            var cameraCount = Camera.GetAllCameras(_cameraCache);

            // Iterate all camera objects and create camera listener for those without
            for (var i = 0; i < cameraCount; i++)
            {
                var c = _cameraCache[i];

                var found = false;

                for (var j = 0; j < _cameraListeners.Count; j++)
                {
                    if (_cameraListeners[j].Camera == c)
                    {
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    continue;
                }

                //Debug.Log("[SRDebugger] Found New Camera: {0}".Fmt(c.name));

                var listener = c.gameObject.AddComponent<ProfilerCameraListener>();
                listener.hideFlags = HideFlags.NotEditable | HideFlags.DontSave;
                listener.RenderDurationCallback = CameraDurationCallback;

                _cameraListeners.Add(listener);
            }
        }
    }
}
