using System;
using UnityEngine;

/// <summary>
/// Evenly divides the difficulty range to arrange parameters base with difficulty
/// Note that this may not be desired if difficulty is pre-determined to  have
/// only few discrete values!
/// 
/// Increase( 1, 5)
/// 
/// 0                                                                        1
/// difficulty---------------------------------------------------------------->
///            1    |         2        |        3         |        4         |   (no offset)
///   1    |         2        |         3         |        4         |        5  (offset)
/// 1                2                  3                  4                  5
///
/// Using some offsett we can get maximum value even if difficulty is slightly less than 1
/// 
/// </summary>
public class DifficultyRegulation : MonoBehaviour {

    float difficulty;
    float startingFrom;

    public DifficultyRegulation( float diff)
    {
        difficulty = diff;
        difficulty *= 1.33f;
        difficulty -= 0.32f; // difficulty is never setted to 0. however
                                  // configuration is based on assumption the whole 
                                  // difficulty range is used.

        difficulty = Mathf.Clamp01( difficulty);
    }

    public void SetStartingFrom( float from)
    {
        startingFrom = from;
    }

	public int Increase( int min, int max)
    {
        if (min > max)
            throw new ArgumentException( "This parameter should only increase.");

        float finalVal = Mathf.Clamp01(difficulty - startingFrom);
        return (int)Mathf.RoundToInt( Mathf.Lerp( min, max, finalVal));
    }

    public int Decrease( int max, int min)
    {
        if (min > max)
            throw new ArgumentException( "This parameter should only decrease.");

        float finalVal = Mathf.Clamp01(difficulty - startingFrom);
        return (int)Mathf.RoundToInt( Mathf.Lerp( max, min, finalVal));
    }
}
