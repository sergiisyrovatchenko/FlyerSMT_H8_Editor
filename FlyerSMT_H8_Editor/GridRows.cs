using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Forms;

namespace FlyerSMT_H8_Editor
{
    // A grid row that can be compared, cell by cell, with its counterpart in the other file
    public abstract class GridRow
    {
        // Columns that differ from the saved file (unsaved edits)
        [Browsable(false)] public HashSet<string> EditedColumns { get; } = new HashSet<string>();

        // Saved value per edited column (tooltips, menu)
        [Browsable(false)] public Dictionary<string, string> SavedValues { get; } = new Dictionary<string, string>();

        // Columns that differ from the other file's row
        [Browsable(false)] public HashSet<string> DiffColumns { get; } = new HashSet<string>();

        // The other file's value per differing column (tooltips, menu)
        [Browsable(false)] public Dictionary<string, string> OtherValues { get; } = new Dictionary<string, string>();

        // The other file is loaded but has no counterpart for this row
        [Browsable(false)] public bool Missing { get; set; }

        // Text per column index at the last Capture; the grid compares it to know which widths to refit
        [Browsable(false)] public string[] Shown { get; private set; }

        // Reads every column's text once; both comparisons use it
        public void Capture(DataGridViewColumnCollection columns)
        {
            var shown = new string[columns.Count];
            foreach (DataGridViewColumn col in columns)
                shown[col.Index] = Display(col.DataPropertyName, col.DefaultCellStyle.Format);
            Shown = shown;
        }

        public void ClearDiff()
        {
            DiffColumns.Clear();
            OtherValues.Clear();
            EditedColumns.Clear();
            SavedValues.Clear();
            Missing = false;
        }

        // Columns differing from the saved row (after Capture)
        public void CompareWithSaved(GridRow saved, DataGridViewColumnCollection columns)
        {
            foreach (DataGridViewColumn col in columns)
            {
                string prop = col.DataPropertyName;
                string before = saved.Display(prop, col.DefaultCellStyle.Format);
                if (Shown[col.Index] != before)
                {
                    EditedColumns.Add(prop);
                    SavedValues[prop] = before;
                }
            }
        }

        // Columns differing from the other file's row, compared as displayed, so float noise below the shown
        // precision is ignored (after Capture)
        public void Compare(GridRow other, DataGridViewColumnCollection columns, ICollection<string> notCompared)
        {
            foreach (DataGridViewColumn col in columns)
            {
                string prop = col.DataPropertyName;
                if (notCompared.Contains(prop)) continue;
                string theirs = other.Display(prop, col.DefaultCellStyle.Format);
                if (Shown[col.Index] != theirs)
                {
                    DiffColumns.Add(prop);
                    OtherValues[prop] = theirs;
                }
            }
        }

        private PropertyInfo Property(string prop) => GetType().GetProperty(prop);

        // Compiled getter per row type and property: every edit reads every cell three times (shown, saved, other
        // file), and PropertyInfo.GetValue is several times slower
        private static readonly Dictionary<Type, Dictionary<string, Func<GridRow, object>>> GetterCache =
            new Dictionary<Type, Dictionary<string, Func<GridRow, object>>>();

        private Func<GridRow, object> Getter(string prop)
        {
            var type = GetType();
            if (!GetterCache.TryGetValue(type, out var byName))
                GetterCache[type] = byName = new Dictionary<string, Func<GridRow, object>>();
            if (!byName.TryGetValue(prop, out var getter))
            {
                // row => (object)((TRow)row).Prop
                var row = Expression.Parameter(typeof(GridRow), "row");
                var value = Expression.Property(Expression.Convert(row, type), type.GetProperty(prop));
                byName[prop] = getter = Expression.Lambda<Func<GridRow, object>>(Expression.Convert(value, typeof(object)), row).Compile();
            }
            return getter;
        }

