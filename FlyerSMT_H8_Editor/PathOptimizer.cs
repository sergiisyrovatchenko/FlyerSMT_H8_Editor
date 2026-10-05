using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FlyerSMT_H8_Editor
{
    // Finds a placement order with a shorter head path for the feeders as set, measured with PathSim.
    // Only placed components move, among their own rows. Local search (swap, move, reverse), then small
    // shake-ups of the best order until the budget runs out or nothing improves for "stall". Fixed seed;
    // never longer than the current order
    public static class PathOptimizer
    {
        public sealed class Result
        {
            public List<H8Component> Order; // all components, in the new order
            public double Before;           // path length of the current order, mm
            public double After;            // path length of the new order, mm
            public long Evaluations;        // orders whose length was computed (a speed measure)
        }

        // Stops at budget, or once stall passes without a shorter order (null: no early stop)
        public static Result Optimize(H8File file, TimeSpan budget, TimeSpan? stall = null)
        {
            var placed = new List<H8Component>();
            var slots = new List<int>(); // rows the placed components occupy
            var items = PathSim.ItemsOf(file, placed, slots);

            int[] order = Enumerable.Range(0, items.Length).ToArray();
            double before = Length(items, order);
            var best = (int[])order.Clone();
            double bestLen = before;

            if (items.Length > 1)
            {
                var clock = new SearchClock(budget, stall ?? budget, before);
                var random = new Random(1);
                var current = (int[])best.Clone();
                double currentLen = Improve(items, current, bestLen, clock);
                if (currentLen < bestLen) { bestLen = currentLen; best = (int[])current.Clone(); }

                // Shake the best order a little and descend again, while there is time
                while (!clock.Up)
                {
                    var trial = (int[])best.Clone();
                    Perturb(trial, random);
                    double trialLen = Improve(items, trial, Length(items, trial), clock);
                    if (trialLen < bestLen - 1e-9) { bestLen = trialLen; best = trial; }
                }
            }

            var result = file.Components.ToList();
            long evaluations = _evaluations;
            _evaluations = 0;
            for (int k = 0; k < slots.Count; k++)
                result[slots[k]] = placed[best[k]];
            return new Result { Order = result, Before = before, After = bestLen, Evaluations = evaluations };
        }

        // The budget, cut short when no shorter order is found for "stall"
        private sealed class SearchClock
        {
            private readonly Stopwatch _watch = Stopwatch.StartNew();
            private readonly TimeSpan _budget, _stall;
            private TimeSpan _lastGain;
            private double _best;

            public SearchClock(TimeSpan budget, TimeSpan stall, double start)
            {
                _budget = budget;
                _stall = stall;
                _best = start;
            }

            public bool Up
            {
                get
                {
                    var now = _watch.Elapsed;
                    return now >= _budget || now - _lastGain >= _stall;
                }
            }

            // Only a new overall best restarts the stall timer
            public void Found(double length)
            {
                if (length < _best - 1e-9)
                {
                    _best = length;
                    _lastGain = _watch.Elapsed;
                }
            }
        }

        // First-improvement local search: swap two, move one elsewhere, reverse a run; until nothing helps.
        // A changed order is measured from its first changed place on (Length with from), with the simulation
        // state saved before every place of the current order; an accepted change saves the states anew from there
        private static double Improve(PathSim.Item[] items, int[] a, double len, SearchClock clock)
        {
            int n = a.Length;
            var states = new PathSim.State[n + 1];
            PathSim.Run(items, a, 0, PathSim.State.Initial, states);

            bool improved = true;
            while (improved && !clock.Up)
            {
                improved = false;
                for (int i = 0; i < n - 1 && !clock.Up; i++)
                    for (int j = i + 1; j < n; j++)
                    {
                        Swap(a, i, j);
                        double l = Length(items, a, i, states);
                        if (l < len - 1e-9) { len = l; improved = true; clock.Found(l); Keep(items, a, i, states); }
                        else Swap(a, i, j);
                    }
                for (int i = 0; i < n && !clock.Up; i++)
                    for (int j = 0; j < n; j++)
                    {
                        if (j == i || j == i - 1) continue;
                        int from = Math.Min(i, j);
                        Move(a, i, j);
                        double l = Length(items, a, from, states);
                        if (l < len - 1e-9) { len = l; improved = true; clock.Found(l); Keep(items, a, from, states); }
                        else Move(a, j, i); // back where it was
                    }
                for (int i = 0; i < n - 2 && !clock.Up; i++)
                    for (int j = i + 2; j < n; j++)
                    {
                        Array.Reverse(a, i, j - i + 1);
                        double l = Length(items, a, i, states);
                        if (l < len - 1e-9) { len = l; improved = true; clock.Found(l); Keep(items, a, i, states); }
                        else Array.Reverse(a, i, j - i + 1);
                    }
            }
            return len;
        }

        // A few random swaps and one random move
        private static void Perturb(int[] a, Random random)
        {
            int n = a.Length;
            int swaps = 1 + random.Next(Math.Min(3, n));
            for (int s = 0; s < swaps; s++)
                Swap(a, random.Next(n), random.Next(n));
            int from = random.Next(n), to = random.Next(n);
            if (from != to) Move(a, from, to);
        }

        private static void Swap(int[] a, int i, int j)
        {
            int t = a[i]; a[i] = a[j]; a[j] = t;
        }

        // Moves the element at "from" to "to" (index in the result); Move(a, to, from) undoes it
        private static void Move(int[] a, int from, int to)
        {
            int v = a[from];
            if (from < to) Array.Copy(a, from + 1, a, from, to - from);
            else Array.Copy(a, to, a, to + 1, from - to);
            a[to] = v;
        }

        [ThreadStatic] private static long _evaluations;

        // Head travel for this order
        private static double Length(PathSim.Item[] items, int[] order)
        {
            _evaluations++;
            return PathSim.Run(items, order);
        }

        // Head travel for an order that matches the one states was saved for up to position from
        private static double Length(PathSim.Item[] items, int[] order, int from, PathSim.State[] states)
        {
            _evaluations++;
            return PathSim.Run(items, order, from, states[from], null);
        }

        // The order changed from position from on: the states saved from there on are refreshed
        private static void Keep(PathSim.Item[] items, int[] order, int from, PathSim.State[] states)
        {
            PathSim.Run(items, order, from, states[from], states);
        }
    }
}
