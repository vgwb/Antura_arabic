namespace Antura.AnturaSpace
{
    public class ShopAction_Photo : ShopAction
    {
        public override void PerformAction()
        {
            // TODO: save the photo
            CommitAction();
        }

        public override bool IsLocked
        {
            get
            {
                // TODO: set a limit for photos
                return base.IsLocked;
            }
        }
    }
}