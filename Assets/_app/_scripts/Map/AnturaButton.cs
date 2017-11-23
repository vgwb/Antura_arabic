using Antura.AnturaSpace.UI;
using Antura.Core;
using Antura.Profile;
using Antura.UI;
using UnityEngine;

namespace Antura.Map
{
    public class AnturaButton : UIButton
    {
        void Start()
        {
            GameObject icoNew = GetComponentInChildren<AnturaSpaceNewIcon>().gameObject;
            // TODO: this should be handled by the TUtorial instead
            icoNew.SetActive(!FirstContactManager.I.IsInsideFirstContact() && AppManager.I.Player.ThereIsSomeNewReward());
        }
    }
}