using Antura.Core;
using UnityEngine;

namespace Antura.AnturaSpace
{
    public class ShopAction : MonoBehaviour
    {
        public Sprite iconSprite;   // TODO: move to the DECO object instead?
        public int bonesCost;

        public bool IsLocked { get { return locked; } }

        private bool locked = false;

        #region Virtual

        public virtual void PerformAction()
        {
            // nothing to do here
        }

        public virtual void CancelAction()
        {
            // nothing to do here
        }

        public virtual void PerformDrag()
        {
            // nothing to do here
        }

        #endregion

        protected void CommitAction()
        {
            AppManager.I.Player.RemoveBones(bonesCost);
        }

        #region Locking state

        public void SetLocked(bool _locked)
        {
            locked = _locked;
        }

        // TODO: update at each bone action spending
        public virtual void InitialiseLockedState()
        {
            SetLocked(AppManager.I.Player.GetTotalNumberOfBones() <= bonesCost);
        }

        #endregion
    }
}