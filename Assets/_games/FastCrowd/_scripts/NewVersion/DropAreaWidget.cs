using UnityEngine;

namespace EA4S.FastCrowd
{
    /// <summary>
    /// This Widget hides the old implementation of DropSingleArea/DropContainer
    /// </summary>
    [RequireComponent(typeof(DropContainer))]
    public class DropAreaWidget : MonoBehaviour
    {
        public DropSingleArea dropAreaPrefab;

        DropContainer container;

        void Awake()
        {
            container = GetComponent<DropContainer>();
        }

        public void AddDropArea(ILivingLetterData newWord)
        {
            DropSingleArea dropSingleArea = Instantiate(dropAreaPrefab);
            dropSingleArea.transform.SetParent(transform, false);
            dropSingleArea.transform.position = Camera.main.transform.position;
            dropSingleArea.Init(newWord, container);
            container.SetupDone(); // not-optimal but simpler to use
        }

        public void Clean()
        {
            container.Clean();
        }
    }
}
