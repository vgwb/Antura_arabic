using UnityEngine;
namespace Battlehub.Integration
{
    public static class WiresIntegration 
    {
        public static event IntegrationHandler WireParamsChanged;
        public static event IntegrationHandler BeforeWireCreated;

        public static bool RaiseBeforeWireCreated(GameObject gameObject, Mesh mesh)
        {
            if (BeforeWireCreated != null)
            {
                IntegrationArgs args = new IntegrationArgs(gameObject, mesh);
                BeforeWireCreated(args);
                return !args.Cancel;
            }

            return true;
        }

        public static void RaiseWireParamsChanged(GameObject gameObject, Mesh mesh)
        {
            if (WireParamsChanged != null)
            {
                WireParamsChanged(new IntegrationArgs(gameObject, mesh));
            }
        }
    }
}
