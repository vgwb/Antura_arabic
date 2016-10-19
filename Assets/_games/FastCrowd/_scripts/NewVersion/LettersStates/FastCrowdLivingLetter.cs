using System;
using EA4S;
using EA4S.FastCrowd;
using UnityEngine;

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

        if (Vector3.Distance(transform.position, antura.transform.position) < 10.0f)
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
            bool matching = GetComponent<LetterObjectView>().Model.Data.Key == currentData.Key;

            if (onDropped != null)
                onDropped(matching);
        }
    }

    public void LookAt(Vector3 position)
    {
        LerpLookAt(position, 1);
    }

    public void LerpLookAt(Vector3 position, float t)
    {
        Vector3 targetDir3D = (transform.position - position);
        if (targetDir3D.sqrMagnitude < 0.001f)
            return;

        Vector2 targetDir = new Vector2(targetDir3D.x, targetDir3D.z);
        Vector2 letterDir = new Vector2(transform.forward.x, transform.forward.z);

        targetDir.Normalize();
        letterDir.Normalize();

        var desiredAngle = AngleCounterClockwise(targetDir, Vector2.down);
        var currentAngle = AngleCounterClockwise(letterDir, Vector2.up);

        currentAngle = Mathf.LerpAngle(currentAngle, desiredAngle, t);

        transform.rotation = Quaternion.AngleAxis(currentAngle * Mathf.Rad2Deg, Vector3.up);
    }

    static float Cross(Vector2 a, Vector2 b)
    {
        return a.x * b.y - a.y * b.x;
    }

    static float AngleCounterClockwise(Vector2 a, Vector2 b)
    {
        float dot = Vector2.Dot(a.normalized, b.normalized);
        dot = Mathf.Clamp(dot, -1.0f, 1.0f);

        if (Cross(a, b) >= 0)
            return Mathf.Acos(dot);
        return Mathf.PI * 2 - Mathf.Acos(dot);
    }
}