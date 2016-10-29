// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/10/28

using UnityEngine;

namespace EA4S
{
    public class ABSMinigamesUIComponent : MonoBehaviour
    {
        public RectTransform RectTransform { get { if (rt == null) rt = this.GetComponent<RectTransform>(); return rt; } }
        RectTransform rt;
    }
}