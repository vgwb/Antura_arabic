using System;
using Antura.Core;
using UnityEngine;

namespace Antura.AnturaSpace
{
    public class ShopAction : MonoBehaviour
    {
        public Sprite iconSprite; 
        public int bonesCost;

        public virtual bool IsOnTheSide
        {
            get { return false; }
        }

        public virtual bool IsLocked
        {
            get
            {
                return AppManager.I.Player.GetTotalNumberOfBones() <= bonesCost;
            }
        }

        public Action OnActionPerformed;

        #region Virtual

        public virtual void PerformAction()
        {
            // nothing to do here
        }

        public virtual void PerformDrag()
        {
            // nothing to do here
        }

        public virtual void CancelAction()
        {
            // nothing to do here
        }

        #endregion

        protected void CommitAction()
        {
            AppManager.I.Player.RemoveBones(bonesCost);
            if (OnActionPerformed != null) OnActionPerformed();
        }

    }
}