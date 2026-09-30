using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Windows.Forms;

namespace FlyerSMT_H8_Editor
{
    // A grid's current cell and scroll, kept across a rebind (undo, redo)
    public struct GridPosition
    {
        public int Row, Column;   // current cell; -1 = none
        public int FirstVisible;  // first row shown; -1 = none
    }

    // Editable grid compared with the other file: yellow = differs, pink = unsaved edit, orange key cell =
    // no counterpart. Right-click takes values from the other file
    public sealed class CompareGrid
    {
        // A cell by row and bound property: what Take and Revert act on
        private readonly struct CellRef
        {
            public CellRef(int row, string prop)
            {
                Row = row;
                Prop = prop;
            }

            public int Row { get; }
            public string Prop { get; }
        }

        private List<GridRow> _rows = new List<GridRow>();
        private bool _otherLoaded;
        private readonly string _keyColumn;
        private readonly HashSet<string> _notCompared;

        public readonly DataGridView Grid;

        // The row's counterpart in the other file (null = none)
        public Func<GridRow, GridRow> Counterpart;

        // The row as saved in the file; cells that differ from it are pink
        public Func<GridRow, GridRow> Saved;

        // Swaps two rows' values (the rows stay); when set, Move up / Move down are offered
        public Action<GridRow, GridRow> SwapData;

        // Removes rows from the data (the owner rebinds); when set, Delete is offered
        public Action<List<GridRow>> DeleteData;

        // Row background when no highlight applies (null = default)
        public Func<GridRow, Color?> RowColor;

        // "file 2", for menus and tooltips
        public Func<string> OtherName;

        // After any edit: the owner recomputes diffs and the dirty state
        public event Action Changed;

        public int DiffRows { get; private set; }
        public int MissingRows { get; private set; }

        // Adds comparison to a grid made in the designer; columns bind to row properties by DataPropertyName
        public CompareGrid(DataGridView grid, string keyColumn, IEnumerable<string> notCompared)
        {
            Grid = grid;
            _keyColumn = keyColumn;
            _notCompared = new HashSet<string>(notCompared);

            // Settings the designer cannot hold
            Grid.AutoGenerateColumns = false;
            Grid.DefaultCellStyle.FormatProvider = Formats.Inv; // 29.395 whatever the Windows locale, as in FlyerSMT

            SetupEditing();
            SetupFormatting();
            SetupContextMenu();

            // Double-buffered: painting is several times faster and does not flicker (protected, hence reflection)
            typeof(Control).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(Grid, true, null);

            Grid.CellPainting += PaintCheckBox;
            Grid.Disposed += (s, e) => { foreach (var b in _checkGlyphs.Values) b.Dispose(); };

            Grid.VisibleChanged += (s, e) => { if (_fitPending.Count > 0 && Grid.Visible) FitPending(); };
        }

        // Margin over the widest header or value
        private const double WidthSlack = 0.05;

        // Columns to fit once the grid is shown
        private readonly HashSet<int> _fitPending = new HashSet<int>();

        // Each row's text per column at the last RefreshDiffs; null right after binding
        private string[][] _lastShown;

        // Fits columns to the widest header or value + 5 % (the built-in auto-size has no margin). Measuring
        // formats every cell, so only columns whose text changed are fitted, and hidden grids wait to be shown,
        // except right after binding: then all are fitted, so every tab opens at once
        private void FitColumns(IEnumerable<int> columns, bool evenIfHidden)
        {
            _fitPending.UnionWith(columns);
            if (evenIfHidden || Grid.Visible) FitPending();
        }

        // Widest a fitted column may get, px (longer text is cut, the tooltip shows it all)
        private readonly Dictionary<int, int> _maxWidth = new Dictionary<int, int>();

        public void MaxWidth(DataGridViewColumn column, int pixels)
        {
            _maxWidth[column.Index] = pixels;
        }

        private void FitPending()
        {
            foreach (int c in _fitPending)
            {
                if (c >= Grid.Columns.Count) continue;
                var col = Grid.Columns[c];
                int width = (int)Math.Ceiling(col.GetPreferredWidth(DataGridViewAutoSizeColumnMode.AllCells, true) * (1 + WidthSlack));
                if (_maxWidth.TryGetValue(c, out int max)) width = Math.Min(width, max);
                if (col.Width != width)
                    col.Width = width;
            }
            _fitPending.Clear();
        }

