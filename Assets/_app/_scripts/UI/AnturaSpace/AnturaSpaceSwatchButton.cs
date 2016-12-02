// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/11/25

using UnityEngine;
using UnityEngine.UI;

namespace EA4S
{
    public class AnturaSpaceSwatchButton : UIButton
    {
        public GameObject IcoLock;
        public GameObject IcoNew;
        public Image[] ColorImgs;

        [System.NonSerialized] public RewardColorItem Data;

        #region Public Methods

        public override void Lock(bool _doLock)
        {
            base.Lock(_doLock);

            IcoLock.SetActive(_doLock);
            if (_doLock) IcoNew.SetActive(false);
        }

        public void SetAsNew(bool _isNew)
        {
            IcoNew.SetActive(_isNew);
        }

        public void SetColors(Color _color0, Color _color1)
        {
            _color0.a = 1;
            _color1.a = 1;
            ColorImgs[0].color = _color0;
            ColorImgs[1].color = _color1;
        }

        #endregion
    }
}