using UnityEngine;
using System.Collections;
using CGL.Antura;

public class TestAPI : MonoBehaviour {

	// Use this for initialization
	void Start () {
        AnturaGameplayInfo gameplayInfo = new AnturaGameplayInfo() { };
        AnturaGameManager.Instance.Modules.GameplayModule.GameplayStart(gameplayInfo);
        AnturaGameManager.Instance.Modules.GameplayModule.GameplayResult(new AnturaGameplayResult() { GameplayInfo = gameplayInfo });
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