        // Check boxes from a cached bitmap per (ticked, background): themed drawing cost ~25 % of the painting
        private readonly Dictionary<KeyValuePair<bool, int>, Bitmap> _checkGlyphs = new Dictionary<KeyValuePair<bool, int>, Bitmap>();

        private void PaintCheckBox(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || !(Grid.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn)) return;
            bool ticked = e.Value is true;
            bool selected = (e.State & DataGridViewElementStates.Selected) != 0;
            Color back = selected ? e.CellStyle.SelectionBackColor : e.CellStyle.BackColor;

            e.Paint(e.ClipBounds, DataGridViewPaintParts.Background | DataGridViewPaintParts.Border |
                                  DataGridViewPaintParts.SelectionBackground | DataGridViewPaintParts.Focus);
            var glyph = CheckGlyph(ticked, back);
            e.Graphics.DrawImageUnscaled(glyph,
                e.CellBounds.X + (e.CellBounds.Width - glyph.Width) / 2,
                e.CellBounds.Y + (e.CellBounds.Height - glyph.Height) / 2);
            e.Handled = true;
        }

        private Bitmap CheckGlyph(bool ticked, Color back)
        {
            var key = new KeyValuePair<bool, int>(ticked, back.ToArgb());
            if (_checkGlyphs.TryGetValue(key, out var glyph)) return glyph;
            var state = ticked ? System.Windows.Forms.VisualStyles.CheckBoxState.CheckedNormal
                               : System.Windows.Forms.VisualStyles.CheckBoxState.UncheckedNormal;
            using (var g0 = Grid.CreateGraphics())
            {
                var size = CheckBoxRenderer.GetGlyphSize(g0, state);
                glyph = new Bitmap(size.Width, size.Height);
            }
            using (var g = Graphics.FromImage(glyph))
            {
                g.Clear(back);
                CheckBoxRenderer.DrawCheckBox(g, Point.Empty, state);
            }
            _checkGlyphs[key] = glyph;
            return glyph;
        }

        public void Bind<T>(List<T> rows) where T : GridRow
        {
            _rows = rows.Cast<GridRow>().ToList();
            _lastShown = null;
            Grid.DataSource = rows.Count == 0 ? null : new BindingList<T>(rows);
        }

        public void RefreshDiffs(bool otherLoaded)
        {
            _otherLoaded = otherLoaded;
            foreach (var row in _rows)
            {
                row.ClearDiff();
                row.Capture(Grid.Columns);
                var saved = Saved != null ? Saved(row) : null;
                if (saved != null) row.CompareWithSaved(saved, Grid.Columns);
                if (!otherLoaded || Counterpart == null) continue;
                var twin = Counterpart(row);
                if (twin == null) row.Missing = true;
                else row.Compare(twin, Grid.Columns, _notCompared);
            }
            DiffRows = _rows.Count(r => r.DiffColumns.Count > 0);
            MissingRows = _rows.Count(r => r.Missing);
            bool firstSinceBinding = _lastShown == null;
            FitColumns(ChangedColumns(), evenIfHidden: firstSinceBinding);
            Grid.Invalidate();
        }

        // Columns whose text changed in any row since the last RefreshDiffs (all right after binding)
        private IEnumerable<int> ChangedColumns()
        {
            var now = _rows.Select(r => r.Shown).ToArray();
            var before = _lastShown;
            _lastShown = now;
            int columns = Grid.Columns.Count;
            if (before == null || before.Length != now.Length)
                return Enumerable.Range(0, columns);
            var changed = new List<int>();
            for (int c = 0; c < columns; c++)
                for (int r = 0; r < now.Length; r++)
                    if (now[r][c] != before[r][c]) { changed.Add(c); break; }
            return changed;
        }

        private void RaiseChanged() => Changed?.Invoke();

