
namespace EA4S
{
    public interface IGameState
    {
        void EnterState();
        void ExitState();

        void Update(float delta);
        void UpdatePhysics(float delta);
    }
}