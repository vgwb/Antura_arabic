using EA4S;

/// <summary>
/// Defines a type of data that can be converted to a LivingLetterData
/// This is required as IQuestionPack uses LivingLetterData, but the data engine works on IData
/// </summary>
// refactor: add to a namespace
public interface IConvertibleToLivingLetterData
{
    string GetId();
    ILivingLetterData ConvertToLivingLetterData();
}
