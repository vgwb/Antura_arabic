using EA4S;
using EA4S.Core;
using EA4S.UI;
using UnityEngine;

namespace EA4S.Map
{
    public class AnturaButton : UIButton
    {
        #region Unity

        void Start()
        {
            GameObject icoNew = this.GetComponentInChildren<AnturaSpaceNewIcon>().gameObject;
            icoNew.SetActive(!(AppManager.Instance as AppManager).Player.IsFirstContact() && (AppManager.Instance as AppManager).Player.ThereIsSomeNewReward());
        }

        #endregion
    }
}