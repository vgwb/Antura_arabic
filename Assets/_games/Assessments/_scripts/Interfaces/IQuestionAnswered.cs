namespace EA4S.Assessment
{
    public interface IQuestionDecoration
    {
        void TriggerOnAnswered();

        float TimeToWait();

        void TriggerOnSpawned();
    }
}
