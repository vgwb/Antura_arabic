

using UnityEngine;

namespace Antura.UI
{
    public class AnturaSpaceModsButton : UIButton
    {
        GameObject icoNew;

        public void SetAsNew(bool _isNew)
        {
            if (icoNew == null) icoNew = this.GetComponentInChildren<AnturaSpaceNewIcon>().gameObject;
            icoNew.SetActive(_isNew);
        }
    }
}