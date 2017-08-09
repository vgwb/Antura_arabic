using Antura.Core;
using Antura.LivingLetters;
using UnityEngine;
using UnityEngine.UI;

namespace Antura.UI
{
    /// <summary>
    /// A container for multiple contents that should be shown based on the reading direction.
    /// </summary>
    public class ReadingDirectionGrid : MonoBehaviour
    {
        private GridLayoutGroup gridLayoutGroup;

        void Awake()
        {
            gridLayoutGroup = GetComponent<GridLayoutGroup>();
            switch (AppConstants.ReadingDirection)
            {
                case ReadingDirection.LeftToRight:
                    switch (gridLayoutGroup.startCorner)
                    {
                        case GridLayoutGroup.Corner.LowerRight:
                            gridLayoutGroup.startCorner = GridLayoutGroup.Corner.LowerLeft;
                            break;
                         case GridLayoutGroup.Corner.UpperRight:
                            gridLayoutGroup.startCorner = GridLayoutGroup.Corner.UpperLeft;
                            break;
                    }
                    break;
                case ReadingDirection.RightToLeft:
                    switch (gridLayoutGroup.startCorner)
                    {
                        case GridLayoutGroup.Corner.LowerLeft:
                            gridLayoutGroup.startCorner = GridLayoutGroup.Corner.LowerRight;
                            break;
                        case GridLayoutGroup.Corner.UpperLeft:
                            gridLayoutGroup.startCorner = GridLayoutGroup.Corner.UpperRight;
                            break;
                    }
                    break;
            }
        }
    }
}