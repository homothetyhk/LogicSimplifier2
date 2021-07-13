using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicSimplifier2
{
    public enum TermOrder
    {
        Alphabetical,
        ReverseAlphabetical,
        FrequencyAscending,
        FrequencyDescending,
    }

    public class LogicManager
    {
        public Dictionary<string, int> termFrequencies = new();
        public readonly int termCount;
        public Dictionary<string, int> termIndex;
        public string[] terms;
        public Dictionary<string, DNF> relLocationLogic = new();
        public Dictionary<string, DNF> relWaypointLogic = new();
        public TermOrder termOrder = TermOrder.Alphabetical;

        public LogicManager(Dictionary<string, string[][]> locationLogic,
            Dictionary<string, string[][]> waypointLogic)
        {
            foreach (var (name, arr) in locationLogic)
            {
                foreach (string[] chain in arr)
                {
                    foreach (string term in chain)
                    {
                        if (termFrequencies.ContainsKey(term)) termFrequencies[term]++;
                        else termFrequencies[term] = 1;
                    }
                }
            }
            foreach (var (name, arr) in waypointLogic)
            {
                foreach (string[] chain in arr)
                {
                    foreach (string term in chain)
                    {
                        if (termFrequencies.ContainsKey(term)) termFrequencies[term]++;
                        else termFrequencies[term] = 1;
                    }
                }
            }
            termCount = termFrequencies.Count;
            terms = termFrequencies.Keys.OrderBy(k => termFrequencies[k]).ToArray();
            termIndex = terms.Select((t, i) => (t, i)).ToDictionary(p => p.t, p => p.i);
            Empty = new(new bool[termCount]);

            foreach (var (name, arr) in locationLogic)
            {
                List<bool[]> chains = new();
                foreach (string[] logic in arr)
                {
                    chains.Add(GetChain(logic));
                }
                this.relLocationLogic[name] = new DNF(chains.Select(c => new ConjunctiveClause(c)));
            }

            foreach (var (name, arr) in waypointLogic)
            {
                List<bool[]> chains = new();
                foreach (string[] logic in arr)
                {
                    chains.Add(GetChain(logic));
                }
                this.relWaypointLogic[name] = new DNF(chains.Select(c => new ConjunctiveClause(c)));
            }

        }

        private bool[] GetChain(string[] logic)
        {
            bool[] arr = new bool[termCount];
            foreach (string t in logic) arr[termIndex[t]] = true;
            return arr;
        }

        public WaypointSolver GetSolver()
        {
            Waypoint[] arr = relWaypointLogic.Select(kvp => new Waypoint
            {
                name = kvp.Key,
                index = termIndex[kvp.Key],
                relativeLogic = kvp.Value,
                absoluteLogic = new(),
            }).ToArray();

            return new WaypointSolver(this, arr);
        }

        public readonly ConjunctiveClause Empty;

        public void UpdateTermFrequenciesByDNF(Dictionary<string, DNF> dnfs)
        {
            termFrequencies = dnfs.ToDictionary(kvp => kvp.Key, kvp => 0);
            foreach (var (name, dnf) in dnfs)
            {
                for (int i = 0; i < termCount; i++)
                {
                    if (dnf.Clauses.Any(c => c.reqs[i]))
                    {
                        termFrequencies[name]++;
                    }
                }
            }
        }

        public string Convert(ConjunctiveClause cc)
        {
            IEnumerable<string> members = cc.reqs.GetTrueIndices().Select(i => terms[i]);
            OrderTerms(ref members);

            return string.Join(" + ", members);
        }

        public IEnumerable<string> ConvertE(ConjunctiveClause cc)
        {
            IEnumerable<string> members = cc.reqs.GetTrueIndices().Select(i => terms[i]);
            OrderTerms(ref members);
            return members;
        }

        public void OrderTerms(ref IEnumerable<string> terms)
        {
            switch (termOrder)
            {
                case TermOrder.FrequencyAscending:
                    terms.OrderBy(t => termFrequencies[t]);
                    break;
                case TermOrder.FrequencyDescending:
                    terms = terms.OrderByDescending(t => termFrequencies[t]);
                    break;
                case TermOrder.Alphabetical:
                    terms = terms.OrderBy(s => s);
                    break;
                case TermOrder.ReverseAlphabetical:
                    terms = terms.OrderByDescending(s => s);
                    break;
            }
        }

    }
}
