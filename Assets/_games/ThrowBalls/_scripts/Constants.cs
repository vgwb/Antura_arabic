using UnityEngine;
using System.Collections;

public class Constants
{
    /*public static readonly Vector3[] LETTER_POSITIONS = new Vector3[] { new Vector3(-2.7f, 7.05f, -1),
        new Vector3(-14.6f, 13.3f, -5), new Vector3(9.1f, 12, -15), new Vector3(0.52f, 11f, 10f),
        new Vector3(-28.2f, -2.8f, 11.3f), new Vector3(-14.8f, 3.3f, 11f)};*/

    /*public static readonly Vector3[] LETTER_POSITIONS = new Vector3[] { new Vector3(-6.8f, 4.41f, -19.7f),
        new Vector3(-7.68f, 7.52f, -15), new Vector3(0f, 13.2f, -6.75f)};*/

    /*public static readonly Vector3[] LETTER_POSITIONS = new Vector3[] { new Vector3(9.89f, 5.78f, -10.01f),
        new Vector3(-16.05f, 9.63f, -6.5f), new Vector3(9.8f, 14.12f, -2.13f)};*/

    /*public static readonly Vector3[] LETTER_POSITIONS = new Vector3[] { new Vector3(-6.72f, 6.75f -3.5f, -12f + 4.5f),
        new Vector3(12.1f, 10.7f -3.5f, 1.5f + 4.5f), new Vector3(0f, 16.7f -3.5f, 20.34f)};*/

    public static readonly Vector3[] LETTER_POSITIONS = new Vector3[] { new Vector3(-6.9025f, 0.56f, -5.274f),
        new Vector3(12.386f, 0.51f, 5.688f), new Vector3(0f, 0.51f, 20.34f)};

    public const string TAG_CORRECT_LETTER = "CorrectLetter";
    public const string TAG_WRONG_LETTER = "WrongLetter";

    public const string TAG_POKEBALL = "Pokeball";
    public const string TAG_RAIL = "Rail";

    public static readonly Vector3 GRAVITY = new Vector3(0, -150, 0);
}