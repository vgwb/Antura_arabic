using UnityEngine;

namespace Battlehub.Integration
{
    public static class MeshCombinerIntegration 
    {
        public static event IntegrationHandler Combined;
        public static event IntegrationHandler BeginEditPivot;
       
        public static void RaiseCombined(GameObject go, Mesh mesh)
        {
            if(Combined != null)
            {
                Combined(new IntegrationArgs(go, mesh));
            }
        }

        public static bool RaiseBeginEditPivot(GameObject go, Mesh mesh)
        {
            if(BeginEditPivot != null)
            {
                IntegrationArgs args = new IntegrationArgs(go, mesh);
                BeginEditPivot(args);
                return !args.Cancel;
            }

            return true;
        }


    }
}

