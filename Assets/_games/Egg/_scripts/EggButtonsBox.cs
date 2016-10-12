using System.Collections.Generic;
using UnityEngine;

namespace EA4S.Egg
{
    public class EggButtonsBox : MonoBehaviour
    {
        GameObject eggButtonPrefab;

        List<EggButton> eggButtons = new List<EggButton>();

        float buttonDistance = 20f;

        int buttonCount;

        public void Initialize(GameObject eggButtonPrefab)
        {
            this.eggButtonPrefab = eggButtonPrefab;
        }

        public void AddButton(ILivingLetterData letterData)
        {
            EggButton eggButton = CreateButton();

            eggButton.SetAnswer(letterData);
        }

        public void RemoveButtons()
        {
            for (int i = 0; i < eggButtons.Count; i++)
            {
                Destroy(eggButtons[i].gameObject);
            }

            eggButtons.Clear();
        }

        EggButton CreateButton()
        {
            EggButton eggButton = GameObject.Instantiate(eggButtonPrefab).GetComponent<EggButton>();
            eggButton.transform.SetParent(transform, false);
            return eggButton;
        }

        public void SetButtonsOnPosition()
        {
            buttonCount = eggButtons.Count;

            Vector3[] buttonsPosition = CalculateButtonPositions();

            List<int> buttonsIndex = new List<int>();

            for (int i = 0; i < buttonCount; i++)
            {
                buttonsIndex.Add(i);
            }

            for (int i = 0; i < buttonsPosition.Length; i++)
            {
                int index = Random.Range(0, buttonsIndex.Count);
                int currentIndex = buttonsIndex[index];
                buttonsIndex.RemoveAt(index);

                eggButtons[currentIndex].transform.position = buttonsPosition[i];
            }
        }

        Vector3[] CalculateButtonPositions()
        {
            Vector3[] buttonsPosition = new Vector3[buttonCount];

            Vector2 eggSizeDelta = ((RectTransform)eggButtons[0].transform).sizeDelta;

            Vector3 currentPosition = Vector3.zero;

            float positionUp = eggSizeDelta.y + (buttonDistance / 2f);
            float positionDown = -positionUp;

            int upLineLength = 0;
            int downLineLength = 0;

            if (buttonCount <= 4)
            {
                upLineLength = buttonCount;
                downLineLength = 0;
            }
            else
            {
                upLineLength = (buttonCount == 5 || buttonCount == 6) ? 3 : 4;
                downLineLength = (buttonCount % 2) == 0 ? upLineLength : upLineLength - 1;
            }

            int lineIndex = 0;
            bool goDown = false;

            for (int i = 0; i < buttonCount; i++)
            {
                if (buttonCount <= 4)
                {
                    currentPosition.y = 0f;
                }
                else
                {
                    currentPosition.y = goDown ? positionDown : positionUp;
                }

                currentPosition.x = GetHorizontalPositions(eggSizeDelta.x, goDown ? downLineLength : upLineLength)[lineIndex];

                lineIndex++;
                if (lineIndex >= upLineLength)
                {
                    goDown = true;
                    lineIndex = 0;
                }

                buttonsPosition[i] = currentPosition;
            }

            return buttonsPosition;
        }

        float[] GetHorizontalPositions(float size, int number)
        {
            float[] horizontalPositions = new float[number];

            if (number == 1)
            {
                horizontalPositions[0] = 0f;
            }
            else
            {
                float currentHorizontal = ((size * number) + (buttonDistance * (number - 1)) / 2f);

                for (int i = 0; i < number; i++)
                {
                    if (i != 0)
                    {
                        currentHorizontal += size + buttonDistance;
                    }

                    horizontalPositions[i] = currentHorizontal;
                }
            }

            return horizontalPositions;
        }
    }
}