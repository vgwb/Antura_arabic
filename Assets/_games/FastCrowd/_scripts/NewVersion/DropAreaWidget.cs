using UnityEngine;
using System.Collections.Generic;

namespace EA4S.FastCrowd
{
    /// <summary>
    /// This Widget hides the old implementation of DropSingleArea/DropContainer
    /// </summary>
    [RequireComponent(typeof(DropContainer))]
    public class DropAreaWidget : MonoBehaviour
    {
        public DropSingleArea dropAreaPrefab;

        public event System.Action OnComplete;

        Dictionary<ILivingLetterData, DropSingleArea> letters = new Dictionary<ILivingLetterData, DropSingleArea>();

        DropContainer container;

        void Awake()
        {
            container = GetComponent<DropContainer>();

            // TODO: WARNING: THIS SHOULD NOT BE STATIC (possible errors on multiple game sessions, reuse, etc.)
            DropContainer.OnObjectiveBlockCompleted += OnCompleted;
        }

        void OnDestroy()
        {
            // TODO: WARNING: THIS SHOULD NOT BE STATIC (possible errors on multiple game sessions, reuse, etc.)
            DropContainer.OnObjectiveBlockCompleted -= OnCompleted;
        }

        public void AddDropArea(ILivingLetterData newElement)
        {
            DropSingleArea dropSingleArea = Instantiate(dropAreaPrefab);
            dropSingleArea.transform.SetParent(transform, false);
            dropSingleArea.transform.position = Camera.main.transform.position;
            dropSingleArea.Init(newElement, container);
            container.SetupDone(); // not-optimal but simpler to use

            letters[newElement] = dropSingleArea;
        }

        public void Clean()
        {
            container.Clean();
            letters.Clear();
        }

        void OnCompleted()
        {
            if (OnComplete != null)
                OnComplete();
        }
    }
}
