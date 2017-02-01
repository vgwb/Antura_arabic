using EA4S.Helpers;
using EA4S.MinigamesAPI;
using TMPro;
using UnityEngine;

namespace EA4S.Assessment
{
    /// <summary>
    /// Alternative LL, no animation, more lightweight,
    /// and with some Assessments specific functionalities
    /// </summary>
    public class StillLetterBox : MonoBehaviour
    {
        /// <summary>
        /// Injected properties
        /// </summary>
        public TMP_Text Label;
        public TextMeshPro Drawing;
        public GameObject poofPrefab;
        public Transform scaleSprite;

        public SpriteRenderer questionSprite;
        public SpriteRenderer answerSprite;
        public SpriteRenderer slotSprite;

        /// <summary>
        /// Gets the data.
        /// </summary>
        ILivingLetterData data;
        public ILivingLetterData Data
        {
            get
            {
                return data;
            }
            private set
            {
                data = value;

                OnModelChanged();
            }
        }

        private float Wideness
        {
            get
            {
                return scaleSprite.localScale.x;
            }
            set
            {
                scaleSprite.localScale = new Vector3( value, 1, 1);
                GetComponent<BoxCollider>().size = new Vector3( 3 * value, 3, 1);
            }
        }

        private void Start()
        {
            OnModelChanged();
        }

        /// <summary>
        /// Called when [model changed].
        /// </summary>
        private void OnModelChanged()
        {
            DisableSlots();
            if (Data == null)
            {
                Wideness = 1.0f;
                Drawing.enabled = false;
                Label.enabled = false;
            }
            else
            {
                if (Data.DataType == LivingLetterDataType.Image)
                {
                    Drawing.text = Data.DrawingCharForLivingLetter;
                    Drawing.enabled = true;

                    LL_ImageData data = ( LL_ImageData)Data;
                    if (data.Data.Category == Database.WordDataCategory.Color)
                    {
                        Drawing.color = GenericHelper.GetColorFromString( data.Data.Value);
                    }
                    else
                    {
                        Drawing.color = Color.black;
                    }

                    Label.enabled = false;
                    Wideness = 1.0f;
                }
                else
                {
                    Drawing.enabled = false;
                    Label.enabled = true;
                    Label.text = Data.TextForLivingLetter;

                    SetScale( data.DataType);
                }
            }
        }

        private void SetScale( LivingLetterDataType dataType)
        {
            switch (dataType)
            {
                case LivingLetterDataType.Word:
                    Wideness = 1.3f;
                    break;
                case LivingLetterDataType.Phrase:
                    Wideness = 3.5f;
                    break;
                default:
                    Wideness = 1f;
                    break;
            }
        }

        private void DisableSlots()
        {
            questionSprite.enabled = false;
            answerSprite.enabled = false;
            slotSprite.enabled = false;
        }

        /// <summary>
        /// Return size of LL, usefull for determining layout offsets
        /// </summary>
        /// <returns></returns>
        public float GetSize()
        {
            return Wideness;
        }

        /// <summary>
        /// Initializes  object with the specified data.
        /// </summary>
        /// <param name="_data">The data.</param>
        public void Init( ILivingLetterData _data, bool answer)
        {
            Data = _data;
            answerSprite.enabled = answer;
            questionSprite.enabled = !answer;
        }

        /// <summary>
        /// Initializes  object with the specified data.
        /// </summary>
        /// <param name="_data">The data.</param>
        public void InitAsSlot( LivingLetterDataType dataType)
        {
            Data = null;
            SetScale( dataType);
            slotSprite.enabled = true;
        }
    }
}
