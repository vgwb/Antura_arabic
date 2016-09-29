using UnityEngine;
using System.Collections.Generic;

namespace EA4S.Tobogan
{
    public class LettersTower : MonoBehaviour
    {
        // The tower is a stack of letters, that could be crash
        private List<LivingLetter> stackedLetters = new List<LivingLetter>();

        float letterHeight = 4.75f;
        public bool isCrashing = false;
        public float TowerFullHeight { get { return stackedLetters.Count * letterHeight; } }
        public GameObject letterPrefab;

        // Used to manage the backlog in the tube and the falling letter
        public Queue<LivingLetter> backlogTube = new Queue<LivingLetter>();
        public LivingLetter fallingLetter;

        public bool testAddLetter = false;

        // Used to simulate tower bounciness
        public bool doBounce = false;
        public float Elasticity = 100.0f;
        float currentHeight;
        float yVelocity = 0.0f;
        float lastCompressionValue = 1;

        // Used to simulate tower swing
        public float swingAmountFactor = 8.0f;
        public float swingSpeed = 0.15f;
        float currentSwing = 0.0f;
        float swingFreightenedSpeedFactor = 1.0f;
        float swingPercentage = 0; // a swing starts from center, go left, go right and comes back to center

        // Used to calculate the right moment in which a letter should be dropped
        public Transform fallingSpawnPosition;
        float fallingSpawnHeight;
        // I use here time, instead of integrating over timesteps to be sure that the fall takes that time
        float fallingTime = 0.0f;
        float remainingFallingTime = 0.0f;
        float letterInitialFallSpeed = -0.0f;
        float spawnTimer = 0;

        /// <summary>
        /// Crash the tower!
        /// </summary>
        public void Crash()
        {
            isCrashing = true;
        }

        /// <summary>
        /// Add a new letter to falling queue, it will be released when is the right time
        /// </summary>
        public void AddLetter()
        {
            var newLetter = GameObject.Instantiate(letterPrefab);
            newLetter.SetActive(false);

            backlogTube.Enqueue(newLetter.GetComponent<LivingLetter>());
        }

        void Start()
        {
            currentHeight = TowerFullHeight;

            // Test
            for (int i = 0; i < 15; ++i)
                AddLetter();
        }

        void Update()
        {
            fallingSpawnHeight = (fallingSpawnPosition.position - transform.position).y;

            if (fallingLetter == null)
            {
                // Manage backlog
                UpdateBacklog();
            }

            if (fallingLetter != null)
            {
                // Update current falling letter
                UpdateFallingLetter(fallingLetter);
            }

            UpdateTowerMovements();

            if (testAddLetter)
            {
                testAddLetter = false;
                AddLetter();
            }
        }


        void UpdateTowerMovements()
        {
            float normalHeight = TowerFullHeight;

            //// Simulate elastic movement
            float elasticForce = ComputeElasticForce();

            // F = m * a; since all letters are equal, we approximate letter mass to 1 and work on Elasticity to model the graphics feedback
            // F -> 1 * a
            float yAcceleration = elasticForce + Physics.gravity.y;

            if (stackedLetters.Count < 2)
            {
                yVelocity = 0;
                currentHeight = TowerFullHeight;
            }
            else
            {
                // V = a * t
                yVelocity += yAcceleration * Time.deltaTime;

                // Integrates changes between last timestep
                // h += V * t + 0.5 * a * t^2
                currentHeight += yVelocity * Time.deltaTime + 0.5f * yAcceleration * Time.deltaTime * Time.deltaTime;

                // Put a maximum of height so letters cannot be detached
                if (currentHeight > normalHeight + letterHeight * 0.2f)
                {
                    currentHeight = normalHeight + letterHeight * 0.2f;

                    //yVelocity = -yVelocity*0.8f;// elastic bounce
                    yVelocity = Mathf.Min(yVelocity, 0.1f); // just a small jump
                }
                else if (currentHeight < letterHeight)
                    currentHeight = letterHeight;
            }

            //// Simulate a bit of horizontal swinging
            float swingFrequency = swingSpeed * swingFreightenedSpeedFactor;

            swingPercentage += Time.deltaTime * swingFrequency;
            currentSwing = Mathf.Sin(swingPercentage * 2 * Mathf.PI);

            // Update letters positions
            if (currentHeight == 0)
                lastCompressionValue = 1;
            else
                lastCompressionValue = currentHeight / normalHeight;
            for (int i = 0, count = stackedLetters.Count; i < count; ++i)
            {
                float heightSwingFactor = i / 30.0f;
                heightSwingFactor = heightSwingFactor * heightSwingFactor;

                stackedLetters[i].transform.position = transform.position + Vector3.up * (i * letterHeight * lastCompressionValue) + transform.right * currentSwing * swingAmountFactor * heightSwingFactor;

                if (i > 0)
                    stackedLetters[i].transform.up = (stackedLetters[i].transform.position - stackedLetters[i - 1].transform.position).normalized;
            }

            // for testing purposed
            if (doBounce)
            {
                doBounce = false;
                yVelocity += -20f;
            }


            if (isCrashing)
            {
                swingFreightenedSpeedFactor = Mathf.Lerp(swingFreightenedSpeedFactor, 2.0f, 3.0f * Time.deltaTime);

                // wait for a good swing
                if (Mathf.Abs(currentSwing) >= 0.8f)
                {
                    isCrashing = false;

                    for (int i = 0, count = stackedLetters.Count; i < count; ++i)
                    {
                        var randomVelocity = Random.insideUnitSphere * 10.0f;
                        randomVelocity.y = Mathf.Abs(randomVelocity.y);

                        //randomVelocity.y = Mathf.Min(Mathf.Abs(randomVelocity.y), 5);

                        randomVelocity += transform.right * Mathf.Sign(currentSwing) * i;

                        stackedLetters[i].GetComponent<LivingLetterRagdoll>().SetRagdoll(true, randomVelocity);
                    }
                    stackedLetters.Clear();
                }
            }
            else
                swingFreightenedSpeedFactor = 1.0f;
        }

