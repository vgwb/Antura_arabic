using EA4S.Helpers;
using EA4S.MinigamesAPI;
using DG.Tweening;
using EA4S.MinigamesCommon;
using System;
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

        public SpriteRenderer questionSprite;
        public SpriteRenderer answerSprite;

        ///################# ANIMATIONS #################
        
        // Local Tween
        Tween tween = null;

        /// <summary>
        /// Flip the LetterBox updside down to reveal the letter
        /// </summary>
        internal void RevealHiddenQuestion()
        {
            KillTween();
            answerSprite.enabled = true;

            answerSprite.DOColor( new Color32( 61, 185, 30, 255), 0.5f);
            questionSprite.DOFade( 0, 1);
            Label.alpha = 0;
            Label.DOFade( 1, 0.6f);
        }


        /// <summary>
        /// Hides the letter
        /// </summary>
        internal void HideHiddenQuestion()
        {
            Label.alpha = 0;
            answerSprite.color = new Color( 1, 1, 1, 0);
            InstaShrink();
        }

        /// <summary>
        /// Magnify animation.
        /// </summary>
        internal void Magnify()
        {
            TweenScale( 1);
        }

        internal void InstaShrink()
        {
            Scale = 0;
        }

        internal void Grabbed()
        {
            Scale = 1.3f;
        }

        internal void Dropped()
        {
            Scale = 1;
        }

        internal void TweenScale( float newScale)
        {
            KillTween();

            tween =
                DOTween.To( () => Scale, x => Scale = x, newScale, 0.4f);
        }

        private void KillTween()
        {
            if (tween != null)
                tween.Kill( true);

            tween = null;
        }


        ///############### IMPLEMENTATION ################

        public SpriteRenderer slotSprite;

        /// <summary>
        /// Gets the data.
        /// </summary>
        ILivingLetterData data = null; // NOT SET ALWAYS. DEBUGGIN
        bool nullOnDemand = false;
        public ILivingLetterData Data
        {
            get
            {
                if (data == null && ! nullOnDemand)
                    throw new ArgumentNullException( "Null on demand: " + nullOnDemand);
                return data;
            }
            private set
            {
                data = value;

                OnModelChanged();
            }
        }


        private float Wideness { get; set; }

        private float Scale
        {
            get
            {
                return transform.localScale.x;
            }
            set
            {
                float effectiveScale = value * Wideness;
                transform.localScale = new Vector3( effectiveScale, value, 1);
                GetComponent< BoxCollider>().size =
                    transform.localScale * 3;
            }
        }

        /// <summary>
        /// Called when [model changed].
        /// </summary>
        private void OnModelChanged()
        {
            DisableSlots();
            if (data == null)
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

                    SetWidness( data.DataType);
                }
            }
        }

        private void SetWidness( LivingLetterDataType dataType)
        {
            Wideness = ElementsSize.Get( dataType);
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
            if (_data == null)
                throw new ArgumentNullException( "Cannot init with null data");

            nullOnDemand = false;

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
            nullOnDemand = true;
            Data = null;
            SetWidness( dataType);
            slotSprite.enabled = true;
        }

        public void Poof()
        {
            var puffGo = GameObject.Instantiate( poofPrefab);
            puffGo.AddComponent< AutoDestroy>().duration = 2;
            puffGo.SetActive( true);
            puffGo.transform.localScale *= 0.75f;
        }
    }
}