        // The text a column is compared and shown by (tooltips, menus); overridden where the cell shows only
        // part of the value
        public virtual string Display(string prop, string format)
        {
            object value = Getter(prop)(this);
            if (value is IFormattable formattable && !string.IsNullOrEmpty(format))
                return formattable.ToString(format, Formats.Inv);
            return value == null ? "" : Convert.ToString(value, Formats.Inv);
        }

        // Copies one column from a row of the same kind; overridden where the shown text is rounded
        public virtual void CopyFrom(GridRow source, string prop)
        {
            var p = Property(prop);
            p.SetValue(this, p.GetValue(source, null), null);
        }

        // Puts back one saved column; overridden where the setter does more than store the value
        public virtual void RevertFrom(GridRow saved, string prop) => CopyFrom(saved, prop);
    }

    // Feeder slot row; setters write into the feeder
    public sealed class FeederRow : GridRow
    {
        private readonly H8Feeder _f;
        private readonly H8File _file;
        private readonly H8File _other;

        public FeederRow(H8Feeder f, H8File file, H8File other)
        {
            _f = f;
            _file = file;
            _other = other;
        }

        [Browsable(false)] public H8Feeder Feeder => _f;

        private static readonly List<string> None = new List<string>();

        // From the file's usage snapshot (refreshed per edit), not a scan per cell
        private static List<string> UsedIn(H8File file, int feederIndex) => file == null ? None : file.DesignatorsOn(feederIndex);

        // Live, so reassigned components show without a rebind
        [Browsable(false)] public int UsedCount => UsedIn(_file, _f.Index).Count;
        [Browsable(false)] public int OtherUsedCount => UsedIn(_other, _f.Index).Count;

        public string Number => _f.Number;
        public bool Enabled { get => _f.Enabled; set => _f.Enabled = value; }
        public string Value { get => _f.Value; set => _file.SetFeederValue(_f, value); }
        public string Package { get => _f.Package; set => _file.SetFeederPackage(_f, value); }
        public float X { get => _f.X; set => _f.X = value; }
        public float Y { get => _f.Y; set => _f.Y = value; }
        public float HoleX { get => _f.HoleX; set => _f.HoleX = value; }
        public float HoleY { get => _f.HoleY; set => _f.HoleY = value; }
        public float Angle { get => _f.Angle; set => _f.Angle = value; }
        public float NozzleHeight { get => _f.NozzleHeight; set => _f.NozzleHeight = value; }
        public float Thickness { get => _f.Thickness; set => _f.Thickness = value; }
        public float Distance { get => _f.Distance; set => _f.Distance = value; }

        // "W*H" ("2.01*1.26") as in FlyerSMT; typing takes '*' or 'x' and a decimal comma. A side whose shown
        // value is unchanged keeps its exact stored number
        public string Size
        {
            get => SizeText(_f.SizeW, _f.SizeH);
            set
            {
                string text = (value ?? "").Trim().Replace(',', '.');
                string[] parts = text.Split('*', 'x', 'X');
                if (parts.Length != 2
                    || !float.TryParse(parts[0].Trim(), NumberStyles.Float, Formats.Inv, out float w)
                    || !float.TryParse(parts[1].Trim(), NumberStyles.Float, Formats.Inv, out float h))
                    throw new ArgumentException($"Size \"{value}\" is not valid: enter width*height, e.g. 2.01*1.26.");
                if (w.ToString(SizeFormat, Formats.Inv) != _f.SizeW.ToString(SizeFormat, Formats.Inv)) _f.SizeW = w;
                if (h.ToString(SizeFormat, Formats.Inv) != _f.SizeH.ToString(SizeFormat, Formats.Inv)) _f.SizeH = h;
            }
        }

        private const string SizeFormat = "0.00";

        private static string SizeText(float w, float h) => w.ToString(SizeFormat, Formats.Inv) + "*" + h.ToString(SizeFormat, Formats.Inv);

        // Size copies and reverts the exact numbers, not the rounded text
        public override void CopyFrom(GridRow source, string prop)
        {
            if (prop != nameof(Size))
            {
                base.CopyFrom(source, prop);
                return;
            }
            var s = ((FeederRow)source)._f;
            _f.SizeW = s.SizeW;
            _f.SizeH = s.SizeH;
        }

