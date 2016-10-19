using UnityEngine;
using System.Collections.Generic;
using System;

namespace EA4S.FastCrowd
{
    /// <summary>
    /// This class manages the letters crowd
    /// </summary>
    public class Crowd : MonoBehaviour
    {
        public event System.Action<bool> onDropped;

        public FastCrowdWalkableArea walkableArea;
        public AnturaController antura;

        public LetterObjectView livingLetterPrefab;
        public GameObject puffPrefab;

        public int MaxConcurrentLetters = 5;

        List<FastCrowdLivingLetter> letters = new List<FastCrowdLivingLetter>();

        Queue<ILivingLetterData> toAdd = new Queue<ILivingLetterData>();

        Queue<FastCrowdLivingLetter> toDestroy = new Queue<FastCrowdLivingLetter>();
        float destroyTimer = 0;
        FastCrowdDraggableLetter dragging;

        public void GetNearLetters(List<FastCrowdLivingLetter> output, Vector3 position, float radius)
        {
            for (int i = 0, count = letters.Count; i < count; ++i)
            {
                if (Vector3.Distance(letters[i].transform.position, position) < radius)
                {
                    output.Add(letters[i]);
                }
            }
        }

        void Awake()
        {
            var inputManager = FastCrowdConfiguration.Instance.Context.GetInputManager();

            inputManager.onPointerDown += OnPointerDown;
            inputManager.onPointerUp += OnPointerUp;
        }

        void OnPointerDown()
        {
            if (dragging != null)
                return;

            var inputManager = FastCrowdConfiguration.Instance.Context.GetInputManager();

            Vector3 draggingPosition = Vector3.zero;
            float draggingDistance = 100;

            for (int i = 0, count = letters.Count; i < count; ++i)
            {
                Vector3 position;
                float distance;
                if (letters[i].Raycast(out distance, out position, Camera.main.ScreenPointToRay(inputManager.LastPointerPosition), draggingDistance) &&
                    distance < draggingDistance)
                {
                    draggingPosition = position;
                    draggingDistance = distance;
                    dragging = letters[i].GetComponent<FastCrowdDraggableLetter>();
                }
            }

            if (dragging != null)
                dragging.StartDragging(draggingPosition - dragging.transform.position);
        }

        void OnPointerUp()
        {
            if (dragging != null)
                dragging.EndDragging();
            dragging = null;
        }

        public void AddLivingLetter(ILivingLetterData letter)
        {
            toAdd.Enqueue(letter);
        }

        public void Clean()
        {
            toAdd.Clear();

            foreach (var l in letters)
                toDestroy.Enqueue(l);

            letters.Clear();
        }

        void SpawnLetter()
        {
            // Spawn!
            LetterObjectView letterObjectView = Instantiate(livingLetterPrefab);
            letterObjectView.transform.SetParent(transform, true);
            Vector3 newPosition = walkableArea.GetFurthestSpawn(letters); // Find isolated spawn point

            letterObjectView.transform.position = newPosition;
            letterObjectView.transform.rotation = Quaternion.Euler(0, UnityEngine.Random.value*360, 0);
            letterObjectView.Init(toAdd.Dequeue());

            var livingLetter = letterObjectView.gameObject.AddComponent<FastCrowdLivingLetter>();
            livingLetter.crowd = this;

            letterObjectView.gameObject.AddComponent<FastCrowdDraggableLetter>();
            letterObjectView.gameObject.AddComponent<Rigidbody>().isKinematic = true;
            Destroy(letterObjectView.gameObject.GetComponent<Hangable>());
            var pos = letterObjectView.transform.position;
            pos.y = 10;
            letterObjectView.transform.position = pos;

            letters.Add(livingLetter);

            livingLetter.onDropped += (result) =>
            {
                if (result)
                {
                    letters.Remove(livingLetter);
                    toDestroy.Enqueue(livingLetter);
                }

                if (onDropped != null)
                    onDropped(result);
            };
        }

        void Update()
        {
            if (toAdd.Count > 0)
            {
                if (letters.Count < MaxConcurrentLetters)
                {
                    SpawnLetter();
                }
            }

            if (toDestroy.Count > 0)
            {
                destroyTimer -= Time.deltaTime;

                if (destroyTimer <= 0)
                {
                    destroyTimer = 0.1f;

                    var puffGo = GameObject.Instantiate(puffPrefab);
                    puffGo.AddComponent<AutoDestroy>().duration = 2;
                    puffGo.SetActive(true);

                    var t = toDestroy.Dequeue();
                    puffGo.transform.position = t.transform.position + Vector3.up * 3;
                    Destroy(t.gameObject);
                }
            }
        }
    }
}
