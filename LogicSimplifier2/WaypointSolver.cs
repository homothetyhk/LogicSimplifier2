using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace LogicSimplifier2
{
    public class WaypointSolver
    {
        readonly Queue<(ConjunctiveClause cc, Waypoint wp)> updates = new();
        readonly LogicManager lm;
        public readonly Waypoint[] Waypoints;
        Stopwatch solveWatch = new();
        Stopwatch substWatch = new();

        public WaypointSolver(LogicManager _lm, Waypoint[] _waypoints)
        {
            lm = _lm;
            Waypoints = _waypoints;
        }

        public void GiveWaypoint(string name)
        {
            GiveWaypoint(Waypoints.First(w => w.name == name));
        }

        public void GiveWaypoint(Waypoint waypoint)
        {
            waypoint.absoluteLogic.AddAndRemoveSupersets(lm.Empty);
            updates.Enqueue((lm.Empty, waypoint));
        }

        public void Solve()
        {
            Console.Clear();
            Console.CursorVisible = false;
            solveWatch.Start();
            while (updates.Any()) Step();
            solveWatch.Stop();
            Dictionary<string, string[]> statements = Waypoints
                .ToDictionary(w => w.name,
                w => w.absoluteLogic.ToStringArray(lm));
            statements.Serialize("waypoints.json");
        }

        public void Step()
        {
            var (cc, wp) = updates.Dequeue();
            UpdateConsoleWaypoint(wp.name);

            foreach (Waypoint to in Waypoints)
            {
                List<ConjunctiveClause> newRelativeStatements = null;
                foreach (ConjunctiveClause cj in to.relativeLogic.Clauses)
                {
                    if (cj.reqs[wp.index])
                    {
                        ConjunctiveClause newCJ = cj.Substitute(wp.index, cc);
                        if (IsRelative(newCJ))
                        {
                            if (to.relativeLogic.Clauses.Any(s => newCJ >= s))
                            {
                                continue;
                            }
                            newRelativeStatements ??= new();
                            newRelativeStatements.Add(newCJ);
                        }
                        else
                        {
                            if (to.absoluteLogic.Clauses.Any(s => newCJ >= s))
                            {
                                continue;
                            }
                            to.absoluteLogic.AddAndRemoveSupersets(newCJ);
                            updates.Enqueue((newCJ, to));
                        }
                    }
                }
                if (newRelativeStatements != null)
                {
                    foreach (var cj in newRelativeStatements)
                    {
                        if (to.relativeLogic.Clauses.Any(c => cj >= c)) continue;
                        to.relativeLogic.AddAndRemoveSupersets(cj);
                    }
                }
            }
        }

        public void ApplyToLocations(Dictionary<string, DNF> logic)
        {
            Console.Clear();
            substWatch.Start();
            Dictionary<string, DNF> newLogic = new();
            int i = logic.Count - 1;
            foreach (var (s, b) in logic)
            {
                UpdateConsoleLocation(s, i--);
                newLogic[s] = SubstAll(b);
            }

            ConsoleComplete();
            substWatch.Stop();

            newLogic
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToStringArray(lm))
                .Serialize("locations.json");
        }

        public DNF SubstAll(DNF logic)
        {
            DNF subst = new DNF();
            
            foreach (var cc in logic.Clauses)
            {
                (Waypoint, int)[] waypoints = Waypoints.Where(w => cc.reqs[w.index])
                    .Select((w, i) => (w, i)).ToArray();
                int count = waypoints.Length;
                if (count == 0)
                {
                    if (subst.Clauses.Any(c => cc >= c)) continue;
                    subst.AddAndRemoveSupersets(cc);
                    continue;
                }

                ConjunctiveClause[] statements = new ConjunctiveClause[count];
                statements[0] = cc;

                foreach (var (w, i) in waypoints)
                {
                    foreach (var cj in w.absoluteLogic.Clauses)
                    {
                        if (i + 1 < count)
                        {
                            statements[i + 1] = statements[i].Substitute(w.index, cj);
                        }
                        else
                        {
                            ConjunctiveClause cl = statements[i].Substitute(w.index, cj);
                            if (subst.Clauses.Any(c => cl >= c)) continue;
                            subst.AddAndRemoveSupersets(cl);
                        }
                    }
                }
            }

            return subst;
        }

        private bool IsRelative(ConjunctiveClause cc)
        {
            return Waypoints.Any(w => cc.reqs[w.index]);
        }

        public void UpdateConsoleWaypoint(string waypoint)
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine($"Solving waypoint logic... {solveWatch.Elapsed}".PadRight(Console.BufferWidth - 1));
            Console.WriteLine($"Current source: {waypoint}".PadRight(Console.BufferWidth - 1));
            Console.WriteLine($"Queued updates: {updates.Count}".PadRight(Console.BufferWidth - 1));
        }

        public void UpdateConsoleLocation(string location, int remaining)
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine($"Substituting in location logic... {substWatch.Elapsed}".PadRight(Console.BufferWidth - 1));
            Console.WriteLine($"Current location {location}".PadRight(Console.BufferWidth - 1));
            Console.WriteLine($"Remaining: {remaining}".PadRight(Console.BufferWidth - 1));
        }

        public void ConsoleComplete()
        {
            Console.Clear();
            Console.WriteLine("Finished substituting waypoint logic into locations!");
            Console.WriteLine($"Waypoint solve time: {solveWatch.Elapsed}");
            Console.WriteLine($"Location substitution time: {substWatch.Elapsed}");
        }

    }
}
