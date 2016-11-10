using System;
using EA4S;
using UnityEngine;
using System.Collections.Generic;

public class ReadingBarSet : MonoBehaviour
{
    public bool active = true;

    public ReadingBar readingBarPrefab;

    List<ReadingBar> bars = new List<ReadingBar>();

    public Camera mainCamera;

    public Transform barsStart;
    public float distanceBetweenBars = 3;
    int completedBars = 0;

    ReadingBar activeBar;
    Vector3 barsStartInitialPosition;

    void Awake()
    {
        barsStartInitialPosition = barsStart.transform.localPosition;
    }

    void SetActiveBar(ReadingBar bar)
    {
        if (activeBar != null)
            activeBar.Active = false;

        activeBar = bar;
        if (activeBar != null)
            activeBar.Active = true;
    }

    public void Clear()
    {
        SetActiveBar(null);

        // Clear past data
        foreach (var b in bars)
        {
            Destroy(b.gameObject);
        }
        bars.Clear();

        completedBars = 0;
        barsStart.transform.localPosition = barsStartInitialPosition;
    }

    public void SetData(ILivingLetterData data)
    {
        string text = data.TextForLivingLetter;

        SetData(text.Split(' '));
    }

    void SetData(string[] words)
    {
        Clear();

        int wordsCount = words.Length;

        var currentReadingBar = GameObject.Instantiate(readingBarPrefab);
        currentReadingBar.transform.SetParent(barsStart);
        currentReadingBar.transform.localPosition = Vector3.zero;
        currentReadingBar.text.text = words[0];
        bars.Add(currentReadingBar);
        SetActiveBar(currentReadingBar);

        for (int i = 1; i < wordsCount; ++i)
        {
            var word = words[i];

            var previous = currentReadingBar.text.text;
            currentReadingBar.text.text = currentReadingBar.text.text + " " + word;

            // Evaluate split
            if (currentReadingBar.text.GetPreferredValues().x >= 10)
            {
                currentReadingBar.text.text = previous;

                currentReadingBar = GameObject.Instantiate(readingBarPrefab);
                currentReadingBar.transform.SetParent(barsStart);
                currentReadingBar.transform.localPosition = Vector3.down * (bars.Count % 2) * distanceBetweenBars;
                currentReadingBar.text.text = word;
                bars.Add(currentReadingBar);
            }
        }
    }

    public bool SwitchToNextBar()
    {
        ++completedBars;

        if (activeBar != null)
            activeBar.Complete();

        int nextId = bars.FindIndex((b) => { return b == activeBar; }) + 1;

        if (nextId >= bars.Count)
        {
            // Completed!
            SetActiveBar(null);
            return true;
        }
        else
        {
            // Switch to next
            SetActiveBar(bars[nextId]);

        }

        return false;
    }

    public ReadingBar PickGlass(Camera main, Vector2 lastPointerPosition)
    {
        if (!active || activeBar == null)
            return null;

        var barCollider = activeBar.glass.GetComponentInChildren<Collider>();

        RaycastHit hitInfo;
        if (barCollider.Raycast(mainCamera.ScreenPointToRay(lastPointerPosition), out hitInfo, 1000))
        {
            return activeBar;
        }
        return null;
    }

    void Update()
    {
        for (int i = 0; i < bars.Count; ++i)
        {
            var bar = bars[i];

            int completedPairId = (completedBars / 2) * 2;

            bool show = (i >= completedPairId) && (i < completedPairId + 2);

            bar.Show(show);
        }
    }
}
