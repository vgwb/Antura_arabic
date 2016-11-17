using System.Collections.Generic;
using UnityEngine;

namespace EA4S
{
    public class KaraokeSong
    {
        public readonly List<KaraokeSegment> lines = new List<KaraokeSegment>();

        public KaraokeSong(IEnumerable<KaraokeSegment> segments)
        {
            this.lines.AddRange(segments);
        }
    }
}