using EA4S;
using EA4S.FastCrowd;
using UnityEngine;

public class FastCrowdLivingLetter : MonoBehaviour
{
    public event System.Action<bool> onDropped;
    public event System.Action onDestroy;

    GameStateManager stateManager = new GameStateManager();

    public LetterWalkingState WalkingState { get; private set; }
    public LetterIdleState IdleState { get; private set; }
    public LetterStealthState StealthState { get; private set; }
    public LetterFallingState FallingState { get; private set; }

    // Use Scare() method instead
    private LetterScaredState ScaredState { get; set; }

    LetterObjectView thisView;

    void Awake()
    {
        thisView = GetComponent<LetterObjectView>();

        WalkingState = new LetterWalkingState(this);
        IdleState = new LetterIdleState(this);
        StealthState = new LetterStealthState(this);
        ScaredState = new LetterScaredState(this);
        FallingState = new LetterFallingState(this);

        // TODO: WARNING: THIS SHOULD NOT BE STATIC (possible errors on multiple game sessions, reuse, etc.)
        Droppable.OnRightMatch += OnDroppedRight;
        Droppable.OnWrongMatch += OnDroppedWrong;

        SetCurrentState(IdleState);
    }

    void OnDroppedRight(LetterObjectView letter)
    {
        if (letter == thisView)
        {
            if (onDropped != null)
                onDropped(true);
        }
    }

    void OnDroppedWrong(LetterObjectView letter)
    {
        if (letter == thisView)
        {
            if (onDropped != null)
                onDropped(false);
        }
    }

    void Update()
    {
        stateManager.Update(Time.deltaTime);
    }

    void FixedUpdate()
    {
        stateManager.UpdatePhysics(Time.fixedDeltaTime);
    }

    public void SetCurrentState(LetterState letterState)
    {
        stateManager.CurrentState = letterState;
    }

    /// <summary>
    /// Scare time is the duration of being in scared state
    /// </summary>
    public void Scare(Vector3 scareSource, float scareTime)
    {
        ScaredState.ScaredDuration = scareTime;
        ScaredState.ScareSource = scareSource;
        SetCurrentState(ScaredState);
    }

    void OnDestroy()
    {
        // TODO: WARNING: THIS SHOULD NOT BE STATIC (possible errors on multiple game sessions, reuse, etc.)
        Droppable.OnWrongMatch -= OnDroppedWrong;
        Droppable.OnRightMatch -= OnDroppedRight;

        if (onDestroy != null)
            onDestroy();
    }
}