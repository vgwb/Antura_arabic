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
using UnityEditor;
using System.Collections;
using System.IO;

namespace Panda
{
    public class CreatePandaScriptMenu : MonoBehaviour
    {

        [MenuItem("Assets/Create/Panda BT Script")]
        static void CreatePandaScript()
        {
            string parentDirectory = "Assets";
            string filename = "New Panda Script.BT";

            if (Selection.activeObject)
            {
                var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
                if (IsDirectory(assetPath))
                {
                    parentDirectory = assetPath;
                }
                else
                {
                    parentDirectory = Path.GetDirectoryName(assetPath);
                }
            }

            string filepath = "";
            int i = 0;
            do
            {
                if (i == 0)
                {
                    filepath = string.Format("{0}/{1}.txt", parentDirectory, filename);
                }
                else
                {
                    filepath = string.Format("{0}/{1} {2}.txt", parentDirectory, filename, i);
                }
                i++;
            } while (File.Exists(filepath));

            File.WriteAllText(filepath, defaultsource);
            AssetDatabase.Refresh();
            var asset = AssetDatabase.LoadMainAssetAtPath(filepath);
            Selection.activeObject = asset;

        }


        static bool IsDirectory(string path)
        {
            FileAttributes attr = File.GetAttributes(path);
            return (attr & FileAttributes.Directory) == FileAttributes.Directory;
        }

        const string defaultsource = "tree(\"Root\")\n\tSucceed\n";
    }
}
