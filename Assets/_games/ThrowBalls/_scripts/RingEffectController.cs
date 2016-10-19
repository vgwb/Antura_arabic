using UnityEngine;
using System.Collections;

namespace EA4S.ThrowBalls
{
    public class RingEffectController : MonoBehaviour
    {
        // Important: Don't modify these two variables. Set the period using the SetPeriod method.
        private float period;
        private float frequency;

        private float startScale = 0.2f;
        private float endScale = 1f;
        private float animationStartTime;
        private float fadeOutBegin = 0.6f;
        private bool isFadingOut = false;
        private float startAlpha = 222;
        private float endAlpha = 0;

        public SpriteRenderer spriteRenderer;

        void Start()
        {
            SetPeriod(0.33f);
        }

        void Update()
        {
            if (animationStartTime > 0)
            {
                float t = Mathf.Sin(2 * Mathf.PI * frequency * (Time.time - animationStartTime) * 0.25f);
                transform.localScale = new Vector3(Mathf.Lerp(startScale, endScale, t), Mathf.Lerp(startScale, endScale, t), 1);

                if (Time.time - animationStartTime > fadeOutBegin * period && !isFadingOut)
                {
                    FadeOut();
                }
            }
        }

        public void Animate(PokeballController.ChargeStrength chargeStrength)
        {
            animationStartTime = Time.time;

            switch (chargeStrength)
            {
                case PokeballController.ChargeStrength.None:
                    endScale *= 0;
                    break;
                case PokeballController.ChargeStrength.Low:
                    endScale *= 1;
                    break;
                case PokeballController.ChargeStrength.Medium:
                    endScale *= 2;
                    break;
                case PokeballController.ChargeStrength.High:
                    endScale *= 3;
                    break;
                default:
                    break;
            }
        }

        void SetPeriod(float period)
        {
            this.period = period;
            frequency = 1 / period;
        }

        private void FadeOut()
        {
            StartCoroutine(FadeOutCoroutine());
            isFadingOut = true;
        }

        private IEnumerator FadeOutCoroutine()
        {
            float timeLeft = (1 - fadeOutBegin) * period;
            float alphaRange = endAlpha - startAlpha;
            float increment = ((Time.fixedDeltaTime) / timeLeft) * alphaRange;

            increment /= 255;

            while (true)
            {
                float newAlpha = spriteRenderer.color.a + increment;

                newAlpha = Mathf.Clamp(newAlpha, 0, 1);

                spriteRenderer.color = new Color(255, 255, 255, newAlpha);

                if (newAlpha <= 0)
                {
                    break;
                }
                
                yield return new WaitForFixedUpdate();
            }

            Destroy(gameObject);
        }
    }
}

