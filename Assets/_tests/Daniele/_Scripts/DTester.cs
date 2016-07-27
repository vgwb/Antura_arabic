// Author: Daniele Giardini - http://www.demigiant.com
using UnityEngine;

namespace EA4S
{
    /// <summary>
    /// Just used to test stuff here and there. Not to be included in final build
    /// </summary>
    public class DTester : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) {
                SceneTransitioner.Show(!SceneTransitioner.IsShown);
            }
        }
    }
}