using EA4S;
using EA4S.FastCrowd;
using UnityEngine;

public class FastCrowdLivingLetter : MonoBehaviour
{
    GameStateManager stateManager = new GameStateManager();

    public LetterWalkingState LetterWalkingState { get; private set; }
    public LetterStealthState LetterStealthState { get; private set; }

    // Use Scare() method instead
    private LetterScaredState LetterScaredState { get; set; }


    void Awake()
    {
        LetterWalkingState = new LetterWalkingState(this);
        LetterStealthState = new LetterStealthState(this);
        LetterScaredState = new LetterScaredState(this);
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
        LetterScaredState.ScaredDuration = scareTime;
        LetterScaredState.ScareSource = scareSource;
        SetCurrentState(LetterScaredState);
    }
}