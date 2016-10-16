using UnityEngine;
using System.Collections;


public class MiniGame : MonoBehaviour {

    public string nameMiniGame;
    public float height;
    public bool forced;
    public int order;
    public int playSession;
	
    public MiniGame(string nameM, float heightM, bool forcedM, int orderM)
    {
        nameMiniGame = nameM;
        height = heightM;
        forced = forcedM;
        order = orderM;
    }
}
