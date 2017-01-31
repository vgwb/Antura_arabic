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
        public MeshRenderer Body;
        public GameObject poofPrefab;

        public SpriteRenderer LetterSlot;
        public SpriteRenderer WordSlot;
        public SpriteRenderer PhraseSlot;

        /// <summary>
        /// State-holders
        /// </summary>
        private float wideness;
        private bool showBox;

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
                wideness = 1.0f;
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
                    wideness = 1.0f;
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
                    wideness = 1.3f;
                    break;
                case LivingLetterDataType.Phrase:
                    wideness = 3.5f;
                    break;
                default:
                    wideness = 1f;
                    break;
            }
        }

        private void DisableSlots()
        {
            LetterSlot.enabled = false;
            WordSlot.enabled = false;
            PhraseSlot.enabled = false;
        }

        private void SetSlot( LivingLetterDataType dataType)
        {
            switch (dataType)
            {
                case LivingLetterDataType.Word:
                    WordSlot.enabled = true;
                    break;
                case LivingLetterDataType.Phrase:
                    PhraseSlot.enabled = true;
                    break;
                default:
                    LetterSlot.enabled = true;
                    break;
            }
        }

        /// <summary>
        /// Return size of LL, usefull for determining layout offsets
        /// </summary>
        /// <returns></returns>
        public float GetSize()
        {
            return wideness;
        }

        /// <summary>
        /// Initializes  object with the specified data.
        /// </summary>
        /// <param name="_data">The data.</param>
        public void Init( ILivingLetterData _data, bool showBox)
        {
            this.showBox = showBox;
            Data = _data;
        }

        /// <summary>
        /// Initializes  object with the specified data.
        /// </summary>
        /// <param name="_data">The data.</param>
        public void InitAsSlot( LivingLetterDataType dataType)
        {
            showBox = false;
            Data = null;
            SetScale( dataType);
        }

    }
}
