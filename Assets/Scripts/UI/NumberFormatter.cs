namespace IdleSpace.UI
{
    public static class NumberFormatter
    {
        private static readonly string[] Suffixes = { "", "K", "M", "B", "T", "Qa", "Qi" };

        public static string Compact(double value)
        {
            int suffixIndex = 0;
            double display = value;
            while (display >= 1000d && suffixIndex < Suffixes.Length - 1)
            {
                display /= 1000d;
                suffixIndex++;
            }

            return suffixIndex == 0 ? display.ToString("F0") : $"{display:F2}{Suffixes[suffixIndex]}";
        }
    }
}
