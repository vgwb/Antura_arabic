using UnityEngine;

namespace Battlehub.Integration
{ 
    public static class MeshDeformerIntegration
    {
        public static event IntegrationHandler BeforeDeform;

        public static bool RaiseBeforeDeform(GameObject gameObject, Mesh mesh)
        {
            if(BeforeDeform != null)
            {
                IntegrationArgs args = new IntegrationArgs(gameObject, mesh);
                BeforeDeform(args);
                return !args.Cancel;
            }

            return true;
        }
    }
}

