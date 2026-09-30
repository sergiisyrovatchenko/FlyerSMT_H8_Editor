using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Windows.Forms;

namespace FlyerSMT_H8_Editor
{
    // Component table of one file: grid in the designer, behaviour from CompareGrid
    public partial class ComponentsView : UserControl
    {
        public ComponentsView()
        {
            InitializeComponent();
            // Matched by designator. No. and Cycle are not compared: one extra part would shift every number after it
            Compare = new CompareGrid(gridComponents, colDesignator.DataPropertyName,
                new[] { colNo.DataPropertyName, colCycle.DataPropertyName });

            gridComponents.EditingControlShowing += GridComponents_EditingControlShowing;
            gridComponents.CellParsing += GridComponents_CellParsing;
            gridComponents.CellFormatting += GridComponents_CellFormatting;
        }

        // Value without a feeder: the component's own text ("-OFF-", FlyerSMT's "---"). Nozzle: N1 blue, N2 purple
        private void GridComponents_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (e.ColumnIndex == colValue.Index)
            {
                if (gridComponents.Rows[e.RowIndex].DataBoundItem is ComponentRow row && row.Slot < 0)
                {
                    e.Value = row.Component.Value;
                    e.FormattingApplied = true;
                }
                return;
            }
            if (e.ColumnIndex != colNozzle.Index) return;
            var nozzle = e.Value as string;
            if (nozzle == "N1") e.CellStyle.ForeColor = Theme.Nozzle1Color;
            else if (nozzle == "N2") e.CellStyle.ForeColor = Theme.Nozzle2Color;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CompareGrid Compare { get; private set; }

        // The Value drop-down; items rebuilt from the file's feeders at run time
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DataGridViewComboBoxColumn ValueColumn => colValue;

        // The open list shows "FD007 - 100nF" (values repeat, slots do not); the box shows the value, like the cell
        private void GridComponents_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is not ComboBox box || gridComponents.CurrentCell?.ColumnIndex != colValue.Index)
                return;
            box.DrawMode = DrawMode.OwnerDrawFixed;
            box.DrawItem -= ValueBox_DrawItem; // the editing control is reused: attach once
            box.DrawItem += ValueBox_DrawItem;
            box.DropDown -= ValueBox_DropDown;
            box.DropDown += ValueBox_DropDown;
        }

        // Wide enough for the longest item; set on each opening, after the grid's own handler narrows it
        private static void ValueBox_DropDown(object sender, System.EventArgs e)
        {
            var box = (ComboBox)sender;
            int widest = box.Items.OfType<FeederChoice>()
                .Select(c => TextRenderer.MeasureText(ListText(c), box.Font).Width).DefaultIfEmpty(0).Max();
            box.DropDownWidth = System.Math.Max(box.Width, widest + SystemInformation.VerticalScrollBarWidth + 8);
        }

        // "FD007 - 100nF", or "FD007 - 100nF (off)" for a disabled feeder
        private static string ListText(FeederChoice c) => c.Enabled ? c.Label : c.Label + " (off)";

        private static void ValueBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            var box = (ComboBox)sender;
            if (e.Index >= 0 && e.Index < box.Items.Count)
            {
                var choice = (FeederChoice)box.Items[e.Index];
                bool edit = (e.State & DrawItemState.ComboBoxEdit) != 0;
                string text = edit ? choice.Text : ListText(choice);
                Color color = !choice.Enabled && !edit ? SystemColors.GrayText
                    : (e.State & DrawItemState.Selected) != 0 ? SystemColors.HighlightText : e.ForeColor;
                TextRenderer.DrawText(e.Graphics, text, e.Font, e.Bounds, color,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.NoPrefix);
            }
            e.DrawFocusRectangle();
        }

        // Slot from the picked item: the grid would look up the shown text ("100nF") and could hit another
        // feeder with that value. A disabled feeder cannot be picked: the list goes back to the current one
        private void GridComponents_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            if (e.ColumnIndex != colValue.Index) return;
            var box = gridComponents.EditingControl as ComboBox;
            if (box?.SelectedItem is not FeederChoice choice) return;
            var row = gridComponents.Rows[e.RowIndex].DataBoundItem as ComponentRow;
            if (!choice.Enabled && row != null && choice.Slot != row.Slot)
            {
                SystemSounds.Beep.Play();
                int keep = row.Slot;
                e.Value = keep;
                e.ParsingApplied = true;
                BeginInvoke(new System.Action(() =>
                {
                    var current = box.Items.OfType<FeederChoice>().FirstOrDefault(c => c.Slot == keep);
                    if (gridComponents.EditingControl == box && current != null)
                        box.SelectedItem = current;
                }));
                return;
            }
            e.Value = choice.Slot;
            e.ParsingApplied = true;
        }
    }
}
