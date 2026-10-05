using System.Collections.Generic;

namespace FlyerSMT_H8_Editor
{
    // One head operation: pick a part from its feeder, or place it on the board
    public sealed class PathOp
    {
        public int Cycle;           // pick-and-place iteration this operation belongs to
        public bool IsPick;
        public int Nozzle;          // 1 or 2
        public H8Component Component;
    }

    // One iteration "pick -> (pick) -> place -> (place)"
    public sealed class PathCycle
    {
        public int No;
        public List<PathOp> Ops = new List<PathOp>();
    }

    // Simulated pick-and-place (rules in PathSim): cycles, each component's pick and the head travel
    public sealed class PathPlan
    {
        public List<PathCycle> Cycles = new List<PathCycle>();
        public double Length { get; private set; } // head travel, mm
        public int Placed { get; private set; }

        private readonly Dictionary<H8Component, PathOp> _picks = new Dictionary<H8Component, PathOp>();

        // A component's pick (cycle, nozzle), or null if it is not placed
        public PathOp PickOf(H8Component c) => c != null && _picks.TryGetValue(c, out var op) ? op : null;

        // Nozzles a feeder allows: its N1 / N2 bits; with neither ticked, nozzle 1
        public static int AllowedNozzles(H8Feeder f)
        {
            int mask = f.NozzleMask & (int)Nozzles.Both;
            return mask == 0 ? (int)Nozzles.N1 : mask;
        }

        public static PathPlan Build(H8File file)
        {
            var plan = new PathPlan();
            if (file == null) return plan;

            var placed = new List<H8Component>();
            var items = PathSim.ItemsOf(file, placed, null);
            var order = new int[items.Length];
            for (int k = 0; k < order.Length; k++) order[k] = k;

            plan.Length = PathSim.Run(items, order, (k, isPick, nozzle, cycle) =>
            {
                if (cycle > plan.Cycles.Count)
                    plan.Cycles.Add(new PathCycle { No = cycle });
                var op = new PathOp { Cycle = cycle, IsPick = isPick, Nozzle = nozzle, Component = placed[k] };
                plan.Cycles[cycle - 1].Ops.Add(op);
                if (isPick) plan._picks[placed[k]] = op;
                else plan.Placed++;
            });
            return plan;
        }
    }

    // FlyerSMT's two-nozzle pick-and-place on plain numbers, so the optimizer can run it millions of times;
    // the only place the rules live. Components go in placement order (No.), each picked with the first
    // free nozzle its feeder allows, N1 first. When no allowed nozzle is free, everything held is placed
    // (in pick order) and a new cycle starts; what is held at the end is placed. No feeder: not placed.
    // Travel counts from the first pick
    internal static class PathSim
    {
        // One placed component, reduced to what the path depends on
        internal struct Item
        {
            public double Fx, Fy;   // feeder (pick)
            public double Cx, Cy;   // component (place)
            public int Mask;        // allowed Nozzles bits (PathPlan.AllowedNozzles)
        }

        // Told about every operation: item index, pick or place, nozzle, cycle number (from 1)
        internal delegate void Visit(int item, bool isPick, int nozzle, int cycle);

        // Placed components (with a feeder) in placement order; placed gets them, rows (optional) their rows
        public static Item[] ItemsOf(H8File file, List<H8Component> placed, List<int> rows)
        {
            var items = new List<Item>();
            for (int i = 0; i < file.Components.Count; i++)
            {
                var c = file.Components[i];
                var f = file.FeederOf(c);
                if (f == null) continue; // no feeder: cannot be picked
                placed.Add(c);
                if (rows != null) rows.Add(i);
                items.Add(new Item { Fx = f.X, Fy = f.Y, Cx = c.X, Cy = c.Y, Mask = PathPlan.AllowedNozzles(f) });
            }
            return items.ToArray();
        }

        // Where the simulation stands before a component: everything carried from one component to the next.
        // The optimizer keeps one per position, so a change in the order is measured from the first changed place
        internal struct State
        {
            public double Total, X, Y;          // travel so far, head position
            public bool Started;                // the head has made its first stop
            public int Held0, Held1;            // held items in pick order; -1 = none
            public int Nozzle0, Nozzle1;        // their nozzles
            public int Cycle;

            public static State Initial => new State { Held0 = -1, Held1 = -1 };
        }

        // Head travel for this order, mm. visit null = length only (the optimizer)
        public static double Run(Item[] items, int[] order, Visit visit = null) => Run(items, order, 0, State.Initial, null, visit);

        // Head travel for this order, going on from state, the state before order[from]. states (optional, one more
        // than the order) receives the state before every position from "from" on. The result is the same, bit for
        // bit, as a run from the start: the same additions in the same order. No helper calls: it runs millions of times
        public static double Run(Item[] items, int[] order, int from, State state, State[] states, Visit visit = null)
        {
            const int N1 = (int)Nozzles.N1, N2 = (int)Nozzles.N2;
            double total = state.Total, px = state.X, py = state.Y, dx, dy;
            bool started = state.Started;
            int held0 = state.Held0, held1 = state.Held1;
            int nozzle0 = state.Nozzle0, nozzle1 = state.Nozzle1;
            int cycle = state.Cycle;

            // Each stop below: dx = x - px; dy = y - py; if (started) total += sqrt(..): travel from the first stop

            for (int n = from; n <= order.Length; n++)
            {
                if (states != null)
                    states[n] = new State
                    {
                        Total = total, X = px, Y = py, Started = started,
                        Held0 = held0, Held1 = held1, Nozzle0 = nozzle0, Nozzle1 = nozzle1, Cycle = cycle,
                    };
                bool end = n == order.Length;
                int k = end ? -1 : order[n];
                int mask = end ? 0 : items[k].Mask;
                bool busy1 = (held0 >= 0 && nozzle0 == 1) || (held1 >= 0 && nozzle1 == 1);
                bool busy2 = (held0 >= 0 && nozzle0 == 2) || (held1 >= 0 && nozzle1 == 2);
                int nozzle = (mask & N1) != 0 && !busy1 ? 1 : (mask & N2) != 0 && !busy2 ? 2 : 0;

                // Place what is held: at the end, or when no allowed nozzle is free (a new cycle starts)
                if (end || (cycle > 0 && nozzle == 0))
                {
                    if (held0 >= 0)
                    {
                        dx = items[held0].Cx - px; dy = items[held0].Cy - py;
                        if (started) total += System.Math.Sqrt(dx * dx + dy * dy);
                        px = items[held0].Cx; py = items[held0].Cy; started = true;
                        visit?.Invoke(held0, false, nozzle0, cycle);
                    }
                    if (held1 >= 0)
                    {
                        dx = items[held1].Cx - px; dy = items[held1].Cy - py;
                        if (started) total += System.Math.Sqrt(dx * dx + dy * dy);
                        px = items[held1].Cx; py = items[held1].Cy; started = true;
                        visit?.Invoke(held1, false, nozzle1, cycle);
                    }
                    held0 = held1 = -1;
                    if (end) break;
                    cycle++;
                }
                if (cycle == 0) cycle = 1;
                if (nozzle == 0)
                    nozzle = (mask & N1) != 0 ? 1 : 2;

                // Pick
                dx = items[k].Fx - px; dy = items[k].Fy - py;
                if (started) total += System.Math.Sqrt(dx * dx + dy * dy);
                px = items[k].Fx; py = items[k].Fy; started = true;
                visit?.Invoke(k, true, nozzle, cycle);
                if (held0 < 0) { held0 = k; nozzle0 = nozzle; }
                else { held1 = k; nozzle1 = nozzle; }
            }
            return total;
        }
    }
}
