using UnityEngine;
using System.Collections.Generic;

namespace Antura.Map
{
    /// <summary>
    /// A rope connects two pins. 
    /// It represents the flow of a learning block.
    /// </summary>
    public class Rope : MonoBehaviour
    {
        public List<Dot> dots = new List<Dot>();
        public MeshRenderer meshRenderer;

        public Dot DotForPS(int ps)
        {
            return dots[ps - 1];
        }
    }
}