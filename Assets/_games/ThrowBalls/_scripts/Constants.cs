using UnityEngine;
using System.Collections;

public class Constants
{
    /*public static readonly Vector3[] LETTER_POSITIONS = new Vector3[] { new Vector3(-2.7f, 7.05f, -1),
        new Vector3(-14.6f, 13.3f, -5), new Vector3(9.1f, 12, -15), new Vector3(0.52f, 11f, 10f),
        new Vector3(-28.2f, -2.8f, 11.3f), new Vector3(-14.8f, 3.3f, 11f)};*/

    /*public static readonly Vector3[] LETTER_POSITIONS = new Vector3[] { new Vector3(-6.8f, 4.41f, -19.7f),
        new Vector3(-7.68f, 7.52f, -15), new Vector3(0f, 13.2f, -6.75f)};*/

    public static readonly Vector3[] LETTER_POSITIONS = new Vector3[] { new Vector3(9.89f, 5.78f, -10.01f),
        new Vector3(-16.05f, 9.63f, -6.5f), new Vector3(9.8f, 14.12f, -2.13f)};

    public const string TAG_CORRECT_LETTER = "CorrectLetter";
    public const string TAG_WRONG_LETTER = "WrongLetter";

    public const string TAG_POKEBALL = "Pokeball";
    public const string TAG_RAIL = "Rail";

    public static readonly Vector3 GRAVITY = new Vector3(0, -147, 0);
}