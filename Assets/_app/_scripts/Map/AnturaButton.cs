using EA4S.UI;
using UnityEngine;

namespace EA4S.Map
{
    public class AnturaButton : UIButton
    {
        #region Unity

        void Start()
        {
            GameObject icoNew = GetComponentInChildren<AnturaSpaceNewIcon>().gameObject;
            icoNew.SetActive(!AppManager.I.Player.IsFirstContact() && AppManager.I.Player.ThereIsSomeNewReward());
        }

        #endregion
    }
}