        private void SetupEditing()
        {
            // Check boxes and drop-downs commit at once, not when the cell loses focus
            Grid.CurrentCellDirtyStateChanged += (s, e) =>
            {
                if (Grid.IsCurrentCellDirty && !(Grid.CurrentCell is DataGridViewTextBoxCell))
                    Grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };
            Grid.CellValueChanged += (s, e) =>
            {
                if (e.RowIndex >= 0) RaiseChanged();
            };

            // Characters the file cannot hold are not typed (e.g. Cyrillic); pasted text is checked by the model
            Grid.EditingControlShowing += (s, e) =>
            {
                if (e.Control is not TextBox box) return;
                box.KeyPress -= RejectNonLatin1; // the editing control is reused: attach once
                box.KeyPress -= RejectNonDigit;
                box.KeyPress -= RejectNonNumber;
                box.KeyPress += RejectNonLatin1;
                // Whole numbers: digits only. Decimals: digits, '.' or ',', minus
                var cell = Grid.CurrentCell;
                if (cell != null && cell.ValueType == typeof(int))
                    box.KeyPress += RejectNonDigit;
                else if (cell != null && cell.ValueType == typeof(float))
                    box.KeyPress += RejectNonNumber;
            };

            // Accept both "29.395" and "29,395"
            Grid.CellParsing += (s, e) =>
            {
                if (e.Value == null) return;
                var text = e.Value.ToString().Trim().Replace(',', '.');
                if (e.DesiredType == typeof(float))
                {
                    if (float.TryParse(text, NumberStyles.Float, Formats.Inv, out float f)) { e.Value = f; e.ParsingApplied = true; }
                }
                else if (e.DesiredType == typeof(int))
                {
                    if (int.TryParse(text, NumberStyles.Integer, Formats.Inv, out int i)) { e.Value = i; e.ParsingApplied = true; }
                }
            };
            // No more decimals than the column shows ("0.0" = 1), so nothing is hidden by rounding
            Grid.CellValidating += (s, e) =>
            {
                if (!Grid.IsCurrentCellInEditMode || e.RowIndex < 0) return;
                var col = Grid.Columns[e.ColumnIndex];
                if (Grid[e.ColumnIndex, e.RowIndex].ValueType != typeof(float)) return;
                int allowed = DecimalsOf(col.DefaultCellStyle.Format);
                if (allowed < 0) return;
                string text = Convert.ToString(e.FormattedValue, Formats.Inv).Trim().Replace(',', '.');
                int dot = text.IndexOf('.');
                int typed = dot < 0 ? 0 : text.Skip(dot + 1).TakeWhile(char.IsDigit).Count();
                if (typed <= allowed) return;
                e.Cancel = true;
                ShowWarning($"{col.HeaderText}: at most {allowed} {(allowed == 1 ? "digit" : "digits")} after the decimal point (\"{text}\").");
            };
            Grid.DataError += (s, e) =>
            {
                // Drop-downs hold model values, never typed text: "not valid" only means the list was being rebuilt
                // (swap, rename, revert, save). Ignore it and repaint once the list is current
                if (e.ColumnIndex >= 0 && Grid.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn
                    && (e.Context & DataGridViewDataErrorContexts.Parsing) == 0)
                {
                    e.ThrowException = false;
                    e.Cancel = false;
                    if (Grid.IsHandleCreated)
                        Grid.BeginInvoke(new Action(Grid.Invalidate));
                    return;
                }
                string text = Grid.EditingControl?.Text ?? "";
                ShowWarning(Unwrap(e.Exception) is ArgumentException rule
                    ? rule.Message
                    : $"\"{text}\" is not a valid value for {Grid.Columns[e.ColumnIndex].HeaderText}.");
                e.Cancel = true;
            };
        }

