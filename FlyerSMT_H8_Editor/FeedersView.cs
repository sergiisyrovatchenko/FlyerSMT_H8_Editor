using System.ComponentModel;
using System.Windows.Forms;

namespace FlyerSMT_H8_Editor
{
    // Feeder table of one file: grid in the designer, behaviour from CompareGrid
    public partial class FeedersView : UserControl
    {
        public FeedersView()
        {
            InitializeComponent();
            // Key column, and the job-specific columns never compared (they always differ, e.g. top vs bottom)
            Compare = new CompareGrid(gridFeeders, colNumber.DataPropertyName,
                new[] { colUsedBy.DataPropertyName, colOtherUsedBy.DataPropertyName });
            // Component lists: as wide as their text, at most 120 px
            Compare.MaxWidth(colUsedBy, 120);
            Compare.MaxWidth(colOtherUsedBy, 120);
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CompareGrid Compare { get; private set; }

        // Component columns, this file's first: "File 1", "File 2" on file 1's tab; "File 2", "File 1" on file 2's
        public void NameComponentColumns(int file, int otherFile)
        {
            colUsedBy.HeaderText = "File " + file;
            colOtherUsedBy.HeaderText = "File " + otherFile;
        }
    }
}
