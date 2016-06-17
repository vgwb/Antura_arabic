//----------------------------------------------
//    Google2u: Google Doc Unity integration
//         Copyright © 2013 Litteratus
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

namespace Google2u
{
    public interface IGoogle2uRow
    {
        string GetStringData(string in_colID);
    }

    public interface IGoogle2uDB
    {
        IGoogle2uRow GetGenRow(string in_rowString);
    }

    public class Google2uComponentBase : MonoBehaviour
    {


        public virtual void AddRowGeneric(List<string> input)
        {
        }
        public virtual void Clear()
        {
        }
    }
}