        public bool Vision { get => _f.Vision; set => _f.Vision = value; }

        public string VisionType
        {
            get => _f.VisionTypeName;
            set
            {
                int i = Array.IndexOf(H8Feeder.VisionTypeNames, value);
                if (i >= 0) _f.VisionType = i;
            }
        }

        // value when within min..max; otherwise the edit is rejected with a message
        private static int InRange(int value, int min, int max, string name)
        {
            if (value < min || value > max)
                throw new ArgumentException($"{name} {value} is out of range: allowed {min} to {max}.");
            return value;
        }

        // Vision threshold, a grey level 1..255; 0 = automatic
        public int Threshold { get => _f.Threshold; set => _f.Threshold = InRange(value, 0, 255, "Threshold"); }

        public bool Nozzle1 { get => _f.Allows(Nozzles.N1); set => _f.Allow(Nozzles.N1, value); }
        public bool Nozzle2 { get => _f.Allows(Nozzles.N2); set => _f.Allow(Nozzles.N2, value); }

        // Head speeds, 1..100
        public int TakeDown { get => _f.TakeDown; set => _f.TakeDown = InRange(value, 1, 100, "Take down speed"); }
        public int TakeUp { get => _f.TakeUp; set => _f.TakeUp = InRange(value, 1, 100, "Take up speed"); }
        public int PasteDown { get => _f.PasteDown; set => _f.PasteDown = InRange(value, 1, 100, "Paste down speed"); }
        public int PasteUp { get => _f.PasteUp; set => _f.PasteUp = InRange(value, 1, 100, "Paste up speed"); }
        public string UsedBy => string.Join(", ", UsedIn(_file, _f.Index).ToArray());
        public string OtherUsedBy => string.Join(", ", UsedIn(_other, _f.Index).ToArray());
    }

    // Component row; setters write into the component
    public sealed class ComponentRow : GridRow
    {
        public const string NoFeeder = "NULL"; // how FlyerSMT's Feeder column shows a component without a feeder

        private readonly H8Component _c;
        private readonly H8File _file;

        public ComponentRow(H8Component c, H8File file)
        {
            _c = c;
            _file = file;
        }

        [Browsable(false)] public H8Component Component => _c;

        // Picked from a feeder whose ON is unticked
        [Browsable(false)]
        public bool OnDisabledFeeder
        {
            get
            {
                var f = _file.FeederOf(_c);
                return f != null && !f.Enabled;
            }
        }

        // "FD007": the slot's number, as on the Feeders tab
        public static string FeederNumber(int index) =>
            index < 0 || index >= H8File.FeederCount ? NoFeeder : H8Feeder.NumberOf(index);

        // "FD007 - 100nF": slot number plus what the slot holds in that file
        public static string FeederLabel(H8File file, int index)
        {
            string number = FeederNumber(index);
            if (number == NoFeeder) return number;
            string value = FeederValue(file, index);
            return value == "" ? number : number + " - " + value;
        }

        private static string FeederValue(H8File file, int index) =>
            file != null && index >= 0 && index < file.Feeders.Count ? file.Feeders[index].Value : "";

        // Value drop-down: -OFF- (no feeder: FlyerSMT skips the part) and the enabled feeders. A disabled
        // feeder is listed (greyed, not selectable) only while a component of this file still sits on it
        public static List<FeederChoice> FeederChoices(H8File file)
        {
            var list = new List<FeederChoice> { new FeederChoice(-1, H8File.NullValue, H8File.NullValue, true) };
            for (int i = 0; i < H8File.FeederCount; i++)
            {
                bool enabled = file != null && i < file.Feeders.Count && file.Feeders[i].Enabled;
                if (enabled || (file != null && file.DesignatorsOn(i).Count > 0))
                    list.Add(new FeederChoice(i, FeederValue(file, i), FeederLabel(file, i), enabled));
            }
            return list;
        }

