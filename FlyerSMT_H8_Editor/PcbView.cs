using System.ComponentModel;
using System.Windows.Forms;

namespace FlyerSMT_H8_Editor
{
    // Board table of one file (FlyerSMT's PCB tab): grid in the designer, behaviour from CompareGrid
    public partial class PcbView : UserControl
    {
        public PcbView()
        {
            InitializeComponent();
            // Boards are matched by their number
            Compare = new CompareGrid(gridPcbs, colNo.DataPropertyName, new string[0]);
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CompareGrid Compare { get; private set; }
    }
}
