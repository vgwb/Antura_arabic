using UnityEngine;

namespace EA4S
{
    public interface ILivesWidget
    {
        void Show();
        void Hide();

        void SetLives(int score);
        
    }
}