        // Placement order, like the NO. column in FlyerSMT
        public int No => _c.Index + 1;

        public string Designator { get => _c.Designator; set => _file.SetDesignator(_c, value); }
        // From the feeder's package (AssignFeeder, SetFeederPackage); read-only here
        public string Footprint => _c.Footprint;
        public float X { get => _c.X; set => _c.X = value; }
        public float Y { get => _c.Y; set => _c.Y = value; }
        public float Rotation { get => _c.Rotation; set => _c.Rotation = value; }

        // Pick height as in FlyerSMT: board height minus the feeder's thickness
        public string Z
        {
            get
            {
                var f = _file.FeederOf(_c);
                if (f == null || float.IsNaN(_file.PcbHeight)) return "";
                return (_file.PcbHeight - f.Thickness).ToString("0.0", Formats.Inv);
            }
        }

        // The feeder slot, chosen in the Value column: the cell shows the feeder's value ("100nF"), the list
        // "FD007 - 100nF"; picking sets slot, value and package. Compared as "FD007 - 100nF", so the same value
        // on another slot is a difference, and taking it from the other file takes that slot
        public int Slot
        {
            get => _file.FeederOf(_c)?.Index ?? -1;
            set => _file.AssignFeeder(_c, value);
        }

        // "FD007" (FlyerSMT: "7#"); follows the Value column
        public string Feeder => FeederNumber(Slot);

        // Nozzles the feeder allows (N1 / N2 on the Feeders tab); both off without a feeder
        public bool FeederN1 => _file.FeederOf(_c)?.Allows(Nozzles.N1) ?? false;
        public bool FeederN2 => _file.FeederOf(_c)?.Allows(Nozzles.N2) ?? false;

        // Cycle and nozzle of the pick (PathPlan); empty when not placed
        public string Cycle => _file.Plan.PickOf(_c)?.Cycle.ToString(Formats.Inv) ?? "";

        public string Nozzle
        {
            get
            {
                var op = _file.Plan.PickOf(_c);
                return op == null ? "" : "N" + op.Nozzle;
            }
        }

        public override string Display(string prop, string format)
        {
            if (prop != nameof(Slot)) return base.Display(prop, format);
            // No feeder: "NULL - " plus the component's own value
            return Slot < 0 ? NoFeeder + " - " + _c.Value : FeederLabel(_file, Slot);
        }

        public override void RevertFrom(GridRow saved, string prop)
        {
            if (prop != nameof(Slot))
            {
                base.RevertFrom(saved, prop);
                return;
            }
            // Undo all that picking a feeder did: slot, value and footprint
            var s = ((ComponentRow)saved)._c;
            _c.FeederIndex = s.FeederIndex;
            _c.Value = s.Value;
            _c.Footprint = s.Footprint;
        }
    }

    // Board row (PCB tab); setters write into the board
    public sealed class PcbRow : GridRow
    {
        private readonly H8Pcb _p;

        public PcbRow(H8Pcb p)
        {
            _p = p;
        }

        [Browsable(false)] public H8Pcb Pcb => _p;

        public int No => _p.Index + 1;
        public float X { get => _p.X; set => _p.X = value; }
        public float Y { get => _p.Y; set => _p.Y = value; }
        public float A { get => _p.A; set => _p.A = value; }
        public bool On { get => _p.On; set => _p.On = value; }
    }

    // One item of the component Value drop-down: the cell shows Text, the open list shows Label
    public sealed class FeederChoice
    {
        public FeederChoice(int slot, string text, string label, bool enabled)
        {
            Slot = slot;
            Text = text;
            Label = label;
            Enabled = enabled;
        }

        public int Slot { get; }     // 0-based feeder slot, -1 = none
        public string Text { get; }  // "100nF"
        public string Label { get; } // "FD007 - 100nF"
        public bool Enabled { get; } // false: a disabled feeder, shown but not selectable

        // Identifies the item for "has the list changed?" checks
        public string Key => Label + (Enabled ? "" : " (off)");
    }
}
