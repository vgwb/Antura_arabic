/*
Copyright (c) 2015 Eric Begue (ericbeg@gmail.com)

This source file is part of the Panda BT package, which is licensed under
the Unity's standard Unity Asset Store End User License Agreement ("Unity-EULA").

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Panda
{
    public class PandaBehaviour : BehaviourTree
    {

        #region status tasks

        /// <summary>
        /// Succeed immediately.
        /// </summary>
        [Task]
        public void Succeed() { Task.current.Succeed(); }

        /// <summary>
        /// Fail immediately.
        /// </summary>
        [Task]
        public void Fail() { Task.current.Fail(); }

        /// <summary>
        /// Run indefinitely.
        /// </summary>
        [Task]
        public void Running() { }
        #endregion

        #region time tasks

        /// <summary>
        /// Run for \p duration seconds then succeed.
        /// </summary>
        /// <param name="message"></param>
        [Task]
        public void Wait(float duration)
        {
            var task = Task.current;
            if (task.isStarting)
                task.item = Time.time;

            float elapsedTime = Time.time - (float)Task.current.item;

            if (Task.isInspected)
            {
                float tta = Mathf.Clamp(duration - elapsedTime, 0.0f, float.PositiveInfinity);
                task.debugInfo = string.Format("t-{0:0.000}", tta);
            }

            if (elapsedTime >= duration)
            {
                task.debugInfo = "t-0.000";
                task.Succeed();
            }
        }

        /// <summary>
        /// Run for \p ticks ticks then succeed.
        /// </summary>
        /// <param name="ticks"></param>
        [Task]
        public void Wait(int ticks)
        {
            var task = Task.current;
            int n = 0;
            if (task.isStarting)
            {

                task.item = n;
            }
            else
            {
                // increment tickcount
                n = (int)task.item;
                ++n;
                task.item = n;
            }

            if (Task.isInspected)
                task.debugInfo = string.Format("n-{0}", ticks - n);

            if (n >= ticks)
            {
                task.debugInfo = "n-0";
                task.Succeed();
            }

        }

        /// <summary>
        /// Run for \p duration unscaled seconds then succeed.
        /// </summary>
        /// <param name="duration"></param>
        [Task]
        public void RealtimeWait(float duration)
        {
            var task = Task.current;

            if (task.isStarting)
                task.item = Time.realtimeSinceStartup;

            float elapsedTime = Time.realtimeSinceStartup - (float)Task.current.item;

            float tta = duration - elapsedTime;
            if (tta < 0.0f) tta = 0.0f;

            if (Task.isInspected)
                task.debugInfo = string.Format("t-{0:0.000}", tta);

            if (elapsedTime >= duration)
            {
                task.debugInfo = "t-0.000";
                task.Succeed();
            }
        }

        #endregion

        #region input tasks
        // Is*

        /// <summary>
        /// Succeed if the key \p keycode is down on the current tick, otherwise fail.
        /// </summary>
        /// <param name="keycode"></param>
        [Task]
        public void IsKeyDown(string keycode)
        {
            KeyCode k = GetKeyCode( keycode );
            Task.current.Complete( Input.GetKeyDown(k) );
        }

        /// <summary>
        /// Succeed if the key \p keycode is up on the current tick, otherwise fail.
        /// </summary>
        /// <param name="keycode"></param>
        [Task]
        public void IsKeyUp(string keycode)
        {
            KeyCode k = GetKeyCode( keycode );
            Task.current.Complete( Input.GetKeyUp(k));
        }

        /// <summary>
        /// Succeed if the key \p keycode is pressed on the current tick, otherwise fail.
        /// </summary>
        /// <param name="keycode"></param>
        [Task]
        public void IsKeyPressed(string keycode)
        {
            KeyCode k = GetKeyCode( keycode );
            Task.current.Complete(Input.GetKey(k));
        }

        /// <summary>
        /// Succeed if the mouse button \p button is pressed on the current tick, otherwise fail.
        /// </summary>
        /// <param name="button"></param>
        [Task]
        public void IsMouseButtonPressed(int button)
        {
            Task.current.Complete(Input.GetMouseButton(button));
        }


        /// <summary>
        /// Succeed if the mouse button \p button is up on the current tick, otherwise fail.
        /// </summary>
        /// <param name="button"></param>
        [Task]
        public void IsMouseButtonUp(int button)
        {
            Task.current.Complete(Input.GetMouseButtonUp(button));
        }

        /// <summary>
        /// Succeed if the mouse button \p button is down on the current tick, otherwise fail.
        /// </summary>
        /// <param name="button"></param>
        [Task]
        public void IsMouseButtonDown(int button)
        {
            Task.current.Complete(Input.GetMouseButtonDown(button));
        }

        /// <summary>
        /// Succeed if the button \p buttonName is up on the current tick, otherwise fail.
        /// </summary>
        /// <param name="button"></param>
        [Task]
        public void IsButtonUp(string buttonName)
        {
            Task.current.Complete(Input.GetButtonUp(buttonName));
        }

        /// <summary>
        /// Succeed if the button \p buttonName is down on the current tick, otherwise fail.
        /// </summary>
        /// <param name="button"></param>
        [Task]
        public void IsButtonDown(string buttonName)
        {
            Task.current.Complete(Input.GetButtonDown(buttonName));
        }

        /// <summary>
        /// Succeed if the button \p buttonName is pressed on the current tick, otherwise fail.
        /// </summary>
        /// <param name="button"></param>
        [Task]
        public void IsButtonPressed(string buttonName)
        {
            Task.current.Complete(Input.GetButton(buttonName));
        }


        // Wait*
        /// <summary>
        /// Run indefinitely and succeed when the key \p keycode is down.
        /// </summary>
        /// <param name="keycode"></param>
        [Task]
        public void WaitKeyDown(string keycode)
        {
            KeyCode k = GetKeyCode( keycode );

            if (Input.GetKeyDown(k))
                Task.current.Succeed();
        }

        /// <summary>
        /// Run indefinitely and succeed when any key is down.
        /// </summary>
        /// <param name="keycode"></param>
        [Task]
        public void WaitAnyKeyDown()
        {
            var task = Task.current;
            if (!task.isStarting)
            {
                if (Input.anyKeyDown)
                    task.Succeed();
            }

        }

        /// <summary>
        /// Run indefinitely and succeed when the key \p keycode is up.
        /// </summary>
        /// <param name="keycode"></param>
        [Task]
        public void WaitKeyUp(string keycode)
        {
            KeyCode k = GetKeyCode(keycode);
            if (Input.GetKeyUp(k))
                Task.current.Succeed();

        }

        /// <summary>
        /// Run indefinitely and succeed the mouse button \p button is up.
        /// </summary>
        /// <param name="button"></param>
        [Task]
        public void WaitMouseButtonUp(int button)
        {
            if (Input.GetMouseButtonUp(button))
                Task.current.Succeed();
        }

        /// <summary>
        /// Run indefinitely and succeed the mouse button \p button is down.
        /// </summary>
        /// <param name="button"></param>
        [Task]
        public void WaitMouseButtonDown(int button)
        {
            if (Input.GetMouseButtonDown(button))
                Task.current.Succeed();
        }

        /// <summary>
        ///  Run indefinitely and succeed the button \p buttonName is up.
        /// </summary>
        /// <param name="buttonName"></param>
        [Task]
        public void WaitButtonUp(string buttonName)
        {
            if (Input.GetButtonUp(buttonName))
                Task.current.Succeed();
        }

        /// <summary>
        ///  Run indefinitely and succeed the button \p buttonName is down.
        /// </summary>
        /// <param name="buttonName"></param>
        [Task]
        public void WaitButtonDown(string buttonName)
        {
            if (Input.GetButtonUp(buttonName))
                Task.current.Succeed();
        }

        // Hold*

        /// <summary>
        /// Run indefinitely and complete when the key \p keycode is up. Succeed if the key had been held for \p duration seconds, otherwise fail.
        /// </summary>
        /// <param name="keycode"></param>
        /// <param name="duration"></param>
        [Task]
        public void HoldKey(string keycode, float duration)
        {
            var task = Task.current;
            if (task.isStarting)
                task.item = null;

            KeyCode k = GetKeyCode(keycode);

            if (Input.GetKeyDown(k))
                task.item = Time.time;

            if( task.item != null )
            {
                float downTime = (float)task.item;
                float elapsedTime = Time.time - downTime;
                
                if (Input.GetKeyUp(k))
                {
                    task.Complete( elapsedTime >= duration );
                    task.debugInfo = "Done";

                }
                else
                {
                    if (elapsedTime < duration)
                    {
                        if (Task.isInspected)
                            task.debugInfo = string.Format("{0:000}%", Mathf.Clamp01(elapsedTime / duration) * 100.0f);
                    }
                    else
                    {
                        task.debugInfo = "Waiting for key release.";
                    }
                }

            }

        }

        // IsNext*
        /// <summary>
        /// Run indefinitely and complete when any key is down. Succeed if the key is \p keycode, otherwise fail.
        /// </summary>
        /// <param name="keycode"></param>
        [Task]
        public void IsNextKeyDown(string keycode)
        {
            var task = Task.current;
            if (!task.isStarting && Input.anyKeyDown)
            {
                bool isMouseButton = false;
                for( int i=0; i < 3; i++)
                {
                    if(Input.GetMouseButton(i)  )
                    {
                        isMouseButton = true;
                        break;
                    }
                }

                if (!isMouseButton)
                {
                    KeyCode k = GetKeyCode(keycode);
                    task.Complete(Input.GetKeyDown(k));
                }
            }

        }


        #endregion

        #region debug tasks

        /// <summary>
        /// Log \p message to the console and succeed immediately.
        /// </summary>
        /// <param name="message"></param>
        [Task]
        public void DebugLog(string message)
        {
            Debug.Log(message);
            Task.current.Succeed();
        }

        /// <summary>
        /// Log the error \p message to the console and succeed immediately.
        /// </summary>
        /// <param name="message"></param>
        [Task]
        public void DebugLogError(string message)
        {
            Debug.LogError(message);
            Task.current.Succeed();
        }

        /// <summary>
        /// Log the warning \p message to the console and succeed immediately.
        /// </summary>
        /// <param name="message"></param>
        [Task]
        public void DebugLogWarning(string message)
        {
            Debug.LogWarning(message);
            Task.current.Succeed();
        }

        /// <summary>
        /// Break (pause the editor) and succeed immediately.
        /// </summary>
        [Task]
        public void DebugBreak()
        {
            Debug.Break();
            Task.current.Succeed();
        }

        #endregion


        public KeyCode GetKeyCode(string keycode)
        {
            return (KeyCode)System.Enum.Parse(typeof(KeyCode), keycode);
        }

    }
}
