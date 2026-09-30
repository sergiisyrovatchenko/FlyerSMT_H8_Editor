using System;
using System.Globalization;
using System.Text;

namespace FlyerSMT_H8_Editor
{
    // Number and text formats shared by the .H8 file, the CSV, the grids and the status bar
    public static class Formats
    {
        // Numbers are shown and parsed like FlyerSMT (29.395), whatever the Windows locale
        public static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

        // Text in .H8 and CSV files: one byte per character
        public static readonly Encoding Latin1 = Encoding.GetEncoding(28591);

        // Same bit pattern, as a save compares: -0.0 vs 0.0 or NaN payloads count as different
        public static bool SameBits(float a, float b) => BitsOf(a) == BitsOf(b);

        public static bool IsNegativeZero(float v) => v == 0 && BitsOf(v) < 0;

        private static int BitsOf(float v) => BitConverter.ToInt32(BitConverter.GetBytes(v), 0);

        // Lengths in mm: "3297.6"
        public static string Mm(double value) => value.ToString("0.0", Inv);
    }
}
