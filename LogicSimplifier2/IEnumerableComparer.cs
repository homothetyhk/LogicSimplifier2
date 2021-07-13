using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicSimplifier2
{
    public class IEnumerableComparer<T> : IComparer<IEnumerable<T>> where T : IComparable<T>
    {
        public int Compare(IEnumerable<T> x, IEnumerable<T> y)
        {
            var enumeratorX = x.GetEnumerator();
            var enumeratorY = y.GetEnumerator();

            while (true)
            {
                bool moveX = enumeratorX.MoveNext();
                bool moveY = enumeratorY.MoveNext();
                if (!moveX && !moveY) return 0;
                if (!moveX) return -1;
                if (!moveY) return 1;
                int i = enumeratorX.Current.CompareTo(enumeratorY.Current);
                if (i != 0) return i;
            }
        }
    }
}
