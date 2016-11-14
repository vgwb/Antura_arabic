using UnityEngine;
using EA4S.Tobogan;
using System.Collections.Generic;

public class ToboganFeedbackGraphics : MonoBehaviour
{
    Queue<bool> answersResults = new Queue<bool>();

    public LettersTower tower;
    public HeightMeter heightMeter;
    public WrongTubes wrongTubes;
    public AnturaAngerController antura;

    bool waitingForTowerRelease = false;
    bool waitingForTowerCrash = false;

    public void ShowPoorPlayerPerformanceFeedback()
    {
        answersResults.Clear();
        waitingForTowerCrash = true;
        tower.RequestCrash();
        // antura.Howl();
    }

    public void OnResult(bool result)
    {
        answersResults.Enqueue(result);
    }

    void OnLetterGoodReleased()
    {
        waitingForTowerRelease = false;
    }

    void OnTowerCrashed()
    {
        waitingForTowerCrash = false;
    }

    public void Initialize()
    {
        heightMeter.targetHeight = 0;

        tower.onCrashed += OnTowerCrashed;
    }

    void Update()
    {
        if (waitingForTowerRelease || waitingForTowerCrash)
            return;

        if (answersResults.Count > 0)
        {
            var nextValue = answersResults.Dequeue();

            if (nextValue)
            {
                waitingForTowerRelease = true;
                tower.AddLetter(OnLetterGoodReleased);
            }
            else
            {
                waitingForTowerCrash = true;

                wrongTubes.DropLetter(() =>
                {
                    bool hasToBark = tower.HasStackedLetters || antura.IsWaken;
                    tower.RequestCrash();
                    antura.WakeUp(hasToBark);
                });
            }
        }

        heightMeter.targetHeight = Mathf.Max(heightMeter.targetHeight, tower.TowerFullHeight > 0 ? tower.TowerFullHeight + 0.5f : 0);
    }
}
