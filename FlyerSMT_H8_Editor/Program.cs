using System;
using System.Windows.Forms;

namespace FlyerSMT_H8_Editor
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(args)); // up to two .H8 paths: file 1, file 2
        }
    }
}
