using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace FlyerSMT_H8_Editor
{
    // Main window: layout in MainForm.Designer.cs (Visual Studio designer), wiring of the two files here
    public partial class MainForm : Form
    {
        private readonly FileSlot[] _slots;

        // For the designer
        public MainForm() : this(null)
        {
        }

        public MainForm(string[] initialFiles)
        {
            InitializeComponent();

            _slots = new[]
            {
                new FileSlot(this, 1, buttonOpen1, buttonSave1, buttonOptimize1, buttonRevert1, buttonRestore1, buttonExport1, buttonImport1, textPath1,
                    tabPcb1, tabFeeders1, tabComponents1, pcbView1, feedersView1, componentsView1),
                new FileSlot(this, 2, buttonOpen2, buttonSave2, buttonOptimize2, buttonRevert2, buttonRestore2, buttonExport2, buttonImport2, textPath2,
                    tabPcb2, tabFeeders2, tabComponents2, pcbView2, feedersView2, componentsView2),
            };

            for (int i = 0; i < _slots.Length && initialFiles != null && i < initialFiles.Length; i++)
                if (File.Exists(initialFiles[i]))
                    _slots[i].Load(initialFiles[i], render: false);
            RenderAll();
        }

        // Leaving a tab commits a half-typed cell, so the edit counts without Enter
        private void Tabs_Deselecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPage == null) return;
            foreach (var grid in GridsIn(e.TabPage))
                grid.EndEdit();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_slots == null) return;
            var dirty = _slots.Where(x => x.IsDirty).Select(x => "file " + x.Number).ToArray();
            if (dirty.Length > 0 && !Dialogs.Confirm(this, $"Unsaved changes in {string.Join(" and ", dirty)}. Close without saving?"))
                e.Cancel = true;
        }

        // All grids inside a control, however deep (they sit in user controls)
        private static IEnumerable<DataGridView> GridsIn(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is DataGridView grid) yield return grid;
                foreach (var inner in GridsIn(c)) yield return inner;
            }
        }

        private FileSlot Other(FileSlot slot) => slot == _slots[0] ? _slots[1] : _slots[0];

        // Rebinds all grids (after open, revert, save, delete, undo, import)
        private void RenderAll()
        {
            foreach (var slot in _slots)
                slot.Render(Other(slot));
            RefreshDiffs();
        }

        // Recomputes highlights, dirty state and undo steps without rebinding (keeps selection and scroll)
        private void RefreshDiffs()
        {
            // One usage snapshot per file per edit; the grids read it for every cell
            foreach (var slot in _slots)
                if (slot.File != null) slot.File.RefreshUsage();
            foreach (var slot in _slots)
                slot.RefreshDiffs(Other(slot).File);
            foreach (var slot in _slots)
                slot.RecordStep();
            UpdatePathStatus();
        }

        // Ctrl+Z / Ctrl+Y (Ctrl+Shift+Z): undo / redo in the file whose tab is showing. While a cell is being
        // typed in, the keys stay with the text box
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            bool undo = keyData == (Keys.Control | Keys.Z);
            bool redo = keyData == (Keys.Control | Keys.Y) || keyData == (Keys.Control | Keys.Shift | Keys.Z);
            if ((undo || redo) && _slots != null)
            {
                var slot = _slots.FirstOrDefault(x => x.Owns(tabs.SelectedTab));
                if (slot != null && !slot.IsEditingCell)
                {
                    if (undo) slot.Undo(); else slot.Redo();
                    return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        // Right-hand status: "Path 1: … mm (-3.2 %) · Path 2: … · +12.3 %". In brackets, while a file has
        // unsaved edits: its path against the saved file (minus = shorter). At the end: path 2 against path 1
        private void UpdatePathStatus()
        {
            var parts = new List<string>();
            var plans = new PathPlan[_slots.Length];
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i].File == null) continue;
                plans[i] = _slots[i].File.Plan; // simulated in RefreshUsage just before
                string text = $"Path {_slots[i].Number}: {plans[i].Placed} placements, {plans[i].Cycles.Count} cycles, " +
                              $"{Formats.Mm(plans[i].Length)} mm";
                var saved = _slots[i].SavedPlan;
                if (_slots[i].IsDirty && saved != null && saved.Length > 0)
                    text += $" ({Percent(plans[i].Length, saved.Length)})";
                parts.Add(text);
            }
            if (plans[0] != null && plans[1] != null && plans[0].Length > 0)
                parts.Add(Percent(plans[1].Length, plans[0].Length));
            statusPath.Text = string.Join("  ·  ", parts.ToArray());
        }

        // "+1.5 %" / "-3.2 %": value against reference
        private static string Percent(double value, double reference)
        {
            double pct = (value - reference) / reference * 100;
            return (pct >= 0 ? "+" : "") + pct.ToString("0.0", Formats.Inv) + " %";
        }

        // One file: its buttons, path box and tabs (from the designer), plus the bytes it was read from
        private sealed class FileSlot
        {
            private readonly MainForm _owner;
            private byte[] _baseline;  // bytes as read from disk (or as last saved)
            private H8File _savedFile; // _baseline parsed: the reference for the pink "edited" highlight

            public int Number { get; private set; }
            public H8File File { get; private set; }
            public bool IsDirty { get; private set; }

            // Undo: the state after each step (last = current) and the undone states. Restarts on open, save, revert
            private readonly List<H8File> _undo = new List<H8File>();
            private readonly List<H8File> _redo = new List<H8File>();
            private const int UndoSteps = 200;

            // The saved file's path: the reference for the path change in the status bar
            public PathPlan SavedPlan => _savedFile?.Plan;

            public readonly Button OpenButton;
            public readonly Button SaveButton;
            public readonly Button OptimizeButton;
            public readonly Button RevertButton;
            public readonly Button RestoreButton;
            public readonly Button ExportButton;
            public readonly Button ImportButton;
            public readonly TextBox PathBox;
            public readonly TabPage PcbPage;
            public readonly TabPage FeederPage;
            public readonly TabPage ComponentPage;
            public readonly PcbView BoardView;
            public readonly FeedersView FeederView;
            public readonly ComponentsView ComponentView;
            public readonly CompareGrid Pcbs;
            public readonly CompareGrid Feeders;
            public readonly CompareGrid Components;

            public FileSlot(MainForm owner, int number,
                Button open, Button save, Button optimize, Button revert, Button restore, Button export, Button import, TextBox pathBox,
                TabPage pcbPage, TabPage feederPage, TabPage componentPage,
                PcbView boardView, FeedersView feederView, ComponentsView componentView)
            {
                _owner = owner;
                Number = number;
                OpenButton = open;
                SaveButton = save;
                OptimizeButton = optimize;
                RevertButton = revert;
                RestoreButton = restore;
                ExportButton = export;
                ImportButton = import;
                PathBox = pathBox;
                PcbPage = pcbPage;
                FeederPage = feederPage;
                ComponentPage = componentPage;
                BoardView = boardView;
                FeederView = feederView;
                ComponentView = componentView;

                Pcbs = CreatePcbGrid();
                Feeders = CreateFeederGrid();
                Components = CreateComponentGrid();

                foreach (var g in AllGrids)
                {
                    g.OtherName = () => "file " + _owner.Other(this).Number;
                    g.Changed += () => _owner.RefreshDiffs();
                    g.Grid.AllowDrop = true;
                    g.Grid.DragEnter += (s, e) => e.Effect = e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop)
                        ? DragDropEffects.Copy : DragDropEffects.None;
                    g.Grid.DragDrop += (s, e) =>
                    {
                        var files = e.Data == null ? null : e.Data.GetData(DataFormats.FileDrop) as string[];
                        if (files != null && files.Length > 0 && ConfirmDiscard())
                            Load(files[0], render: true);
                    };
                }

                OpenButton.Click += (s, e) => Browse();
                SaveButton.Click += (s, e) => Save();
                OptimizeButton.Click += (s, e) => Optimize();
                RevertButton.Click += (s, e) => Revert();
                RestoreButton.Click += (s, e) => Restore();
                ExportButton.Click += (s, e) => Export();
                ImportButton.Click += (s, e) => Import();
            }

            // Boards match by number (board 3 here against board 3 there)
            private CompareGrid CreatePcbGrid()
            {
                var g = BoardView.Compare;
                g.Counterpart = row =>
                {
                    var other = _owner.Other(this).File;
                    int i = ((PcbRow)row).Pcb.Index;
                    return other == null || i >= other.Pcbs.Count ? null : new PcbRow(other.Pcbs[i]);
                };
                g.Saved = row =>
                {
                    int i = ((PcbRow)row).Pcb.Index;
                    return _savedFile == null || i >= _savedFile.Pcbs.Count ? null : new PcbRow(_savedFile.Pcbs[i]);
                };
                // No SwapData: board order is fixed, so no Move up / Move down
                return g;
            }

            private CompareGrid CreateFeederGrid()
            {
                var g = FeederView.Compare;
                g.Counterpart = row =>
                {
                    var other = _owner.Other(this).File;
                    int i = ((FeederRow)row).Feeder.Index;
                    return other == null || i >= other.Feeders.Count ? null : new FeederRow(other.Feeders[i], other, File);
                };
                g.Saved = row =>
                {
                    if (_savedFile == null) return null;
                    int i = ((FeederRow)row).Feeder.Index;
                    return new FeederRow(_savedFile.Feeders[i], _savedFile, _owner.Other(this).File);
                };
                g.SwapData = (a, b) => File.SwapFeeders(((FeederRow)a).Feeder, ((FeederRow)b).Feeder);
                g.RowColor = row =>
                {
                    var r = (FeederRow)row;
                    if (!r.Enabled) return Theme.DisabledColor; // off: grey, used or not
                    if (r.UsedCount == 0) return null;
                    return r.OtherUsedCount > 0 ? Theme.BothUsedColor : Theme.UsedColor;
                };
                return g;
            }

            private CompareGrid CreateComponentGrid()
            {
                var g = ComponentView.Compare;
                g.Saved = row =>
                {
                    if (_savedFile == null || File == null) return null;
                    // The same record in the saved file (by its place in the file: numbers shift after a delete)
                    int offset = ((ComponentRow)row).Component.Offset;
                    var saved = _savedFile.Components.FirstOrDefault(c => c.Offset == offset);
                    return saved == null ? null : new ComponentRow(saved, _savedFile);
                };
                g.SwapData = (a, b) => H8Component.SwapValues(((ComponentRow)a).Component, ((ComponentRow)b).Component);
                // Grey when its feeder is switched off, as on the Feeders tab
                g.RowColor = row => ((ComponentRow)row).OnDisabledFeeder ? Theme.DisabledColor : (System.Drawing.Color?)null;
                // A delete changes the rows and the other file's "missing" marks: rebind everything
                g.DeleteData = rows =>
                {
                    File.DeleteComponents(rows.Select(r => ((ComponentRow)r).Component));
                    _owner.RenderAll();
                };
                // Components match by designator: the two files can be different boards or sides
                g.Counterpart = row =>
                {
                    var other = _owner.Other(this).File;
                    if (other == null) return null;
                    var c = other.FindComponent(((ComponentRow)row).Designator);
                    return c == null ? null : new ComponentRow(c, other);
                };
                return g;
            }

            private CompareGrid[] AllGrids => new[] { Pcbs, Feeders, Components };

            public bool Owns(TabPage page) => page == PcbPage || page == FeederPage || page == ComponentPage;

            public bool IsEditingCell => AllGrids.Any(g => g.Grid.IsCurrentCellInEditMode);

            // Commits a half-typed cell in every grid; false if one was rejected (the cell stays open)
            private bool CommitEdits() => AllGrids.All(g => g.Grid.EndEdit());

            private void ResetHistory()
            {
                _undo.Clear();
                _redo.Clear();
                if (File != null) _undo.Add(File.Snapshot());
            }

            // After every refresh: a change since the last step becomes a new step (edit, move, delete, take-over,
            // revert of cells, optimize, import)
            public void RecordStep()
            {
                if (File == null) return;
                if (_undo.Count > 0 && File.SameStateAs(_undo[_undo.Count - 1])) return;
                _undo.Add(File.Snapshot());
                if (_undo.Count > UndoSteps + 1) _undo.RemoveAt(0);
                _redo.Clear();
            }

            public void Undo()
            {
                if (File == null || _undo.Count < 2)
                {
                    _owner.statusMain.Text = $"Undo {Number}: nothing to undo";
                    return;
                }
                _redo.Add(_undo[_undo.Count - 1]);
                _undo.RemoveAt(_undo.Count - 1);
                GoTo(_undo[_undo.Count - 1]);
                _owner.statusMain.Text = $"Undo {Number}: {_undo.Count - 1} more step(s) to undo, {_redo.Count} to redo";
            }

            public void Redo()
            {
                if (File == null || _redo.Count == 0)
                {
                    _owner.statusMain.Text = $"Redo {Number}: nothing to redo";
                    return;
                }
                var state = _redo[_redo.Count - 1];
                _redo.RemoveAt(_redo.Count - 1);
                _undo.Add(state);
                GoTo(state);
                _owner.statusMain.Text = $"Redo {Number}: {_undo.Count - 1} step(s) to undo, {_redo.Count} more to redo";
            }

            // Puts the file into a recorded state and rebinds, keeping each grid's place
            private void GoTo(H8File state)
            {
                var grids = AllGrids;
                var places = grids.Select(g => g.Position()).ToArray();
                File.Restore(state);
                _owner.RenderAll(); // records no step: the file now equals the last state
                for (int i = 0; i < grids.Length; i++)
                    grids[i].RestorePosition(places[i]);
            }

            private bool ConfirmDiscard() => !IsDirty || Dialogs.Confirm(_owner, $"File {Number} has unsaved changes. Discard them?");

            private void Browse()
            {
                if (!ConfirmDiscard()) return;
                using (var dlg = new OpenFileDialog())
                {
                    dlg.Title = $"Select .H8 file {Number}";
                    dlg.Filter = "FlyerSMT files (*.H8)|*.H8|All files (*.*)|*.*";
                    var hint = File ?? _owner.Other(this).File;
                    if (hint != null)
                        dlg.InitialDirectory = Path.GetDirectoryName(hint.Path);
                    if (dlg.ShowDialog(_owner) == DialogResult.OK)
                        Load(dlg.FileName, render: true);
                }
            }

            public void Load(string path, bool render)
            {
                try
                {
                    var bytes = System.IO.File.ReadAllBytes(path);
                    File = H8File.Parse(bytes, path);
                    _baseline = bytes;
                    _savedFile = H8File.Parse(bytes, path);
                    ResetHistory();
                    PathBox.Text = path;
                    _owner.tabs.SelectedTab = FeederPage;
                }
                catch (Exception ex)
                {
                    Dialogs.Error(_owner, "Could not read this file:\n" + ex.Message);
                    return;
                }
                if (render)
                    _owner.RenderAll();
            }

            private void Revert()
            {
                if (File == null || _baseline == null) return;
                File = H8File.Parse(_baseline, File.Path);
                ResetHistory();
                _owner.RenderAll();
            }

            // Writes "<file>.tmp", flushes it and swaps it in: a crash mid-write leaves the old or the new file,
            // never half of one
            private static void WriteSafely(string path, byte[] bytes)
            {
                string tmp = path + ".tmp";
                using (var fs = new FileStream(tmp, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Flush(true);
                }
                try
                {
                    if (!System.IO.File.Exists(path))
                        System.IO.File.Move(tmp, path);
                    else
                        System.IO.File.Replace(tmp, path, null, true);
                }
                catch (PlatformNotSupportedException)
                {
                    // No Replace on this file system (FAT, some network shares): plain copy
                    System.IO.File.Copy(tmp, path, true);
                    System.IO.File.Delete(tmp);
                }
                catch
                {
                    try { System.IO.File.Delete(tmp); } catch { }
                    throw;
                }
            }

            // The file as it was before FlyerSMT H8 Editor first saved over it
            private string BackupPath => File == null ? null : File.Path + ".bak";

            // Puts the .bak back over the file and reloads it
            private void Restore()
            {
                if (File == null || !System.IO.File.Exists(BackupPath)) return;
                string path = File.Path;
                if (!Dialogs.Confirm(_owner,
                        $"Restore {Path.GetFileName(path)} from {Path.GetFileName(BackupPath)}?\n\n" +
                        $"The file goes back to how it was before it was first saved from {AppInfo.Title}. " +
                        "All changes saved since then, and any unsaved changes, are lost.", defaultNo: true))
                    return;
                try
                {
                    WriteSafely(path, System.IO.File.ReadAllBytes(BackupPath));
                }
                catch (Exception ex)
                {
                    Dialogs.Error(_owner, $"Could not restore file {Number}:\n{ex.Message}");
                    return;
                }
                Load(path, render: true);
            }

            // Reorders the components for a shorter path with the feeders as set (PathOptimizer). Only the grid
            // changes; Save writes it
            private void Optimize()
            {
                if (File == null || !CommitEdits()) return;
                PathOptimizer.Result r;
                var cursor = Cursor.Current;
                Cursor.Current = Cursors.WaitCursor;
                // At most 1.5 s; stops once 0.5 s bring no shorter order
                try { r = PathOptimizer.Optimize(File, TimeSpan.FromSeconds(1.5), TimeSpan.FromSeconds(0.5)); }
                finally { Cursor.Current = cursor; }

                string title = $"Optimize {Number}: ";
                if (r.After < r.Before - 0.05) // a smaller gain would not show in the status bar
                {
                    File.Reorder(r.Order);
                    _owner.RefreshDiffs();
                    _owner.statusMain.Text = title + $"{Formats.Mm(r.Before)} → {Formats.Mm(r.After)} mm ({Percent(r.After, r.Before)})";
                }
                else
                    _owner.statusMain.Text = title + $"no shorter order found ({Formats.Mm(r.Before)} mm)";
            }

            // Writes the components, unsaved edits included, as a pick-and-place CSV
            private void Export()
            {
                if (File == null || !Components.Grid.EndEdit()) return;
                using (var dlg = new SaveFileDialog())
                {
                    dlg.Title = $"Export components of file {Number}";
                    dlg.Filter = "Component CSV (*.csv)|*.csv|All files (*.*)|*.*";
                    dlg.InitialDirectory = Path.GetDirectoryName(File.Path);
                    dlg.FileName = Path.GetFileNameWithoutExtension(File.Path) + ".csv";
                    if (dlg.ShowDialog(_owner) != DialogResult.OK) return;
                    try
                    {
                        ComponentCsv.Save(File, dlg.FileName);
                    }
                    catch (Exception ex)
                    {
                        Dialogs.Error(_owner, $"Could not export file {Number}:\n{ex.Message}");
                        return;
                    }
                    _owner.statusMain.Text = $"Export {Number}: {File.Components.Count} components to {Path.GetFileName(dlg.FileName)}";
                }
            }

            // Replaces the components with a CSV's (H8File.ImportComponents); only the grid changes until Save
            private void Import()
            {
                if (File == null || !CommitEdits()) return;
                using (var dlg = new OpenFileDialog())
                {
                    dlg.Title = $"Import components into file {Number}";
                    dlg.Filter = "Component CSV (*.csv)|*.csv|All files (*.*)|*.*";
                    dlg.InitialDirectory = Path.GetDirectoryName(File.Path);
                    if (dlg.ShowDialog(_owner) != DialogResult.OK) return;
                    H8File.ImportResult r;
                    try
                    {
                        r = File.ImportComponents(ComponentCsv.Load(dlg.FileName));
                    }
                    catch (Exception ex)
                    {
                        Dialogs.Error(_owner, $"Could not import {Path.GetFileName(dlg.FileName)}:\n{ex.Message}");
                        return;
                    }
                    _owner.RenderAll(); // the component list changed
                    _owner.statusMain.Text = ImportSummary(r, Path.GetFileName(dlg.FileName));
                }
            }

            private string ImportSummary(H8File.ImportResult r, string from)
            {
                var text = $"Import {Number} from {from}: {r.Count} components ({r.Kept} kept, {r.Added} new, {r.Removed} removed)";
                if (r.WithoutFeeder.Count > 0)
                    text += "; no feeder: " + FirstFew(r.WithoutFeeder);
                if (r.FootprintFromFeeder.Count > 0)
                    text += "; footprint taken from feeder: " + FirstFew(r.FootprintFromFeeder);
                return text;
            }

            // "R1, R2, …": at most 8 names, so the status bar stays readable
            private static string FirstFew(List<string> names)
            {
                const int max = 8;
                return string.Join(", ", names.Take(max).ToArray()) + (names.Count > max ? "…" : "");
            }

            private void Save()
            {
                if (File == null || !CommitEdits()) return;
                try
                {
                    byte[] bytes = File.Serialize(_baseline, _savedFile);
                    if (!System.IO.File.Exists(BackupPath))
                        WriteSafely(BackupPath, _baseline);
                    WriteSafely(File.Path, bytes);
                    _baseline = bytes;
                    File = H8File.Parse(bytes, File.Path);
                    _savedFile = H8File.Parse(bytes, File.Path);
                    ResetHistory(); // earlier states refer to the file as it was before this save
                }
                catch (Exception ex)
                {
                    Dialogs.Error(_owner, $"Could not save file {Number}:\n{ex.Message}");
                    return;
                }
                _owner.RenderAll();
            }

            // Keeps the Value drop-down ("FD007 - 100nF") in step with this file's feeders
            private void UpdateFeederChoices()
            {
                var col = ComponentView.ValueColumn;
                var items = ComponentRow.FeederChoices(File);
                if (col.Items.Cast<FeederChoice>().Select(c => c.Key).SequenceEqual(items.Select(c => c.Key))) return;
                col.Items.Clear();
                col.Items.AddRange(items.Cast<object>().ToArray());
            }

            public void Render(FileSlot otherSlot)
            {
                // Detach the old rows before rebuilding the drop-down, so no old row is shown against the new list
                Components.Bind(new List<ComponentRow>());
                UpdateFeederChoices();
                var other = otherSlot.File;
                FeederView.NameComponentColumns(Number, otherSlot.Number);
                if (File == null)
                {
                    Pcbs.Bind(new List<PcbRow>());
                    Feeders.Bind(new List<FeederRow>());
                    Components.Bind(new List<ComponentRow>());
                    return;
                }

                // All 100 slots, empty ones too, matching the Value drop-down
                var feederRows = File.Feeders.Select(f => new FeederRow(f, File, other)).ToList();
                Pcbs.Bind(File.Pcbs.Select(p => new PcbRow(p)).ToList());
                Feeders.Bind(feederRows);
                Components.Bind(File.Components.Select(c => new ComponentRow(c, File)).ToList());
            }

            public void RefreshDiffs(H8File other)
            {
                // Field by field against the saved file: no copy, re-parse and byte compare per edit
                IsDirty = File != null && !File.SameContentAs(_savedFile);
                UpdateFeederChoices();
                Pcbs.RefreshDiffs(other != null);
                Feeders.RefreshDiffs(other != null);
                Components.RefreshDiffs(other != null);

                SaveButton.Enabled = IsDirty;
                OptimizeButton.Enabled = File != null && File.Components.Count > 1;
                RevertButton.Enabled = IsDirty;
                RestoreButton.Enabled = File != null && System.IO.File.Exists(BackupPath);
                ExportButton.Enabled = ImportButton.Enabled = File != null;
                string name = IsDirty ? " *" : "";
                bool compared = File != null && other != null;
                PcbPage.Text = "PCB " + Number + name + (compared ? Counts(Pcbs) : "");
                FeederPage.Text = "Feeders " + Number + name + (compared ? Counts(Feeders) : "");
                ComponentPage.Text = "Components " + Number + name + (compared ? Counts(Components) : "");
            }

            // "  (2 diff, 1 missing)", non-zero counts only; nothing when the tab matches the other file
            private static string Counts(CompareGrid grid)
            {
                var parts = new List<string>();
                if (grid.DiffRows > 0) parts.Add(grid.DiffRows + " diff");
                if (grid.MissingRows > 0) parts.Add(grid.MissingRows + " missing");
                return parts.Count == 0 ? "" : "  (" + string.Join(", ", parts.ToArray()) + ")";
            }
        }
    }
}
