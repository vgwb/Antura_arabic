using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class TestDB : MonoBehaviour
{

    public Text DebugText;

    void Start()
    {
        var ds = new DBService("EA4S_Database.sqlite");

        var minigames = ds.GetMinigames();
        foreach (var minigame in minigames) {
            ToConsole(minigame.ToString());
        }

        var playsessions = ds.GetPlaySessions(1);
        foreach (var playsession in playsessions) {
            ToConsole(playsession.ToString());
        }

        //people = ds.GetPersonsNamedRoberto();
        //ToConsole("Searching for Roberto ...");
        //ToConsole(people);

        //ds.CreatePerson();
        //ToConsole("New person has been created");
        //var p = ds.GetJohnny();
        //ToConsole(p.ToString());

    }

    private void ToConsole(string msg)
    {
        DebugText.text += System.Environment.NewLine + msg;
        Debug.Log(msg);
    }

}
