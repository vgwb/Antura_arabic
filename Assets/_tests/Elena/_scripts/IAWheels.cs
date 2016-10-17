using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class IAWheels : MonoBehaviour {

    List<MiniGame> miniGames = new List<MiniGame>();
    List<MiniGame> forcedMiniGames = new List<MiniGame>();
    List<MiniGame> heightMiniGames = new List<MiniGame>();
    List<MiniGame> finalGames = new List<MiniGame>();
    int sequence;

	void Start () {

        sequence = 4;

        miniGames.Add(new MiniGame("pepito", 0.2f, false,0));
        miniGames.Add(new MiniGame("pepito2", 0.5f, false,0));
        miniGames.Add(new MiniGame("pepito3", 0.1f, true,2));
        miniGames.Add(new MiniGame("pepito4", 0.7f, true,1));

        ChooseMiniGames();
    }

    void ChooseMiniGames()
    {
        OrderMiniGames();
        ListMiniGamesinOrder();

        for (int i=0; i < sequence; i++)
        {
            Debug.Log(finalGames[i].nameMiniGame);

        }
    }
    void OrderMiniGames()
    {
        for (int l = 0; l < miniGames.Count; l++)
        {
            if (miniGames[l].forced == true) ListForcedGames(l);
            else ListGamesbyHeight(l);
        }
    }
    void ListForcedGames(int l)
    {
        forcedMiniGames.Add(miniGames[l]);
        forcedMiniGames = forcedMiniGames.OrderByDescending(go => go.order).ToList();     
    }
    void ListGamesbyHeight(int l)
    {
        heightMiniGames.Add(miniGames[l]);
        heightMiniGames = heightMiniGames.OrderByDescending(go => go.height).ToList();
    }
    void ListMiniGamesinOrder()
    {
        for (int n = 0; n < forcedMiniGames.Count; n++)
        {
            finalGames.Add(forcedMiniGames[n]);
        }
        for (int n = 0; n < heightMiniGames.Count; n++)
        {
            finalGames.Add(heightMiniGames[n]);
        }
    }
}
