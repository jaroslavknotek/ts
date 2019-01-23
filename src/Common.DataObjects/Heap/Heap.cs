using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DataObjects.Heap
{
    /// <summary>
    /// The heap is maximum oriented. Keep in mind while inserting comparer as a parameter to constructor.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Heap<T>
    {
        private const string paramException = "Parameter must not be null";
        private readonly IComparer<T> comparer;
        Collection<T> items;

        public int Count
        {
            get { return items.Count; }
        }

        public bool IsEmpty
        {
            get
            {
                return items.Count == 0;
            }
        }



        public Heap(IComparer<T> comparer)
        {
            if (comparer == null) throw new ArgumentException(paramException, "comparer");
            this.comparer = comparer;
            items = new Collection<T>();
        }


        public void Insert(T obj)
        {
            items.Add(obj);
            int index = Count - 1;
            while (cmp(items[(index - 1) / 2], obj) && index != 0)
            {
                items[index] = items[(index - 1) / 2];
                index = (index - 1) / 2;
            }
            items[index] = obj;
        }


        public T Top()
        {
            if (IsEmpty) return default(T);

            return items[0];
        }

        public T Pop()
        {
            if (IsEmpty) return default(T);

            var r = Top();
            items[0] = items[items.Count - 1];
            items.RemoveAt(items.Count - 1);
            repairTop();
            return r;
        }

        private void repairTop()
        {
            if (items.Count <= 1) return;
            if (items.Count == 2)
            {
                if (cmp(items[0], items[1]))
                {
                    var t = items[0];
                    items[0] = items[1];
                    items[1] = t;
                }
                return;
            }

            int index = 0;
            var tmp = items[index];
            int next = 1;
            if (next < Count - 1 && cmp(items[next], items[next + 1]))
            {
                next++;
            }
            if (next >= Count - 1)//the case when it does not reach to loop code
            {
                items[index] = items[next];
                items[next] = tmp;
                return;
            }

            while (next < Count && cmp(tmp, items[next]))
            {
                items[index] = items[next];
                index = next;
                next = next * 2 + 1;
                if (next < Count - 1 && cmp(items[next], items[next + 1]))
                {
                    next++;
                }
            }
            items[index] = tmp;
        }
        /// <summary>
        /// true when the left is less than right (in comparer)
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <returns></returns>
        private bool cmp(T i1, T i2)
        {
            return (comparer.Compare(i1, i2) > 0);
        }
    }


}
