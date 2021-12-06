using System.Collections;
using System.Collections.Generic;

namespace DudCo.Events
{
    internal class PrioritisedList<T> : IEnumerable<KeyValuePair<int,List<T>>>
    {
        //similar concept to the contents and index sections of a book
        SortedDictionary<int, List<T>> contents = new SortedDictionary<int, List<T>>();
        Dictionary<T, (List<T>list, int priority)> indexes = new Dictionary<T, (List<T>,int)>();

        public void Add(T item, int priority)
        { 
            //invert so the dictionary is sorted the right way around
            priority *= -1;

            bool priorityAllreadyExists = contents.ContainsKey(priority);

            if (priorityAllreadyExists)
                contents[priority].Add(item);
            else
                CreateAndAddToNewItemsList(item, priority);

            var itemList = contents[priority];
            var index = (itemList, priority);
            indexes[item] = index;
        }

        private void CreateAndAddToNewItemsList(T item, int prio)
        {
            List<T> items = new List<T>();
            items.Add(item);
            contents.Add(prio, items);
        }

        public void Remove(T item)
        {
            List<T> items = indexes[item].list;

            items.Remove(item);

            if(items.Count == 0)
                contents.Remove(indexes[item].priority);
            
            indexes.Remove(item);
        }

        public bool Contains(T item)
        {
            return indexes.ContainsKey(item);
        }

        public IEnumerator<KeyValuePair<int, List<T>>> GetEnumerator()
        {
            return contents.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

}
