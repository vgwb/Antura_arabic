using EA4S.Core;
using EA4S.MinigamesAPI;

namespace EA4S.Database
{
    /// <summary>
    /// Defines a type of data that can be converted to a LivingLetterData
    /// This is required as IQuestionPack uses LivingLetterData, but the data engine works on IData
    /// </summary>
    public interface IConvertibleToLivingLetterData
    {
        string GetId();
        ILivingLetterData ConvertToLivingLetterData();
    }
}