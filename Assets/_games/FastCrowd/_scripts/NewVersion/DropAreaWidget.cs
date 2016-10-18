using UnityEngine;
using System.Collections.Generic;
using System;

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

        public ILivingLetterData GetActiveData()
        {
            var currentArea = container.GetActualDropArea();

            if (currentArea == null)
                return null;

            return container.GetActualDropArea().Data;
        }

        void Awake()
        {
            container = GetComponent<DropContainer>();

            // TODO: WARNING: THIS SHOULD NOT BE STATIC (possible errors on multiple game sessions, reuse, etc.)
            DropContainer.OnObjectiveBlockCompleted += OnCompleted;
        }

        public void SetMatchingOutline(bool active, bool matching)
        {
            var currentArea = container.GetActualDropArea();

            if (currentArea == null)
                return;

            if (active)
            {
                if (matching)
                    currentArea.SetMatching();
                else
                    currentArea.SetMatchingWrong();
            }
            else
                currentArea.DeactivateMatching();
        }

        public bool AdvanceArea()
        {
            if (container.GetActualDropArea() != null)
            {
                container.NextArea();
                return false;
            }

            return true;
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
