using UnityEngine;

namespace Battlehub.Integration
{
  
    public class IntegrationArgs
    {
        public GameObject GameObject;
        public Mesh Mesh;
        public bool Cancel;

        public IntegrationArgs()
        {

        }

        public IntegrationArgs(GameObject gameObject)
        {
            GameObject = gameObject;
        }

        public IntegrationArgs(GameObject gameObject, Mesh mesh)
        {
            GameObject = gameObject;
            Mesh = mesh;
        }
    }

    public delegate void IntegrationHandler(IntegrationArgs args);
}

