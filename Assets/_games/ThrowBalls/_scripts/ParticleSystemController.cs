using UnityEngine;
using System.Collections;

namespace EA4S.ThrowBalls
{
    public class ParticleSystemController : MonoBehaviour
    {
        public static ParticleSystemController instance;
        new public ParticleSystem particleSystem;
        ParticleSystem.EmissionModule emissionModule;

        void Awake()
        {
            instance = this;
            emissionModule = particleSystem.emission;
        }

        void Start()
        {

        }

        void Update()
        {

        }

        public void OnPositionUpdate(Vector3 position)
        {
            position.y -= 1f;
            transform.position = position;
        }

        public void OnChargeStrengthUpdate(PokeballController.ChargeStrength chargeStrength)
        {
            switch (chargeStrength)
            {
                case PokeballController.ChargeStrength.None:
                    emissionModule.rate = 0;
                    break;
                case PokeballController.ChargeStrength.Low:
                    emissionModule.rate = 10;
                    particleSystem.gravityModifier = 0.5f;
                    particleSystem.startColor = Color.green;
                    break;
                case PokeballController.ChargeStrength.Medium:
                    emissionModule.rate = 20;
                    particleSystem.gravityModifier = 1f;
                    particleSystem.startColor = Color.yellow;
                    break;
                case PokeballController.ChargeStrength.High:
                    emissionModule.rate = 30;
                    particleSystem.gravityModifier = 1.5f;
                    particleSystem.startColor = Color.red;
                    break;
                default:
                    break;
            }
        }
    }
}