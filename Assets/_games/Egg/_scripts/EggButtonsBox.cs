using System;
using System.Collections.Generic;
using UnityEngine;

namespace EA4S.Egg
{
    public class EggButtonsBox : MonoBehaviour
    {
        public Transform anturaOut;

        public AnimationCurve normalAnimationCurve;
        public AnimationCurve anturaInAnimationCurve;

        GameObject eggButtonPrefab;

        List<EggButton> eggButtons = new List<EggButton>();

        float buttonDistance = 20f;

        int buttonCount;

        IAudioManager audioManager;

        Action<ILivingLetterData> buttonsCallback;

        public void Initialize(GameObject eggButtonPrefab, IAudioManager audioManager, Action<ILivingLetterData> buttonsCallback)
        {
            this.eggButtonPrefab = eggButtonPrefab;
            this.audioManager = audioManager;
            this.buttonsCallback = buttonsCallback;
        }

        public void AddButton(ILivingLetterData letterData)
        {
            EggButton eggButton = CreateButton();

            eggButton.SetAnswer(letterData);

            eggButtons.Add(eggButton);
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
            eggButton.gameObject.SetActive(false);
            eggButton.Initialize(audioManager, buttonsCallback);
            eggButton.DisableInput();
            return eggButton;
        }

        public void ShowButtons()
        {
            for (int i = 0; i < eggButtons.Count; i++)
            {
                eggButtons[i].gameObject.SetActive(true);
            }
        }

        public void HideButtons()
        {
            for (int i = 0; i < eggButtons.Count; i++)
            {
                eggButtons[i].gameObject.SetActive(false);
            }
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
                int index = UnityEngine.Random.Range(0, buttonsIndex.Count);
                int currentIndex = buttonsIndex[index];
                buttonsIndex.RemoveAt(index);

                eggButtons[currentIndex].transform.localPosition = buttonsPosition[i];
                eggButtons[currentIndex].positionIndex = i;
            }
        }

        public void AnturaButtonOut(Action endCallback, float duration, float delay)
        {
            List<EggButton> buttons = GetButtons(true);

            float delayBetweenButton = 0.1f;

            for (int i = 0; i < buttons.Count; i++)
            {
                if (i == buttons.Count - 1)
                {
                    buttons[i].MoveTo(anturaOut.localPosition, duration, normalAnimationCurve, (i * delayBetweenButton) + delay, endCallback);
                }
                else
                {
                    buttons[i].MoveTo(anturaOut.localPosition, duration, normalAnimationCurve, (i * delayBetweenButton) + delay);
                }

                buttons[i].ScaleTo(0f, duration, (i * delayBetweenButton) + delay);
            }
        }

        public void AnturaButtonIn(Action endCallback, float duration, float delay)
        {
            float delayBetweenButton = 0.5f;

            buttonCount = eggButtons.Count;

            Vector3[] buttonsPosition = CalculateButtonPositions();

            List<int> buttonsIndex = new List<int>();

            for (int i = 0; i < buttonCount; i++)
            {
                buttonsIndex.Add(i);
            }

            for (int i = 0; i < buttonsPosition.Length; i++)
            {
                int index = UnityEngine.Random.Range(0, buttonsIndex.Count);
                int currentIndex = buttonsIndex[index];
                buttonsIndex.RemoveAt(index);

                if (i == buttonsPosition.Length - 1)
                {
                    eggButtons[currentIndex].MoveTo(buttonsPosition[i], duration, anturaInAnimationCurve, (i * delayBetweenButton) + delay, endCallback);
                }
                else
                {
                    eggButtons[currentIndex].MoveTo(buttonsPosition[i], duration, anturaInAnimationCurve, (i * delayBetweenButton) + delay);
                }

                eggButtons[currentIndex].positionIndex = i;
                eggButtons[currentIndex].ScaleTo(1f, duration, (i * delayBetweenButton) + delay);
                eggButtons[currentIndex].transform.SetAsFirstSibling();
            }
        }

        Vector3[] CalculateButtonPositions()
        {
            Vector3[] buttonsPosition = new Vector3[buttonCount];

            Vector2 eggSizeDelta = ((RectTransform)eggButtons[0].transform).sizeDelta;

            Vector3 currentPosition = Vector3.zero;

            float positionUp = (eggSizeDelta.y + buttonDistance) / 2f;
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
                float currentHorizontal = (((size + buttonDistance) * (number - 1)) / 2f);

                for (int i = 0; i < number; i++)
                {
                    if (i != 0)
                    {
                        currentHorizontal -= size + buttonDistance;
                    }

                    horizontalPositions[i] = currentHorizontal;
                }
            }

            return horizontalPositions;
        }

        public List<EggButton> GetButtons(bool inPositionOrder)
        {
            if (inPositionOrder)
            {
                List<EggButton> buttons = new List<EggButton>();

                while (buttons.Count < eggButtons.Count)
                {
                    EggButton eB = null;

                    for (int i = 0; i < eggButtons.Count; i++)
                    {
                        if (eggButtons[i].positionIndex == buttons.Count)
                        {
                            eB = eggButtons[i];
                            break;
                        }
                    }

                    buttons.Add(eB);
                }

                return buttons;
            }
            else
            {
                return eggButtons;
            }
        }

        public void EnableButtonsInput()
        {
            for (int i = 0; i < eggButtons.Count; i++)
            {
                eggButtons[i].EnableInput();
            }
        }

        public void DisableButtonsInput()
        {
            for (int i = 0; i < eggButtons.Count; i++)
            {
                eggButtons[i].DisableInput();
            }
        }

        public void PlayButtonsAudio(bool lightUp, bool inPositionOrder, float delay, Action endCallback)
        {
            List<EggButton> buttons = GetButtons(inPositionOrder);

            Action callback = null;

            for (int i = 0; i < buttons.Count; i++)
            {
                if (i == buttons.Count - 1)
                {
                    callback = endCallback;
                }

                delay += buttons[i].PlayButtonAudio(lightUp, delay, callback);
            }
        }
    }
}