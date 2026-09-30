using System.Drawing;

namespace FlyerSMT_H8_Editor
{
    // Colours shared by all grids
    public static class Theme
    {
        // Cell highlights
        public static readonly Color DiffColor = Color.FromArgb(255, 235, 110);     // differs from the other file
        public static readonly Color MissingColor = Color.FromArgb(255, 205, 160);  // no counterpart in the other file
        public static readonly Color EditedColor = Color.FromArgb(255, 209, 220);   // pastel pink: unsaved edit
        public static readonly Color ReadOnlyColor = Color.FromArgb(243, 243, 243);

        // Feeder rows
        public static readonly Color UsedColor = Color.FromArgb(232, 245, 232);     // feeder used by this file's board
        public static readonly Color BothUsedColor = Color.FromArgb(196, 228, 196); // feeder used by both files' boards
        public static readonly Color DisabledColor = Color.FromArgb(214, 214, 214); // switched off, and components picked from it

        // Nozzle column of the component grid
        public static readonly Color Nozzle1Color = Color.FromArgb(30, 100, 200);
        public static readonly Color Nozzle2Color = Color.FromArgb(170, 50, 160);
    }
}
