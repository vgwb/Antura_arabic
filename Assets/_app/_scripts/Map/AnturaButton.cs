using Antura.Core;
using Antura.UI;
using UnityEngine;

namespace Antura.Map
{
    public class AnturaButton : UIButton
    {
        void Start()
        {
            GameObject icoNew = GetComponentInChildren<AnturaSpaceNewIcon>().gameObject;
            icoNew.SetActive(!AppManager.I.Player.IsFirstContact() && AppManager.I.Player.ThereIsSomeNewReward());
        }
    }
}