using UnityEngine;
using System.Collections;


public class MiniGame : MonoBehaviour {

    public string nameMiniGame;
    public float weight;
    public bool forced;
    public int order;
    public int playSession;
	
    public MiniGame(string nameM, float weightM, bool forcedM, int orderM)
    {
        nameMiniGame = nameM;
        weight = weightM;
        forced = forcedM;
        order = orderM;
    }
}
