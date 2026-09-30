using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FlyerSMT_H8_Editor
{
    // Component placement record, 140 bytes (layout: Fields)
    public sealed class H8Component
    {
        public const int StrLen = 40;
        public const int NewRecord = -1;

        // Record layout; reading, saving, change detection and moving rows all use it
        // moves: false = stays with its record when rows move
        internal static readonly RecordField<H8Component>[] Fields =
        {
            RecordField.Text<H8Component>(0, StrLen, c => c.Designator, (c, v) => c.Designator = v),
            RecordField.Text<H8Component>(40, StrLen, c => c.Footprint, (c, v) => c.Footprint = v),
            RecordField.Text<H8Component>(80, StrLen, c => c.Value, (c, v) => c.Value = v),
            RecordField.Float<H8Component>(120, c => c.X, (c, v) => c.X = v),                        // mm
            RecordField.Float<H8Component>(124, c => c.Y, (c, v) => c.Y = v),                        // mm
            RecordField.Float<H8Component>(128, c => c.Rotation, (c, v) => c.Rotation = v),          // degrees
            RecordField.Int32<H8Component>(132, c => c.FeederIndex, (c, v) => c.FeederIndex = v),    // see H8File.FeederOf
            RecordField.Int32<H8Component>(136, c => c.Flag, (c, v) => c.Flag = v, moves: false),    // unknown, 0/1 per file
        };

        public int Index { get; set; }  // placement order (NO. - 1); renumbered after a delete
        public int Offset { get; set; } // the record's place in the parsed bytes (fixed);
                                        // NewRecord: added by import, written from scratch
        public string Designator { get; set; }
        public string Footprint { get; set; }
        public string Value { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Rotation { get; set; }
        public int FeederIndex { get; set; } // 0-based slot in the feeder table (see H8File.FeederOf for "no feeder")
        public int Flag { get; set; }

        // Swaps what the Components tab shows (not the record, not the flag): "move down" swaps two rows' values
        public static void SwapValues(H8Component a, H8Component b) => RecordField.SwapMoving(Fields, a, b);
    }

    // The head's nozzles a feeder allows (N1 / N2 on the Feeders tab); stored as the low bits of a byte
    [Flags]
    public enum Nozzles
    {
        None = 0,
        N1 = 1,
        N2 = 2,
        Both = N1 | N2,
    }

    // Feeder slot, 384 bytes (layout: Fields); bytes past ~188 are stale memory, not read
    public sealed class H8Feeder
    {
        public const int ValueLen = 48;
        public const int PackageLen = 60;

        // Slot layout; reading, saving, change detection and swapping slots all use it
        internal static readonly RecordField<H8Feeder>[] Fields =
        {
            RecordField.Float<H8Feeder>(0, f => f.X, (f, v) => f.X = v),                              // coordinate X
            RecordField.Float<H8Feeder>(4, f => f.Y, (f, v) => f.Y = v),                              // coordinate Y
            RecordField.Float<H8Feeder>(8, f => f.NozzleHeight, (f, v) => f.NozzleHeight = v),
            RecordField.Float<H8Feeder>(12, f => f.Angle, (f, v) => f.Angle = v),
            RecordField.Float<H8Feeder>(16, f => f.Thickness, (f, v) => f.Thickness = v),
            RecordField.Float<H8Feeder>(20, f => f.HoleY, (f, v) => f.HoleY = v),                     // hole / offset Y
            RecordField.Float<H8Feeder>(24, f => f.HoleX, (f, v) => f.HoleX = v),                     // hole / offset X
            RecordField.Float<H8Feeder>(28, f => f.SizeW, (f, v) => f.SizeW = v),                     // part size W
            RecordField.Float<H8Feeder>(32, f => f.SizeH, (f, v) => f.SizeH = v),                     // part size H
            RecordField.Float<H8Feeder>(36, f => f.Distance, (f, v) => f.Distance = v),               // tape pitch
            RecordField.Text<H8Feeder>(40, ValueLen, f => f.Value, (f, v) => f.Value = v),
            RecordField.Text<H8Feeder>(88, PackageLen, f => f.Package, (f, v) => f.Package = v),
            RecordField.Flag<H8Feeder>(148, f => f.Enabled, (f, v) => f.Enabled = v),
            RecordField.Bits<H8Feeder>(152, (int)Nozzles.Both, f => f.NozzleMask, (f, v) => f.NozzleMask = v), // upper bits junk
            RecordField.Int32<H8Feeder>(156, f => f.TakeDown, (f, v) => f.TakeDown = v),              // take speed down
            RecordField.Int32<H8Feeder>(160, f => f.Threshold, (f, v) => f.Threshold = v),            // vision threshold
            RecordField.Byte<H8Feeder>(164, f => f.VisionType, (f, v) => f.VisionType = v),
            RecordField.Flag<H8Feeder>(168, f => f.Vision, (f, v) => f.Vision = v),                   // vision on/off
            RecordField.Int32<H8Feeder>(172, f => f.TakeUp, (f, v) => f.TakeUp = v),                  // take speed up
            RecordField.Int32<H8Feeder>(176, f => f.PasteDown, (f, v) => f.PasteDown = v),            // paste speed down
            RecordField.Int32<H8Feeder>(180, f => f.PasteUp, (f, v) => f.PasteUp = v),                // paste speed up
        };

        // Order of the "Type" drop-down in FlyerSMT's Feeder tab
        public static readonly string[] VisionTypeNames =
            { "NULL", "1feet", "2feet", "3feet", "TwoRowIC", "FourRowIC", "BGA", "High LED", "Currency" };

        // "FD007": a slot's number as FlyerSMT shows it (index is 0-based)
        public static string NumberOf(int index) => "FD" + (index + 1).ToString("000");

        public int Index { get; set; }
        public int Offset { get; set; }
        public string Number => NumberOf(Index);
        public bool Enabled { get; set; }
        public string Value { get; set; }
        public string Package { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float HoleX { get; set; }
        public float HoleY { get; set; }
        public float Angle { get; set; }
        public float NozzleHeight { get; set; }
        public float Thickness { get; set; }
        public float Distance { get; set; }
        public float SizeW { get; set; }
        public float SizeH { get; set; }
        public int Threshold { get; set; }
        public bool Vision { get; set; }
        public int VisionType { get; set; }
        public int NozzleMask { get; set; } // Nozzles bits
        public int TakeDown { get; set; }
        public int TakeUp { get; set; }
        public int PasteDown { get; set; }
        public int PasteUp { get; set; }

        public bool Allows(Nozzles nozzle) => (NozzleMask & (int)nozzle) != 0;

        public void Allow(Nozzles nozzle, bool allowed) =>
            NozzleMask = allowed ? NozzleMask | (int)nozzle : NozzleMask & ~(int)nozzle;

        public string VisionTypeName =>
            VisionType >= 0 && VisionType < VisionTypeNames.Length ? VisionTypeNames[VisionType] : VisionType.ToString();
    }

    // Panel board (FlyerSMT's PCB tab): where the job is placed, and whether at all.
    // Stored column-wise after the feeder table (H8File.Pcb* offsets)
    public sealed class H8Pcb
    {
        public int Index { get; set; }  // 0-based; NO. = Index + 1
        public float X { get; set; }
        public float Y { get; set; }
        public float A { get; set; }    // rotation, degrees
        public bool On { get; set; }
    }

    public sealed class H8File
    {
        public const int HeaderLen = 11;
        public const int ComponentSize = 140;
        public const int FeederSize = 384;
        public const int FeederCount = 100;

        // Block after the feeder table, offsets from its start:
        // 0      i32 number of boards (PCB tab rows)
        // 4..23  5 x f32 (12 = PcbHeight; the rest not decoded)
        // 24     u8[50000]  ON flags (1 = on)
        // 50024  f32[50000] X    250024 f32[50000] Y    450024 f32[50000] A
        // Past the board count: stale data from earlier jobs
        public const int PcbMax = 50000;
        private const int PcbOnAt = 24;
        private const int PcbXAt = PcbOnAt + PcbMax;
        private const int PcbYAt = PcbXAt + 4 * PcbMax;
        private const int PcbAAt = PcbYAt + 4 * PcbMax;
        private const int PcbBlockLen = PcbAAt + 4 * PcbMax;

        // Longest text accepted: component fields hold 39 characters + NUL, and a feeder's value and package
        // are copied into them, so feeders get the same limit
        public const int MaxText = H8Component.StrLen - 1;

        // Trims text and rejects what the file cannot hold: over MaxText characters, or anything outside
        // printable Latin-1 (Cyrillic would silently become '?')
        public static string CheckText(string value, string what)
        {
            value = (value ?? "").Trim();
            if (value.Length > MaxText)
                throw new ArgumentException($"{what} \"{value}\" is too long: {value.Length} characters, max {MaxText}.");
            foreach (char ch in value)
                if (!IsAllowedChar(ch))
                    throw new ArgumentException($"{what} \"{value}\" contains '{ch}': only Latin characters (Latin-1) are allowed.");
            return value;
        }

        public static bool IsAllowedChar(char ch) => ch >= 0x20 && ch <= 0xFF && !(ch >= 0x7F && ch < 0xA0);

        public string Path { get; private set; }
        public List<H8Component> Components { get; private set; }
        public List<H8Feeder> Feeders { get; private set; }
        public List<H8Pcb> Pcbs { get; private set; }

        private int PcbBlockStart { get; set; }

        // Board surface height: f32 at +12 of the block after the feeder table (11.4 in every job seen).
        // FlyerSMT shows Z = PcbHeight - feeder thickness
        public float PcbHeight { get; private set; }

        public static H8File Parse(byte[] bytes, string path)
        {
            if (bytes.Length < HeaderLen + 4)
                throw new InvalidDataException("File is too short to be a FlyerSMT .H8 file.");

            // Header: machine profile (printable ASCII, e.g. "ZB3245TSB") + CR LF
            bool headerOk = bytes[HeaderLen - 2] == '\r' && bytes[HeaderLen - 1] == '\n';
            for (int i = 0; i < HeaderLen - 2 && headerOk; i++)
                headerOk = bytes[i] >= 0x20 && bytes[i] < 0x7F;
            if (!headerOk)
                throw new InvalidDataException($"Not a FlyerSMT .H8 file: expected a {HeaderLen - 2}-character machine profile " +
                                               "followed by a line break at the start of the file.");

            // Unsigned count: checked against the file size before it becomes an int
            uint rawCount = BitConverter.ToUInt32(bytes, HeaderLen);
            long maxCount = (bytes.Length - HeaderLen - 4 - (long)FeederCount * FeederSize) / ComponentSize;
            if (rawCount > maxCount)
                throw new InvalidDataException($"Header says {rawCount} components, but the file is too short for that plus the feeder table.");
            int count = (int)rawCount;
            long feederStart = HeaderLen + 4 + (long)count * ComponentSize;

            var components = new List<H8Component>(count);
            for (int i = 0; i < count; i++)
            {
                var c = new H8Component { Index = i, Offset = HeaderLen + 4 + i * ComponentSize };
                RecordField.ReadAll(H8Component.Fields, bytes, c.Offset, c);
                components.Add(c);
            }

            var feeders = new List<H8Feeder>(FeederCount);
            for (int i = 0; i < FeederCount; i++)
            {
                var f = new H8Feeder { Index = i, Offset = (int)feederStart + i * FeederSize };
                RecordField.ReadAll(H8Feeder.Fields, bytes, f.Offset, f);
                feeders.Add(f);
            }

            int afterFeeders = (int)feederStart + FeederCount * FeederSize;
            float pcbHeight = afterFeeders + 16 <= bytes.Length ? FloatAt(bytes, afterFeeders + 12) : float.NaN;

            // Boards only if the whole block is there and the count is sane; otherwise the PCB tab stays empty
            var pcbs = new List<H8Pcb>();
            if ((long)afterFeeders + PcbBlockLen <= bytes.Length)
            {
                int pcbCount = BitConverter.ToInt32(bytes, afterFeeders);
                for (int i = 0; i < pcbCount && pcbCount <= PcbMax; i++)
                    pcbs.Add(new H8Pcb
                    {
                        Index = i,
                        On = bytes[afterFeeders + PcbOnAt + i] != 0,
                        X = FloatAt(bytes, afterFeeders + PcbXAt + 4 * i),
                        Y = FloatAt(bytes, afterFeeders + PcbYAt + 4 * i),
                        A = FloatAt(bytes, afterFeeders + PcbAAt + 4 * i),
                    });
            }

            return new H8File
            {
                Path = path, Components = components, Feeders = feeders, PcbHeight = pcbHeight,
                Pcbs = pcbs, PcbBlockStart = afterFeeders,
            };
        }

        // ---------------------------------------------------------------------------------------------
        // Relationships. Feeders are identified by slot ("FD007 - 100nF"), components by designator.
        // A component's Value is its feeder's name and its Footprint the feeder's Package; all edits go
        // through these methods, so the two tables never drift apart
        //
        // As in FlyerSMT, a component is on a feeder only if the slot is valid AND its Value is that feeder's
        // name; otherwise it has none (FlyerSMT shows NULL) and is not placed. "No feeder" is written as slot 0
        // + "-OFF-" (NullValue), as FlyerSMT writes slot 0 + "---"; slot -1 makes FlyerSMT show garbage and is
        // never written. Any value other than the feeder's name reads as no feeder, so "---" works too
        // ---------------------------------------------------------------------------------------------

        public const string NullValue = "-OFF-";

        public H8Feeder FeederOf(H8Component c)
        {
            if (c.FeederIndex < 0 || c.FeederIndex >= Feeders.Count) return null;
            var f = Feeders[c.FeederIndex];
            return c.Value == f.Value ? f : null;
        }

        public List<H8Component> ComponentsOn(H8Feeder f) => Components.Where(c => FeederOf(c) == f).ToList();

        // Renames a feeder; its components take the new name as value (and so stay on it)
        public void SetFeederValue(H8Feeder f, string value)
        {
            var on = ComponentsOn(f); // matched by the old name
            f.Value = CheckText(value, "Value");
            foreach (var c in on)
                c.Value = f.Value;
        }

        // New package for a feeder; its components take it as footprint
        public void SetFeederPackage(H8Feeder f, string package)
        {
            f.Package = CheckText(package, "Package");
            foreach (var c in ComponentsOn(f))
                c.Footprint = f.Package;
        }

        // Puts a component on a feeder, taking its value and package. -1 = no feeder: slot 0, "-OFF-",
        // footprint kept
        public void AssignFeeder(H8Component c, int feederIndex)
        {
            if (feederIndex < 0 || feederIndex >= Feeders.Count)
            {
                c.FeederIndex = 0;
                c.Value = NullValue;
                return;
            }
            var f = Feeders[feederIndex];
            c.FeederIndex = feederIndex;
            c.Value = f.Value;
            c.Footprint = f.Package;
        }

        // Swaps two slots except their numbers: the reel and its settings move, and its components follow
        // (their value and footprint still match)
        public void SwapFeeders(H8Feeder a, H8Feeder b)
        {
            if (a == b) return;
            var onA = ComponentsOn(a); // before the names move
            var onB = ComponentsOn(b);
            RecordField.SwapMoving(H8Feeder.Fields, a, b);
            foreach (var c in onA) c.FeederIndex = b.Index;
            foreach (var c in onB) c.FeederIndex = a.Index;
        }

        // New placement order (every component once). As with Move up / down, records stay and values move,
        // so changed cells show as edited until saved
        public void Reorder(IList<H8Component> order)
        {
            if (order.Count != Components.Count || order.Distinct().Count() != Components.Count)
                throw new ArgumentException("A new order must list every component once.");
            var values = order.Select(c =>
            {
                var copy = new H8Component();
                RecordField.CopyMoving(H8Component.Fields, c, copy);
                return copy;
            }).ToList();
            for (int i = 0; i < Components.Count; i++)
                RecordField.CopyMoving(H8Component.Fields, values[i], Components[i]);
            RefreshUsage();
        }

        // ---------------------------------------------------------------------------------------------
        // Undo: a copy of everything that can be edited, and going back to it
        // ---------------------------------------------------------------------------------------------

        // Components (with their records, so deletes undo too), feeders and boards
        public H8File Snapshot()
        {
            return new H8File
            {
                Path = Path, PcbHeight = PcbHeight, PcbBlockStart = PcbBlockStart,
                Components = Components.Select(CopyOf).ToList(),
                Feeders = Feeders.Select(f =>
                {
                    var copy = new H8Feeder { Index = f.Index, Offset = f.Offset };
                    RecordField.CopyAll(H8Feeder.Fields, f, copy);
                    return copy;
                }).ToList(),
                Pcbs = Pcbs.Select(p => new H8Pcb { Index = p.Index, X = p.X, Y = p.Y, A = p.A, On = p.On }).ToList(),
            };
        }

        // Back to a snapshot. Feeders and boards keep their objects (grid rows stay valid); the component list
        // is rebuilt (a delete changes it), so the owner rebinds
        public void Restore(H8File snapshot)
        {
            Components = snapshot.Components.Select(CopyOf).ToList();
            for (int i = 0; i < Feeders.Count; i++)
                RecordField.CopyAll(H8Feeder.Fields, snapshot.Feeders[i], Feeders[i]);
            for (int i = 0; i < Pcbs.Count; i++)
            {
                H8Pcb from = snapshot.Pcbs[i], to = Pcbs[i];
                to.X = from.X; to.Y = from.Y; to.A = from.A; to.On = from.On;
            }
            RefreshUsage();
        }

        // Same values and same records: going from one to the other changes nothing
        public bool SameStateAs(H8File other) =>
            SameContentAs(other) && Components.Select(c => c.Offset).SequenceEqual(other.Components.Select(c => c.Offset));

        private static H8Component CopyOf(H8Component c)
        {
            var copy = new H8Component { Index = c.Index, Offset = c.Offset };
            RecordField.CopyAll(H8Component.Fields, c, copy);
            return copy;
        }

        // What an import did, for the status bar
        public sealed class ImportResult
        {
            public int Count;                                   // components now in the job
            public int Kept, Added, Removed;                    // records kept (same designator), new, dropped
            public List<string> WithoutFeeder = new List<string>();   // no feeder named like their comment
            public List<string> FootprintFromFeeder = new List<string>(); // feeder found by name, other package
        }

        // Replaces the components with parts, in their order (import). Same designator: keeps its record;
        // others get a new one; the rest are dropped. Feeder: the one named like the comment (the current one
        // if it still fits, else same package, enabled, lowest slot); the part takes its package. No such
        // feeder: none (FlyerSMT skips the part), and the comment is kept as its value
        public ImportResult ImportComponents(IList<CsvPart> parts)
        {
            var result = new ImportResult { Count = parts.Count };
            var existing = Components.GroupBy(c => c.Designator).ToDictionary(g => g.Key, g => g.First());
            int flag = Components.Count == 0 ? 0 : Components.GroupBy(c => c.Flag).OrderByDescending(g => g.Count()).First().Key;

            var list = new List<H8Component>();
            foreach (var p in parts)
            {
                int slot;
                bool kept = existing.TryGetValue(p.Designator, out var c);
                if (kept)
                {
                    var on = FeederOf(c);
                    slot = on != null && on.Value == p.Comment ? on.Index : FeederNamed(p.Comment, p.Footprint);
                    existing.Remove(p.Designator);
                    result.Kept++;
                }
                else
                {
                    c = new H8Component { Offset = H8Component.NewRecord, Flag = flag };
                    slot = FeederNamed(p.Comment, p.Footprint);
                    result.Added++;
                }
                c.Designator = p.Designator;
                // Kept components keep their exact number when the CSV shows the same 3 decimals: the file holds
                // e.g. 30.6599979, the CSV "30.660", which parses to a slightly different float
                c.X = kept && SameAt3(c.X, p.X) ? c.X : p.X;
                c.Y = kept && SameAt3(c.Y, p.Y) ? c.Y : p.Y;
                c.Rotation = kept && SameAt3(c.Rotation, p.Rotation) ? c.Rotation : p.Rotation;
                c.Footprint = p.Footprint;
                if (slot >= 0)
                {
                    AssignFeeder(c, slot);
                    if (c.Footprint != p.Footprint) result.FootprintFromFeeder.Add(p.Designator);
                }
                else
                {
                    // Slot 0 with a value that is not slot 0's name: no feeder (see FeederOf)
                    c.FeederIndex = 0;
                    c.Value = p.Comment == Feeders[0].Value ? NullValue : p.Comment;
                    result.WithoutFeeder.Add(p.Designator);
                }
                list.Add(c);
            }
            result.Removed = existing.Count;

            Components = list;
            for (int i = 0; i < Components.Count; i++)
                Components[i].Index = i;
            RefreshUsage();
            return result;
        }

        private static bool SameAt3(float a, float b) => a.ToString("0.000", Formats.Inv) == b.ToString("0.000", Formats.Inv);

        // The feeder named value: same package first, then enabled, then lowest slot; -1 if none
        private int FeederNamed(string value, string package)
        {
            if (string.IsNullOrEmpty(value)) return -1;
            var f = Feeders.Where(x => x.Value == value)
                .OrderByDescending(x => x.Package == package)
                .ThenByDescending(x => x.Enabled)
                .ThenBy(x => x.Index)
                .FirstOrDefault();
            return f?.Index ?? -1;
        }

        // Removes components; the rest close up and are renumbered. Serialize cuts their records out
        public void DeleteComponents(IEnumerable<H8Component> doomed)
        {
            var set = new HashSet<H8Component>(doomed);
            Components.RemoveAll(set.Contains);
            for (int i = 0; i < Components.Count; i++)
                Components[i].Index = i;
            RefreshUsage();
        }

        // Renames a component; designators are unique within a file
        public void SetDesignator(H8Component c, string designator)
        {
            designator = CheckText(designator, "Designator");
            if (designator == "")
                throw new ArgumentException("A designator cannot be empty.");
            if (Components.Any(x => x != c && x.Designator == designator))
                throw new ArgumentException($"Designator \"{designator}\" already exists in this file.");
            c.Designator = designator;
        }

        // ---------------------------------------------------------------------------------------------
        // Fast queries used on every edit
        // ---------------------------------------------------------------------------------------------

        private Dictionary<int, List<string>> _usage;
        private Dictionary<string, H8Component> _byDesignator;
        private PathPlan _plan;

        // Simulated placement (cycles, nozzles, path length) from the last RefreshUsage
        public PathPlan Plan => _plan ??= PathPlan.Build(this);

        // Per-edit snapshot (the owner calls it): designators per feeder, component per designator and the
        // simulated placement, so painting and comparing read dictionaries instead of scanning per cell
        public void RefreshUsage()
        {
            _plan = PathPlan.Build(this);
            _usage = new Dictionary<int, List<string>>();
            _byDesignator = new Dictionary<string, H8Component>();
            foreach (var c in Components)
            {
                if (!_byDesignator.ContainsKey(c.Designator))
                    _byDesignator[c.Designator] = c; // a repeated designator matches its first component
                if (FeederOf(c) == null) continue; // no feeder (NULL): not counted on the slot it points at
                if (!_usage.TryGetValue(c.FeederIndex, out var list))
                    _usage[c.FeederIndex] = list = new List<string>();
                list.Add(c.Designator);
            }
        }

        private static readonly List<string> NoDesignators = new List<string>();

        // Designators on a feeder, from the last RefreshUsage snapshot
        public List<string> DesignatorsOn(int feederIndex)
        {
            if (_usage == null) RefreshUsage();
            return _usage.TryGetValue(feederIndex, out var list) ? list : NoDesignators;
        }

        // Component with this designator, from the last RefreshUsage snapshot (null if none)
        public H8Component FindComponent(string designator)
        {
            if (_byDesignator == null) RefreshUsage();
            return designator != null && _byDesignator.TryGetValue(designator, out var c) ? c : null;
        }

        // True when every editable field equals saved's, i.e. Serialize would reproduce the saved bytes;
        // far cheaper than serializing
        public bool SameContentAs(H8File saved)
        {
            if (saved == null || saved.Components.Count != Components.Count || saved.Feeders.Count != Feeders.Count
                || saved.Pcbs.Count != Pcbs.Count)
                return false;
            for (int i = 0; i < Components.Count; i++)
                if (!RecordField.SameAll(H8Component.Fields, saved.Components[i], Components[i]))
                    return false;
            for (int i = 0; i < Feeders.Count; i++)
                if (!RecordField.SameAll(H8Feeder.Fields, saved.Feeders[i], Feeders[i]))
                    return false;
            for (int i = 0; i < Pcbs.Count; i++)
            {
                H8Pcb a = saved.Pcbs[i], b = Pcbs[i];
                if (a.On != b.On || !Formats.SameBits(a.X, b.X) || !Formats.SameBits(a.Y, b.Y) || !Formats.SameBits(a.A, b.A))
                    return false;
            }
            return true;
        }

        // baseline (the bytes this file was parsed from) with the current values written in, only the fields
        // that changed, so an unedited file saves to the same bytes. When the component records changed
        // (delete, import), their block is rebuilt: header count, kept records copied from where they were,
        // new ones from zeros, everything after them moved unchanged
        public byte[] Serialize(byte[] baseline, H8File parsedBaseline = null)
        {
            var orig = parsedBaseline ?? Parse(baseline, Path);
            var origByOffset = orig.Components.ToDictionary(c => c.Offset);
            const int recordsAt = HeaderLen + 4;
            int shift = (Components.Count - orig.Components.Count) * ComponentSize;
            bool sameRecords = shift == 0;
            for (int i = 0; i < Components.Count && sameRecords; i++)
                sameRecords = Components[i].Offset == recordsAt + i * ComponentSize;

            byte[] bytes;
            if (sameRecords)
                bytes = (byte[])baseline.Clone();
            else
            {
                int oldTail = recordsAt + orig.Components.Count * ComponentSize;
                int newTail = recordsAt + Components.Count * ComponentSize;
                bytes = new byte[baseline.Length + shift];
                Buffer.BlockCopy(baseline, 0, bytes, 0, HeaderLen);
                Buffer.BlockCopy(BitConverter.GetBytes((uint)Components.Count), 0, bytes, HeaderLen, 4);
                for (int i = 0; i < Components.Count; i++)
                    if (Components[i].Offset != H8Component.NewRecord)
                        Buffer.BlockCopy(baseline, Components[i].Offset, bytes, recordsAt + i * ComponentSize, ComponentSize);
                Buffer.BlockCopy(baseline, oldTail, bytes, newTail, baseline.Length - oldTail);
            }

            var blank = new H8Component { Designator = "", Footprint = "", Value = "" }; // what a new record holds
            for (int i = 0; i < Components.Count; i++)
            {
                var before = origByOffset.TryGetValue(Components[i].Offset, out var parsed) ? parsed : blank;
                RecordField.WriteChanged(H8Component.Fields, bytes, recordsAt + i * ComponentSize, before, Components[i]);
            }
            for (int i = 0; i < Feeders.Count; i++)
                RecordField.WriteChanged(H8Feeder.Fields, bytes, Feeders[i].Offset + shift, orig.Feeders[i], Feeders[i]);
            for (int i = 0; i < Pcbs.Count; i++)
            {
                H8Pcb a = orig.Pcbs[i], b = Pcbs[i];
                int block = PcbBlockStart + shift;
                if (a.On != b.On) bytes[block + PcbOnAt + i] = (byte)(b.On ? 1 : 0);
                PutFloat(bytes, block + PcbXAt + 4 * i, a.X, b.X);
                PutFloat(bytes, block + PcbYAt + 4 * i, a.Y, b.Y);
                PutFloat(bytes, block + PcbAAt + 4 * i, a.A, b.A);
            }
            return bytes;
        }

        private static void PutFloat(byte[] bytes, int offset, float before, float after)
        {
            if (!Formats.SameBits(before, after))
                Buffer.BlockCopy(BitConverter.GetBytes(after), 0, bytes, offset, 4);
        }

        private static float FloatAt(byte[] bytes, int offset) => BitConverter.ToSingle(bytes, offset);
    }
}