        private static void RejectNonDigit(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !(e.KeyChar >= '0' && e.KeyChar <= '9'))
            {
                e.Handled = true;
                SystemSounds.Beep.Play();
            }
        }

        private static void RejectNonNumber(object sender, KeyPressEventArgs e)
        {
            char ch = e.KeyChar;
            if (!char.IsControl(ch) && !(ch >= '0' && ch <= '9') && ch != '.' && ch != ',' && ch != '-')
            {
                e.Handled = true;
                SystemSounds.Beep.Play();
            }
        }

        private static void RejectNonLatin1(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !H8File.IsAllowedChar(e.KeyChar))
            {
                e.Handled = true;
                SystemSounds.Beep.Play();
            }
        }

        private void SetupFormatting()
        {
            Grid.CellFormatting += (s, e) =>
            {
                if (e.RowIndex < 0 || e.RowIndex >= _rows.Count) return;
                var row = _rows[e.RowIndex];
                var col = Grid.Columns[e.ColumnIndex];
                if (row.EditedColumns.Contains(col.DataPropertyName))
                {
                    e.CellStyle.BackColor = Theme.EditedColor;
                    e.CellStyle.SelectionBackColor = Color.PaleVioletRed;
                    return;
                }
                if (row.DiffColumns.Contains(col.DataPropertyName))
                {
                    e.CellStyle.BackColor = Theme.DiffColor;
                    e.CellStyle.SelectionBackColor = Color.DarkGoldenrod;
                    return;
                }
                if (row.Missing && col.DataPropertyName == _keyColumn)
                {
                    e.CellStyle.BackColor = Theme.MissingColor;
                    e.CellStyle.SelectionBackColor = Color.Chocolate;
                    return;
                }
                Color? rowColor = RowColor?.Invoke(row);
                if (rowColor.HasValue)
                    e.CellStyle.BackColor = rowColor.Value;
                else if (col.ReadOnly)
                    e.CellStyle.BackColor = Theme.ReadOnlyColor;
            };
            Grid.CellToolTipTextNeeded += (s, e) =>
            {
                if (e.RowIndex < 0 || e.RowIndex >= _rows.Count) return;
                var row = _rows[e.RowIndex];
                string prop = Grid.Columns[e.ColumnIndex].DataPropertyName;
                var lines = new List<string>();
                if (row.SavedValues.TryGetValue(prop, out var saved))
                    lines.Add("Saved in file: " + saved);
                if (row.OtherValues.TryGetValue(prop, out var theirs))
                    lines.Add($"In {OtherName()}: {theirs}");
                if (row.Missing && prop == _keyColumn)
                    lines.Add("Not in " + OtherName());
                if (lines.Count > 0)
                    e.ToolTipText = string.Join("\n", lines.ToArray());
            };
        }

        private void SetupContextMenu()
        {
            var menu = new ContextMenuStrip();
            var takeCell = new ToolStripMenuItem();
            var takeRow = new ToolStripMenuItem();
            var separator = new ToolStripSeparator();
            var revert = new ToolStripMenuItem();
            var moveSeparator = new ToolStripSeparator();
            var moveUp = new ToolStripMenuItem("Move up") { ShortcutKeyDisplayString = "Ctrl+Up" };
            var moveDown = new ToolStripMenuItem("Move down") { ShortcutKeyDisplayString = "Ctrl+Down" };
            var deleteSeparator = new ToolStripSeparator();
            var delete = new ToolStripMenuItem();
            menu.Items.Add(takeCell);
            menu.Items.Add(takeRow);
            menu.Items.Add(separator);
            menu.Items.Add(revert);
            menu.Items.Add(moveSeparator);
            menu.Items.Add(moveUp);
            menu.Items.Add(moveDown);
            menu.Items.Add(deleteSeparator);
            menu.Items.Add(delete);
            Grid.ContextMenuStrip = menu;

            moveUp.Click += (s, e) => MoveRows(-1);
            moveDown.Click += (s, e) => MoveRows(+1);
            delete.Click += (s, e) => DeleteRows();
            Grid.KeyDown += (s, e) =>
            {
                if (SwapData == null || !e.Control || (e.KeyCode != Keys.Up && e.KeyCode != Keys.Down)) return;
                MoveRows(e.KeyCode == Keys.Up ? -1 : +1);
                e.Handled = true;
            };

            // Right-click selects the cell under the mouse (a multi-cell selection around it is kept)
            Grid.CellMouseDown += (s, e) =>
            {
                if (e.Button != MouseButtons.Right || e.RowIndex < 0 || e.ColumnIndex < 0) return;
                var cell = Grid[e.ColumnIndex, e.RowIndex];
                if (!cell.Selected)
                {
                    Grid.ClearSelection();
                    Grid.CurrentCell = cell;
                }
            };

            // Only commands that would do something are shown; with none, the menu does not open
            menu.Opening += (s, e) =>
            {
                if (_rows.Count == 0 || Grid.SelectedCells.Count == 0)
                {
                    e.Cancel = true;
                    return;
                }
                var rows = SelectedRows();

                // Take: the other file is open and something selected differs from it
                bool canTake = _otherLoaded && Counterpart != null;
                var cells = canTake ? SyncTargets(allColumns: false) : new List<CellRef>();
                bool showTakeCell = cells.Count > 0;
                bool showTakeRow = canTake && SyncTargets(allColumns: true).Count > 0;
                if (showTakeCell || showTakeRow)
                {
                    string from = OtherName();
                    string theirs = null;
                    if (Grid.SelectedCells.Count == 1)
                    {
                        var cell = Grid.SelectedCells[0];
                        _rows[cell.RowIndex].OtherValues.TryGetValue(Grid.Columns[cell.ColumnIndex].DataPropertyName, out theirs);
                    }
                    takeCell.Text = theirs != null
                        ? $"Take value from {from}: {theirs}"
                        : $"Take {cells.Count} differing value(s) from {from}";
                    takeRow.Text = rows.Count == 1
                        ? $"Take whole row {_rows[rows[0]].Display(_keyColumn, null)} from {from}"
                        : $"Take whole rows ({rows.Count}) from {from}";
                }

                // Revert: something selected differs from the saved file
                var edited = RevertTargets();
                bool showRevert = edited.Count > 0;
                if (showRevert)
                {
                    string savedText = null;
                    if (Grid.SelectedCells.Count == 1)
                    {
                        var cell = Grid.SelectedCells[0];
                        _rows[cell.RowIndex].SavedValues.TryGetValue(Grid.Columns[cell.ColumnIndex].DataPropertyName, out savedText);
                    }
                    revert.Text = savedText != null ? "Revert value: " + savedText : $"Revert {edited.Count} changed value(s)";
                }

                // Move: allowed here, and there is a neighbour in that direction
                bool showUp = SwapData != null && rows.Count > 0 && rows[0] > 0;
                bool showDown = SwapData != null && rows.Count > 0 && rows[rows.Count - 1] < _rows.Count - 1;

                // Delete: allowed here
                bool showDelete = DeleteData != null && rows.Count > 0;
                if (showDelete)
                    delete.Text = rows.Count == 1
                        ? "Delete " + _rows[rows[0]].Display(_keyColumn, null)
                        : $"Delete {rows.Count} rows";

                takeCell.Visible = showTakeCell;
                takeRow.Visible = showTakeRow;
                revert.Visible = showRevert;
                moveUp.Visible = showUp;
                moveDown.Visible = showDown;
                delete.Visible = showDelete;

                // A separator shows only between two shown groups
                bool[] groups = { showTakeCell || showTakeRow, showRevert, showUp || showDown, showDelete };
                var separators = new ToolStripItem[] { null, separator, moveSeparator, deleteSeparator };
                bool above = groups[0];
                for (int k = 1; k < groups.Length; k++)
                {
                    separators[k].Visible = groups[k] && above;
                    above |= groups[k];
                }

                if (!above)
                    e.Cancel = true;
            };
            takeCell.Click += (s, e) => TakeFromOther(SyncTargets(allColumns: false));
            revert.Click += (s, e) => RevertCells(RevertTargets());
            takeRow.Click += (s, e) => TakeFromOther(SyncTargets(allColumns: true));
        }

        // Moves the selected rows up (-1) or down (+1) by swapping values with the neighbour; the selection
        // follows, so the command can be repeated
        private void MoveRows(int delta)
        {
            var rows = SelectedRows();
            if (SwapData == null || rows.Count == 0) return;
            if (delta < 0 && rows[0] == 0) return;
            if (delta > 0 && rows[rows.Count - 1] == _rows.Count - 1) return;
            if (!Grid.EndEdit()) return;

            var cells = Grid.SelectedCells.Cast<DataGridViewCell>().Select(c => new { c.RowIndex, c.ColumnIndex }).ToList();
            var current = Grid.CurrentCell;
            int currentCol = current?.ColumnIndex ?? cells[0].ColumnIndex;
            int currentRow = current?.RowIndex ?? cells[0].RowIndex;
            int firstVisible = Grid.FirstDisplayedScrollingRowIndex;

            foreach (int r in delta < 0 ? rows : Enumerable.Reverse(rows))
                SwapData(_rows[r], _rows[r + delta]);

            Grid.CurrentCell = Grid[currentCol, currentRow + (rows.Contains(currentRow) ? delta : 0)];
            Grid.ClearSelection();
            foreach (var c in cells)
                Grid[c.ColumnIndex, c.RowIndex + delta].Selected = true;
            if (firstVisible >= 0 && firstVisible < Grid.RowCount)
                Grid.FirstDisplayedScrollingRowIndex = firstVisible;

            RaiseChanged();
            Grid.Refresh();
        }

        public GridPosition Position()
        {
            var cell = Grid.CurrentCell;
            return new GridPosition
            {
                Row = cell?.RowIndex ?? -1,
                Column = cell?.ColumnIndex ?? -1,
                FirstVisible = Grid.FirstDisplayedScrollingRowIndex,
            };
        }

        public void RestorePosition(GridPosition position)
        {
            if (Grid.RowCount == 0) return;
            if (position.FirstVisible >= 0)
                Grid.FirstDisplayedScrollingRowIndex = Math.Min(position.FirstVisible, Grid.RowCount - 1);
            if (position.Row >= 0 && position.Column >= 0 && position.Column < Grid.ColumnCount)
                Grid.CurrentCell = Grid[position.Column, Math.Min(position.Row, Grid.RowCount - 1)];
        }

        // Deletes the selected rows via DeleteData (the owner rebinds), keeping the scroll; the cursor lands on
        // the row that took the first deleted one's place
        private void DeleteRows()
        {
            var rows = SelectedRows();
            if (DeleteData == null || rows.Count == 0 || !Grid.EndEdit()) return;
            int firstVisible = Grid.FirstDisplayedScrollingRowIndex;
            int column = Grid.CurrentCell?.ColumnIndex ?? 0;

            DeleteData(rows.Select(r => _rows[r]).ToList());

            if (Grid.RowCount == 0) return;
            if (firstVisible >= 0)
                Grid.FirstDisplayedScrollingRowIndex = Math.Min(firstVisible, Grid.RowCount - 1);
            Grid.CurrentCell = Grid[column, Math.Min(rows[0], Grid.RowCount - 1)];
        }

        private List<int> SelectedRows()
        {
            return Grid.SelectedCells.Cast<DataGridViewCell>().Select(c => c.RowIndex).Distinct().OrderBy(i => i).ToList();
        }

        // Cells to copy: differing, editable cells of the selection (or of the selected rows)
        private List<CellRef> SyncTargets(bool allColumns)
        {
            var candidates = allColumns
                ? SelectedRows().SelectMany(r => Grid.Columns.Cast<DataGridViewColumn>().Select(col => new { Row = r, Column = col }))
                : Grid.SelectedCells.Cast<DataGridViewCell>().Select(c => new { Row = c.RowIndex, Column = Grid.Columns[c.ColumnIndex] });
            return candidates
                .Where(t => IsEditable(t.Row, t.Column) && _rows[t.Row].DiffColumns.Contains(t.Column.DataPropertyName))
                .Select(t => new CellRef(t.Row, t.Column.DataPropertyName))
                .ToList();
        }

        // Selected, editable cells that differ from the saved file
        private List<CellRef> RevertTargets()
        {
            return Grid.SelectedCells.Cast<DataGridViewCell>()
                .Select(c => new { Row = c.RowIndex, Column = Grid.Columns[c.ColumnIndex] })
                .Where(t => IsEditable(t.Row, t.Column) && _rows[t.Row].EditedColumns.Contains(t.Column.DataPropertyName))
                .Select(t => new CellRef(t.Row, t.Column.DataPropertyName))
                .ToList();
        }

        private bool IsEditable(int row, DataGridViewColumn column) => !column.ReadOnly && row >= 0 && row < _rows.Count;

        // Digits after the point of a "0.00"-style format (2); "0" = 0; any other format -1 (no limit)
        private static int DecimalsOf(string format)
        {
            if (string.IsNullOrEmpty(format)) return -1;
            if (format == "0") return 0;
            if (format.StartsWith("0.", StringComparison.Ordinal) && format.Substring(2).All(ch => ch == '0'))
                return format.Length - 2;
            return -1;
        }

        private static Exception Unwrap(Exception ex)
        {
            while (ex is TargetInvocationException && ex.InnerException != null)
                ex = ex.InnerException;
            return ex;
        }

        private void ShowWarning(string message) => Dialogs.Warning(Grid.FindForm(), message);

        // Copies or reverts one cell; a broken rule (e.g. a duplicate designator) is reported and skipped
        private void Apply(Action action)
        {
            try { action(); }
            catch (Exception ex) when (Unwrap(ex) is ArgumentException rule)
            {
                ShowWarning(rule.Message);
            }
        }

        private void RevertCells(List<CellRef> targets)
        {
            if (targets.Count == 0 || Saved == null) return;
            Grid.EndEdit();
            foreach (var t in targets)
            {
                var row = _rows[t.Row];
                var saved = Saved(row);
                if (saved != null)
                    Apply(() => row.RevertFrom(saved, t.Prop));
            }
            RaiseChanged();
            Grid.Refresh();
        }

        private void TakeFromOther(List<CellRef> targets)
        {
            if (targets.Count == 0) return;
            Grid.EndEdit();
            // Counterparts first: copying the key column (the designator) would change the match
            var sources = targets.Select(t => Counterpart(_rows[t.Row])).ToList();
            for (int i = 0; i < targets.Count; i++)
            {
                var source = sources[i];
                if (source == null) continue;
                var row = _rows[targets[i].Row];
                string prop = targets[i].Prop;
                Apply(() => row.CopyFrom(source, prop));
            }
            RaiseChanged();
            Grid.Refresh();
        }
    }
}
