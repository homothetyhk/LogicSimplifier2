namespace LogicSimplifier2
{
    public class ConjunctiveClause
    {
        public readonly bool[] reqs;
        public ConjunctiveClause(bool[] reqs)
        {
            this.reqs = reqs;
        }


        public static bool operator <=(ConjunctiveClause left, ConjunctiveClause right)
        {
            for (int i = 0; i < left.reqs.Length; i++)
            {
                if (left.reqs[i] && !right.reqs[i]) return false;
            }
            return true;
        }

        public static bool operator >=(ConjunctiveClause left, ConjunctiveClause right)
        {
            for (int i = 0; i < left.reqs.Length; i++)
            {
                if (!left.reqs[i] && right.reqs[i]) return false;
            }
            return true;
        }

        public ConjunctiveClause Substitute(int waypoint, ConjunctiveClause value)
        {
            bool[] arr = new bool[reqs.Length];

            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = reqs[i] || value.reqs[i];
            }
            arr[waypoint] = false;

            return new ConjunctiveClause(arr);
        }
    }
}
