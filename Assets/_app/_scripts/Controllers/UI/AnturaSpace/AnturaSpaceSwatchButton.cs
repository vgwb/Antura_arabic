// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/11/25

using UnityEngine;

namespace EA4S
{
    public class AnturaSpaceSwatchButton : UIButton
    {
        public GameObject IcoLock;
        public GameObject IcoNew;

        [System.NonSerialized] public RewardColorItem Data;

        #region Public Methods

        public void Lock(bool _doLock)
        {
            IcoLock.SetActive(_doLock);
        }

        public void SetAsNew(bool _isNew)
        {
            IcoNew.SetActive(_isNew);
        }

        #endregion
    }
}