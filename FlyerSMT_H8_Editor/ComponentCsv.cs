using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace FlyerSMT_H8_Editor
{
    // One component line of a pick-and-place CSV
    public sealed class CsvPart
    {
        public string Designator;
        public string Footprint;
        public float X, Y;          // Mid X / Mid Y, mm
        public string Layer;        // T or B (not kept in .H8)
        public float Rotation;      // degrees
        public string Comment;      // part value = the name of its feeder
    }

    // Component CSV as the PCB tool exports and FlyerSMT imports (Samples\*.csv):
    //   Designator,Footprint,Mid X,Mid Y,Ref X,Ref Y,Pad X,Pad Y,Layer,Rotation,Comment
    //   """"
    //   C3,C0805,23.750,30.660,T,90.000,100nF
    // The header names 11 columns, component lines carry 7 (no Ref / Pad). Numbers: 3 decimals, and zero
    // keeps its sign ("-0.000"). CR LF line ends; two empty lines at the end
    public static class ComponentCsv
    {
        private const string Header = "Designator,Footprint,Mid X,Mid Y,Ref X,Ref Y,Pad X,Pad Y,Layer,Rotation,Comment";

        // ---- Export ----

        // Components in placement order; the .H8 has no layer, so every line gets layer
        public static string Write(H8File file, string layer = "T")
        {
            var sb = new StringBuilder();
            sb.Append(Header).Append("\r\n");
            sb.Append("\"\"\"\"").Append("\r\n");
            foreach (var c in file.Components)
                sb.Append(string.Join(",", new[]
                {
                    Field(c.Designator), Field(c.Footprint), Number(c.X), Number(c.Y), layer, Number(c.Rotation), Field(c.Value),
                })).Append("\r\n");
            sb.Append("\r\n\r\n");
            return sb.ToString();
        }

        public static void Save(H8File file, string path, string layer = "T") =>
            File.WriteAllBytes(path, Formats.Latin1.GetBytes(Write(file, layer)));

        // "12.570", "-27.230", and "-0.000" for a negative zero, as the PCB tool writes them
        private static string Number(float v)
        {
            string text = Math.Abs(v).ToString("0.000", Formats.Inv);
            return v < 0 || Formats.IsNegativeZero(v) ? "-" + text : text;
        }

        // Quoted only when it has to be
        private static string Field(string s)
        {
            s = s ?? "";
            return s.IndexOfAny(new[] { ',', '"' }) < 0 ? s : "\"" + s.Replace("\"", "\"\"") + "\"";
        }

        // ---- Import ----

        public static List<CsvPart> Load(string path) => Read(Formats.Latin1.GetString(File.ReadAllBytes(path)));

        // Reads component lines, skipping the header, """" and empty lines; 7 fields or the header's full set.
        // Any problem is an InvalidDataException naming the line, and nothing is read
        public static List<CsvPart> Read(string text)
        {
            var parts = new List<CsvPart>();
            string[] header = null;
            var seen = new HashSet<string>();
            string[] lines = text.Replace("\r\n", "\n").Split('\n');
            for (int n = 0; n < lines.Length; n++)
            {
                string line = lines[n];
                if (line.Trim().Length == 0 || line.Trim() == "\"\"\"\"") continue;
                var f = Split(line);
                if (header == null && f[0].Trim().Equals("Designator", StringComparison.OrdinalIgnoreCase))
                {
                    header = f.Select(h => h.Trim()).ToArray();
                    continue;
                }
                try
                {
                    var part = header != null && f.Count == header.Length && f.Count != 7
                        ? ByName(f, header)
                        : f.Count == 7 ? ByPosition(f) : null;
                    if (part == null)
                        throw new ArgumentException($"expected 7 fields (Designator, Footprint, Mid X, Mid Y, Layer, Rotation, Comment), found {f.Count}.");
                    if (part.Designator == "")
                        throw new ArgumentException("the designator is empty.");
                    if (!seen.Add(part.Designator))
                        throw new ArgumentException($"designator {part.Designator} appears twice.");
                    parts.Add(part);
                }
                catch (ArgumentException ex)
                {
                    throw new InvalidDataException($"Line {n + 1}: {ex.Message}");
                }
            }
            if (parts.Count == 0)
                throw new InvalidDataException("No component lines found.");
            return parts;
        }

        private static CsvPart ByPosition(List<string> f) => Make(f[0], f[1], f[2], f[3], f[4], f[5], f[6]);

        private static CsvPart ByName(List<string> f, string[] header)
        {
            string Column(string name)
            {
                int i = Array.FindIndex(header, h => h.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (i < 0) throw new ArgumentException($"the header has no \"{name}\" column.");
                return f[i];
            }
            return Make(Column("Designator"), Column("Footprint"), Column("Mid X"), Column("Mid Y"), Column("Layer"),
                Column("Rotation"), Column("Comment"));
        }

        private static CsvPart Make(string designator, string footprint, string x, string y, string layer, string rotation, string comment)
        {
            layer = layer.Trim().ToUpperInvariant();
            if (layer == "TOP") layer = "T";
            if (layer == "BOTTOM") layer = "B";
            if (layer != "T" && layer != "B" && layer != "")
                throw new ArgumentException($"layer \"{layer}\" is not T or B.");
            return new CsvPart
            {
                Designator = H8File.CheckText(designator, "Designator"),
                Footprint = H8File.CheckText(footprint, "Footprint"),
                X = Parse(x, "Mid X"),
                Y = Parse(y, "Mid Y"),
                Layer = layer,
                Rotation = Parse(rotation, "Rotation"),
                Comment = H8File.CheckText(comment, "Comment"),
            };
        }

        private static float Parse(string s, string what)
        {
            s = s.Trim();
            if (!float.TryParse(s, NumberStyles.Float, Formats.Inv, out float v))
                throw new ArgumentException($"{what} \"{s}\" is not a number.");
            // .NET Framework reads "-0.000" as +0; the files keep the sign (FlyerSMT stores -0 for it)
            if (v == 0 && s.StartsWith("-", StringComparison.Ordinal))
                v = -0f;
            return v;
        }

        // Comma-separated fields; a field may be in double quotes, with "" for a quote inside
        private static List<string> Split(string line)
        {
            var fields = new List<string>();
            var sb = new StringBuilder();
            bool quoted = false;
            for (int i = 0; i < line.Length; i++)
            {
                char ch = line[i];
                if (quoted)
                {
                    if (ch == '"' && i + 1 < line.Length && line[i + 1] == '"') { sb.Append('"'); i++; }
                    else if (ch == '"') quoted = false;
                    else sb.Append(ch);
                }
                else if (ch == '"') quoted = true;
                else if (ch == ',') { fields.Add(sb.ToString()); sb.Clear(); }
                else sb.Append(ch);
            }
            fields.Add(sb.ToString());
            return fields;
        }
    }
}
