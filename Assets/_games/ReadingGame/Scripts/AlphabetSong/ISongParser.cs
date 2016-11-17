using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EA4S
{
    public interface ISongParser
    {
        List<KaraokeSegment> Parse(Stream stream);
    }
}