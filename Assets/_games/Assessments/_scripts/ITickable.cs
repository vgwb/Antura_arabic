namespace EA4S.Assessment
{
    internal interface ITickable
    {
        //returns true as long as it is updated.
        bool Update( float deltaTime);
    }
}