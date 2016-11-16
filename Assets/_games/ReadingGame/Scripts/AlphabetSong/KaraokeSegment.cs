namespace EA4S
{
    public struct KaraokeSegment
    {
        public string text;
        public float start;
        public float end;
        public bool starsWithLineBreak;

        public KaraokeSegment(string text, float start, float end, bool starsWithLineBreak)
        {
            this.text = text;
            this.start = start;
            this.end = end;
            this.starsWithLineBreak = starsWithLineBreak;
        }
    }
}