using UnityEngine;
using System.Collections;

namespace EA4S.ThrowBalls
{
    public class CrateController : MonoBehaviour
    {
        public Vector2 velocity;

        public LetterController letterController;

        private bool isBouncing = false;

        public Collider collider;

        // Use this for initialization
        void Start()
        {
            velocity = new Vector2(0, 0);
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 position = transform.position;
            Vector2 deltaPosition = velocity * Time.deltaTime;
            position.x += deltaPosition.x;
            position.y += deltaPosition.y;
            transform.position = position;

            if (!isBouncing)
            {
                letterController.MoveBy(deltaPosition.x, deltaPosition.y, 0); 
            }

            Vector3 viewportPoint = Camera.main.WorldToViewportPoint(transform.position);

            if ((viewportPoint.x < 0 && !IsMovingRight()) || (viewportPoint.x > 1 && IsMovingRight()))
            {
                velocity.x *= -1;
            }

            if ((viewportPoint.y < 0 && !IsMovingUp()) || (viewportPoint.y > 1 && IsMovingUp()))
            {
                velocity.y *= -1;
            }
        }

        public void SetIdle()
        {
            velocity.x = 0;
            velocity.y = 0;
        }

        public void SetMoving(bool isMovingRight)
        {
            velocity.x = Random.Range(4, 10);

            if (!isMovingRight)
            {
                velocity.x *= -1;
            }
        }

        private bool IsMovingRight()
        {
            return velocity.x >= 0;
        }

        private bool IsMovingUp()
        {
            return velocity.y >= 0;
        }

        public void Reset()
        {
            transform.Rotate(0, Random.Range(-20, 20), 0);
            GameObject letter = letterController.gameObject;
            transform.position = new Vector3(letter.transform.position.x, letter.transform.position.y - 2.1f, letter.transform.position.z);
            isBouncing = false;
            collider.enabled = true;
            SetIdle();
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == Constants.TAG_POKEBALL)
            {
                Vector3 pokeballVelocity = PokeballController.instance.GetVelocity();

                /*velocity.x = pokeballVelocity.x > 50 ? 50 : pokeballVelocity.x;
                velocity.y = pokeballVelocity.y > 50 ? 50 : pokeballVelocity.y;*/

                pokeballVelocity.Normalize();

                velocity.x = pokeballVelocity.x > 0 ? 40 * pokeballVelocity.x : -40 * pokeballVelocity.y;
                velocity.y = pokeballVelocity.y > 0 ? 40 * pokeballVelocity.x : -40 * pokeballVelocity.y;

                letterController.SetIsDropping();
                isBouncing = true;

                collider.enabled = false;

                StartCoroutine(VanishAfterDelay(1.5f));
            }
        }

        private IEnumerator VanishAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            GameObject poof = (GameObject)Instantiate(ThrowBallsGameManager.Instance.poofPrefab, transform.position, Quaternion.identity);
            Destroy(poof, 10);
            gameObject.SetActive(false);
        }

        public void Enable()
        {
            Reset();
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}