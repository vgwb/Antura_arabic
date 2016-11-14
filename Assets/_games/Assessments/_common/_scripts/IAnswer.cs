using UnityEngine;

namespace EA4S.Assessment
{
    public interface IAnswer
    {
        /// <summary>
        /// Access the GameObject of this answer
        /// </summary>
        GameObject gameObject { get; }

        /// <summary>
        /// Links the answer game object
        /// </summary>
        void SetGameObject( GameObject gameObject);
    }
}