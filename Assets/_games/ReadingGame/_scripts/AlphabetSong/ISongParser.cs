using System.Collections.Generic;
using System.IO;

namespace EA4S.Minigames.ReadingGame
{
    public interface ISongParser
    {
        List<KaraokeSegment> Parse(Stream stream);
    }
}