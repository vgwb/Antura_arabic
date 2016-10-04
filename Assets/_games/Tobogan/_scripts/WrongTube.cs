using UnityEngine;
using System.Collections.Generic;

public class WrongTube : MonoBehaviour
{
    List<System.Action> dropRequest = new List<System.Action>();

    public Transform spawnTransform;
    public GameObject letterPrefab;

    public bool doSpawn = false;

    bool dropping = false;

    void Start()
    {

    }

    void Update()
    {
        if (!dropping && (doSpawn || dropRequest.Count > 0))
        {
            doSpawn = false;
            dropping = true;

            System.Action toCall = null;

            if (dropRequest.Count > 0)
            {
                toCall = dropRequest[dropRequest.Count - 1];
                dropRequest.RemoveAt(dropRequest.Count - 1);
            }

            var letterGo = Instantiate(letterPrefab);
            letterGo.SetActive(true);

            letterGo.transform.position = spawnTransform.position;
            letterGo.transform.forward = spawnTransform.right;

            letterGo.transform.RotateAround(letterGo.transform.position + letterGo.transform.up * 3.5f, letterGo.transform.forward, Random.value * 360);

            var ragdoll = letterGo.GetComponent<LivingLetterRagdoll>();
            
            ragdoll.SetRagdoll(true, spawnTransform.forward * 75);
            ragdoll.onPoofed += () => {
                if (toCall != null)
                    toCall();

                dropping = false;
            };

        }
    }

    public void DropLetter(System.Action callback)
    {
        dropRequest.Add(callback);
    }
}
