using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicSimplifier2
{
    /// <summary>
    /// Disjunctive Normal Form
    /// </summary>
    public class DNF
    {
        public List<ConjunctiveClause> Clauses = new();

        public DNF() { }

        public DNF(IEnumerable<ConjunctiveClause> data)
        {
            Clauses.AddRange(data);
        }

        public void Add(ConjunctiveClause cc)
        {
            Clauses.Add(cc);
        }

        public void Remove(ConjunctiveClause cc)
        {
            Clauses.Remove(cc);
        }

        public void AddAndRemoveSupersets(ConjunctiveClause cc)
        {
            Clauses.RemoveAll(s => s >= cc);
            Add(cc);
        }

        public string[] ToStringArray(LogicManager lm)
        {
            IEnumerable<IEnumerable<string>> terms = Clauses.Select(cc => lm.ConvertE(cc));
            switch (lm.termOrder)
            {
                case TermOrder.FrequencyAscending:
                    terms = terms.OrderBy(ie => ie.Select(t => lm.termIndex[t]), new IEnumerableComparer<int>());
                    break;
                case TermOrder.FrequencyDescending:
                    terms = terms.OrderBy(ie => ie.Select(t => -lm.termIndex[t]), new IEnumerableComparer<int>());
                    break;
                case TermOrder.Alphabetical:
                    terms = terms.OrderBy(ie => string.Join(" + ", ie));
                    break;
                case TermOrder.ReverseAlphabetical:
                    terms = terms.OrderByDescending(ie => string.Join(" + ", ie));
                    break;
            }
            return terms.Select(t => string.Join(" + ", t)).ToArray();
        }

    }
}
