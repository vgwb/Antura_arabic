using System.Collections;
using Antura.Audio;
using Antura.Core;
using UnityEngine;

namespace Antura.AnturaSpace
{
    public class ShopAction_Photo : ShopAction
    {
        // TODO: set a limit for photos to be saved
        //public int photoLimit = 10;

        void Awake()
        {
            var shopActionUI = GetComponent<ShopActionUI>();
            shopActionUI.SetAction(this);
            ShopPhotoManager.I.OnPurchaseCompleted += CommitAction;
            ShopPhotoManager.I.CurrentPhotoCost = bonesCost;
        }

        public override void PerformAction()
        {
            ShopPhotoManager.I.TakePhoto();
        }

        public override bool IsLocked
        {
            get
            {
                if (base.IsLocked) return base.IsLocked;
                return false;
            }
        }

        public override bool IsOnTheSide
        {
            get { return true; }
        }
    }
}