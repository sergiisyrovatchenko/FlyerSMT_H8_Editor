using System;
using System.IO;

namespace FlyerSMT_H8_Editor
{
    // One field of a fixed-size record: its place, and read / write / compare / copy. A layout is a list of
    // these, so a field is described once for every operation
    internal sealed class RecordField<R>
    {
        public Action<byte[], int, R> Read;          // (bytes, record start, record)
        public Action<byte[], int, R, R> Write;      // (bytes, record start, before, after): writes only if changed
        public Func<R, R, bool> Same;                 // same stored value?
        public Action<R, R> Copy;                     // (from, to)
        public bool Moves;                            // travels with the row when rows are swapped or reordered
    }

    internal static class RecordField
    {
        // ---- Field kinds ----

        // f32, compared by bits: -0.0 vs 0.0 or another NaN counts as a change
        public static RecordField<R> Float<R>(int at, Func<R, float> get, Action<R, float> set, bool moves = true) =>
            Make(at, get, set, moves, Formats.SameBits,
                (bytes, pos) => BitConverter.ToSingle(bytes, pos),
                (bytes, pos, v) => Buffer.BlockCopy(BitConverter.GetBytes(v), 0, bytes, pos, 4));

        // i32
        public static RecordField<R> Int32<R>(int at, Func<R, int> get, Action<R, int> set, bool moves = true) =>
            Make(at, get, set, moves, (a, b) => a == b,
                (bytes, pos) => BitConverter.ToInt32(bytes, pos),
                (bytes, pos, v) => Buffer.BlockCopy(BitConverter.GetBytes(v), 0, bytes, pos, 4));

        // u8 read as a number
        public static RecordField<R> Byte<R>(int at, Func<R, int> get, Action<R, int> set, bool moves = true) =>
            Make(at, get, set, moves, (a, b) => a == b,
                (bytes, pos) => bytes[pos],
                (bytes, pos, v) => bytes[pos] = (byte)v);

        // u8 flag: non-zero reads as on, on is written as 1
        public static RecordField<R> Flag<R>(int at, Func<R, bool> get, Action<R, bool> set, bool moves = true) =>
            Make(at, get, set, moves, (a, b) => a == b,
                (bytes, pos) => bytes[pos] != 0,
                (bytes, pos, v) => bytes[pos] = (byte)(v ? 1 : 0));

        // Some bits of a u8; the others (stale memory) are kept as in the file
        public static RecordField<R> Bits<R>(int at, int mask, Func<R, int> get, Action<R, int> set, bool moves = true) =>
            Make(at, get, set, moves, (a, b) => a == b,
                (bytes, pos) => bytes[pos] & mask,
                (bytes, pos, v) => bytes[pos] = (byte)((bytes[pos] & ~mask) | (v & mask)));

        // Latin-1 text in a fixed field, NUL-terminated unless full. Writing clears the field first and must
        // leave room for the NUL
        public static RecordField<R> Text<R>(int at, int length, Func<R, string> get, Action<R, string> set, bool moves = true) =>
            Make(at, get, set, moves, (a, b) => a == b,
                (bytes, pos) =>
                {
                    int nul = Array.IndexOf(bytes, (byte)0, pos, length);
                    return Formats.Latin1.GetString(bytes, pos, (nul < 0 ? pos + length : nul) - pos);
                },
                (bytes, pos, v) =>
                {
                    byte[] text = Formats.Latin1.GetBytes(v ?? "");
                    if (text.Length >= length)
                        throw new InvalidDataException($"\"{v}\" is too long (max {length - 1} characters).");
                    Array.Clear(bytes, pos, length);
                    Buffer.BlockCopy(text, 0, bytes, pos, text.Length);
                });

        private static RecordField<R> Make<R, V>(int at, Func<R, V> get, Action<R, V> set, bool moves,
            Func<V, V, bool> same, Func<byte[], int, V> read, Action<byte[], int, V> write)
        {
            return new RecordField<R>
            {
                Read = (bytes, start, r) => set(r, read(bytes, start + at)),
                Write = (bytes, start, before, after) => { if (!same(get(before), get(after))) write(bytes, start + at, get(after)); },
                Same = (a, b) => same(get(a), get(b)),
                Copy = (from, to) => set(to, get(from)),
                Moves = moves,
            };
        }

        // ---- Whole records ----

        public static void ReadAll<R>(RecordField<R>[] fields, byte[] bytes, int start, R r)
        {
            foreach (var f in fields) f.Read(bytes, start, r);
        }

        public static void WriteChanged<R>(RecordField<R>[] fields, byte[] bytes, int start, R before, R after)
        {
            foreach (var f in fields) f.Write(bytes, start, before, after);
        }

        public static bool SameAll<R>(RecordField<R>[] fields, R a, R b)
        {
            foreach (var f in fields)
                if (!f.Same(a, b)) return false;
            return true;
        }

        public static void CopyAll<R>(RecordField<R>[] fields, R from, R to)
        {
            foreach (var f in fields) f.Copy(from, to);
        }

        public static void CopyMoving<R>(RecordField<R>[] fields, R from, R to)
        {
            foreach (var f in fields)
                if (f.Moves) f.Copy(from, to);
        }

        public static void SwapMoving<R>(RecordField<R>[] fields, R a, R b) where R : new()
        {
            var t = new R();
            CopyMoving(fields, a, t);
            CopyMoving(fields, b, a);
            CopyMoving(fields, t, b);
        }
    }
}
