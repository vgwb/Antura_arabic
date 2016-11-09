using System;
using System.Collections.Generic;
using UnityEngine;

namespace EA4S
{
    public class CircleButtonBox : MonoBehaviour
    {
        public float buttonDistance = 20f;
        public int maxButtonsPerLine = 5;

        public GameObject buttonPrefab;

        List<CircleButton> buttons = new List<CircleButton>();

        Action<ILivingLetterData> buttonsCallback;

        System.Random randomGenerator;
        bool dirty = false;

        public void Start()
        {
            AddButton(null);
            AddButton(null);
            AddButton(null);
            AddButton(null);
            AddButton(null);
            AddButton(null);
        }

        public void AddButton(ILivingLetterData letterData)
        {
            CircleButton button = CreateButton();
            button.SetAnswer(letterData);
            buttons.Add(button);

            dirty = true;
        }

        public void RemoveButtons()
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                CircleButton button = buttons[i];

                button.ScaleTo(0f, 0.1f, 0f, delegate () { Destroy(button.gameObject); });
            }

            buttons.Clear();
            dirty = true;
        }

        CircleButton CreateButton()
        {
            CircleButton button = Instantiate(buttonPrefab).GetComponent<CircleButton>();
            button.transform.SetParent(transform, false);

            return button;
        }

        public void ShowButtons()
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].gameObject.SetActive(true);
            }
        }

        public void HideButtons()
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].gameObject.SetActive(false);
            }
        }

        public void UpdatePositions()
        {
            int buttonPerLine = maxButtonsPerLine;

            if (buttonPerLine == buttons.Count - 1)
                --buttonPerLine;

            int width = Mathf.Min(buttons.Count, buttonPerLine);
            int height = (buttons.Count + buttonPerLine - 1) / buttonPerLine;

            for (int i = 0; i < buttons.Count; i++)
            {
                int idX = i % width;
                int idY = i / width;

                float rowWidth = width;

                if (idY == height - 1)
                {
                    rowWidth = buttons.Count % buttonPerLine;
                    if (rowWidth == 0)
                        rowWidth = width;
                }

                buttons[i].transform.localPosition = Vector3.right * (-0.5f * (rowWidth - 1) + idX) * buttonDistance + Vector3.down * (-0.5f * (height - 1) + idY) * buttonDistance;
        }
    }

    void Update()
    {
        if (dirty)
            UpdatePositions();
    }
}
}