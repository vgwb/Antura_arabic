using EA4S;
using EA4S.UI;
using UnityEngine;

namespace _app._scripts.Map
{
    public class AnturaButton : UIButton
    {
        #region Unity

        void Start()
        {
            GameObject icoNew = this.GetComponentInChildren<AnturaSpaceNewIcon>().gameObject;
            icoNew.SetActive(AppManager.I.Player.ThereIsSomeNewReward());
        }

        #endregion
    }
}