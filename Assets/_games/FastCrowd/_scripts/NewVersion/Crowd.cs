using UnityEngine;
using System.Collections.Generic;

namespace EA4S.FastCrowd
{
    /// <summary>
    /// This class manages the letters crowd
    /// </summary>
    public class Crowd : MonoBehaviour
    {
        public event System.Action<bool> onDropped;

        public LetterObjectView livingLetterPrefab;
        public GameObject puffPrefab;

        public int MaxConcurrentLetters = 5;

        List<FastCrowdLivingLetter> letters = new List<FastCrowdLivingLetter>();

        Queue<ILivingLetterData> toAdd = new Queue<ILivingLetterData>();
        Queue<FastCrowdLivingLetter> toDestroy = new Queue<FastCrowdLivingLetter>();
        float destroyTimer = 0;
        FastCrowdDraggableLetter dragging;

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

            for (int i = 0, count = letters.Count; i < count; ++i)
            {
                Vector3 position;
                if (letters[i].Raycast(out position, Camera.main.ScreenPointToRay(inputManager.LastPointerPosition), 100))
                {
                    dragging = letters[i].GetComponent<FastCrowdDraggableLetter>();
                    dragging.StartDragging(position - letters[i].transform.position);
                    break;
                }
            }
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

        void Update()
        {
            if (toAdd.Count > 0)
            {
                if (letters.Count < MaxConcurrentLetters)
                {
                    // Spawn!
                    LetterObjectView letterObjectView = Instantiate(livingLetterPrefab);
                    letterObjectView.transform.SetParent(transform, true);
                    Vector3 newPosition = Vector3.zero;
                    GameplayHelper.RandomPointInWalkableArea(transform.position, 20f, out newPosition);
                    letterObjectView.transform.position = newPosition;
                    letterObjectView.Init(toAdd.Dequeue());

                    var livingLetter = letterObjectView.gameObject.AddComponent<FastCrowdLivingLetter>();
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
