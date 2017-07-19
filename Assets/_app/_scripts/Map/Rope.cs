using UnityEngine;
using System.Collections.Generic;

namespace Antura.Map
{
    // TODO: used by the PIN instead
    /// <summary>
    /// A rope connects two pins. 
    /// It represents the flow of a learning block.
    /// </summary>
    public class Rope : MonoBehaviour
    {
        //public int assignedLearningBlock;
        public Pin pin;
        public List<Dot> dots = new List<Dot>();

        public Dot DotForPS(int ps)
        {
            return dots[ps - 1];
        }
    }
}