        float ComputeElasticForce()
        {
            // "Normal" height of tower
            float normalHeight = TowerFullHeight;

            // dx
            float deltaHeight = currentHeight - normalHeight;

            // F = -K * dx
            float elasticForce = -Elasticity * deltaHeight;

            return elasticForce;
        }

        void UpdateFallingLetter(LivingLetter letter)
        {
            remainingFallingTime -= Time.deltaTime;

            float passedTime = fallingTime - remainingFallingTime;
            letter.transform.position = transform.position + Vector3.up * (
                letterInitialFallSpeed * passedTime +
                fallingSpawnHeight + 0.5f * Physics.gravity.y * passedTime * passedTime
                );

            bool toStack = false;
            // Manage attaching
            if (stackedLetters.Count == 0)
            {
                toStack = (remainingFallingTime <= 0);
            }
            else
            {
                float fullHeight = TowerFullHeight;
                float compressedHeight = lastCompressionValue * fullHeight;

                float currentFallHeight = letter.transform.position.y - transform.position.y - compressedHeight;
                float totalFallHeight = fallingSpawnHeight - compressedHeight;

                toStack = (currentFallHeight <= 0);
            }

            // Stack it
            if (toStack)
            {
                currentHeight += letterHeight;
                stackedLetters.Add(fallingLetter);
                fallingLetter = null;
                doBounce = true;
            }
        }

        void SpawnLetter(LivingLetter letter)
        {
            remainingFallingTime = fallingTime;
            letter.transform.position = transform.position + fallingSpawnHeight * Vector3.up;
            letter.gameObject.SetActive(true);
            fallingLetter = letter;
            spawnTimer = 0;
        }

        void UpdateBacklog()
        {
            if (backlogTube.Count == 0)
                return;

            // A letter was already scheduled to spawn
            if (spawnTimer > 0)
            {
                spawnTimer -= Time.deltaTime;

                if (spawnTimer <= 0)
                {
                    // Spawn!
                    var currentLetter = backlogTube.Dequeue();
                    SpawnLetter(currentLetter);
                }
            }
            else
            {
                // Must schedule another letter to spawn in the right moment,
                // that is, in order that, when it hits the tower, the swing position is at 0.
                // we can ignore the bounce movement since it's quite small to be approximated to 0

                // Some maths: {....please, do not say that "engineering skills are not needed to make games" no more}

                // The following is true: 
                // tFall + spawnTimer = tSwingToCenter + K*swingPeriod, K in N, K >= 0
                // ---> spawnTimer = tSwingToCenter - tFall + K*swingPeriod
                // ---> we'll select, in the end, the minimum K in order that spawnTimer >= 0

                // tFall = (- initialVelocity +/- sqrtf(initialVelocity^2 - 2 * g * fallHeight)) / g;

                // sin(theta) = 0 -> theta = 0 + K * PI
                // we'll select, in the end, K in order that tSwingToCenter >= 0 and minimum

                float approximatedFallHeight = fallingSpawnHeight - TowerFullHeight;
                float sqrtDelta = Mathf.Sqrt(letterInitialFallSpeed * letterInitialFallSpeed - 2 * Physics.gravity.y * approximatedFallHeight);

                fallingTime = (-letterInitialFallSpeed - sqrtDelta) / Physics.gravity.y;

                if (stackedLetters.Count < 3)
                {
                    spawnTimer = 0;
                }
                else
                {
                    float swingFrequency = swingSpeed * swingFreightenedSpeedFactor;

                    // it 
                    float tSwingToCenter;
                    if (swingFrequency == 0)
                        tSwingToCenter = 0;
                    else
                        tSwingToCenter = (Mathf.CeilToInt(swingPercentage / 0.5f) * 0.5f - swingPercentage) / swingFrequency;


                    float swingPeriod = swingFrequency == 0 ? 0 : 1 / swingFrequency;

                    spawnTimer = tSwingToCenter - fallingTime + swingPeriod * Mathf.Max(0, Mathf.CeilToInt((fallingTime - tSwingToCenter) / swingPeriod));
                }

                if (spawnTimer < 0.05f)
                {
                    // Spawn!
                    var currentLetter = backlogTube.Dequeue();
                    SpawnLetter(currentLetter);
                }
            }
        }
    }
}