using System.Windows.Forms;

namespace FlyerSMT_H8_Editor
{
    public static class AppInfo
    {
        public const string Title = "FlyerSMT H8 Editor";
    }

    // Message boxes titled with the program's name
    internal static class Dialogs
    {
        public static void Error(IWin32Window owner, string text) =>
            MessageBox.Show(owner, text, AppInfo.Title, MessageBoxButtons.OK, MessageBoxIcon.Error);

        public static void Warning(IWin32Window owner, string text) =>
            MessageBox.Show(owner, text, AppInfo.Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        // Yes / No with a warning icon; defaultNo puts the focus on No (commands that lose data)
        public static bool Confirm(IWin32Window owner, string text, bool defaultNo = false) =>
            MessageBox.Show(owner, text, AppInfo.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                defaultNo ? MessageBoxDefaultButton.Button2 : MessageBoxDefaultButton.Button1) == DialogResult.Yes;
    }
}
