using System;
using EA4S;
using EA4S.FastCrowd;
using UnityEngine;

[RequireComponent(typeof(FastCrowdLetterMovement))]
public class FastCrowdLivingLetter : MonoBehaviour
{
    public event System.Action onDestroy;
    public event System.Action<bool> onDropped;

    GameStateManager stateManager = new GameStateManager();

    public LetterWalkingState WalkingState { get; private set; }
    public LetterIdleState IdleState { get; private set; }
    public LetterFallingState FallingState { get; private set; }
    public LetterHangingState HangingState { get; private set; }

    // Use Scare() method instead
    private LetterScaredState ScaredState { get; set; }

    Collider[] colliders;

    public Crowd crowd;
    public FastCrowdWalkableArea walkableArea { get { return crowd.walkableArea; } }
    public AnturaController antura { get { return crowd.antura; } }

    void Awake()
    {
        colliders = GetComponentsInChildren<Collider>();

        WalkingState = new LetterWalkingState(this);
        IdleState = new LetterIdleState(this);
        ScaredState = new LetterScaredState(this);
        FallingState = new LetterFallingState(this);
        HangingState = new LetterHangingState(this);

        SetCurrentState(FallingState);
    }

    void Start()
    {

    }

    void Update()
    {
        stateManager.Update(Time.deltaTime);

        // Just to be safe
        var currentState = GetCurrentState();
        if (currentState != HangingState && currentState != FallingState)
        {
            var oldPos = transform.position;

            if (oldPos.y != 0)
                oldPos.y = 0;
            transform.position = oldPos;
        }

        if (Vector3.Distance(transform.position, antura.transform.position) < 15.0f)
        {
            Scare(antura.transform.position, 5);
            return;
        }
    }

    void FixedUpdate()
    {
        stateManager.UpdatePhysics(Time.fixedDeltaTime);
    }

    public bool Raycast(out float distance, out Vector3 position, Ray ray, float maxDistance)
    {
        for (int i = 0, count = colliders.Length; i < count; ++i)
        {
            RaycastHit info;
            if (colliders[i].Raycast(ray, out info, maxDistance))
            {
                position = info.point;
                distance = info.distance;
                return true;
            }
        }
        position = Vector3.zero;
        distance = 0;
        return false;
    }

    public void SetCurrentState(LetterState letterState)
    {
        stateManager.CurrentState = letterState;
    }

    public LetterState GetCurrentState()
    {
        return (LetterState)stateManager.CurrentState;
    }

    /// <summary>
    /// Scare time is the duration of being in scared state
    /// </summary>
    public void Scare(Vector3 scareSource, float scareTime)
    {
        ScaredState.ScaredDuration = scareTime;
        ScaredState.ScareSource = scareSource;

        if (GetCurrentState() == IdleState ||
            GetCurrentState() == WalkingState)
            SetCurrentState(ScaredState);
    }

    void OnDestroy()
    {
        if (onDestroy != null)
            onDestroy();
    }

    public void DropOnArea(DropAreaWidget area)
    {
        var currentData = area.GetActiveData();

        if (currentData != null)
        {
            //bool matching = GetComponent<LetterObjectView>().Model.Data.Key == currentData.Key;
            bool matching = GetComponent<LetterObjectView>().Data == currentData;

            if (onDropped != null)
                onDropped(matching);
        }
    }
}