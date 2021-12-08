using System.Collections;
using System.Collections.Generic;

namespace DudCo.Events
{
    internal class PrioritisedList<T> : IEnumerable<KeyValuePair<int, LinkedList<T>>>
    {
        //similar concept to the contents and index sections of a book
        SortedDictionary<int, LinkedList<T>> contents = new SortedDictionary<int, LinkedList<T>>();
        Dictionary<T, (LinkedList<T>list, int priority)> indexes = new Dictionary<T, (LinkedList<T>,int)>();

        public void Add(T item, int priority) //O(log n)
        { 
            //invert so the dictionary is sorted the right way around
            priority *= -1;

            bool priorityAllreadyExists = contents.ContainsKey(priority); //O(log n)

            if (priorityAllreadyExists)
                contents[priority].AddLast(item); //O(1)
            else
                CreateAndAddToNewItemsList(item, priority);

            var itemList = contents[priority]; //O(log n)
            var index = (itemList, priority);
            indexes[item] = index; //O(1)
        }

        private void CreateAndAddToNewItemsList(T item, int prio) //O(log n)
        {
            LinkedList<T> items = new LinkedList<T>();
            items.AddLast(item); //O(1)
            contents.Add(prio, items); //O(log n)
        }

        public void Remove(T item) //O(n)
        {
            LinkedList<T> items = indexes[item].list; //O(1)

            items.Remove(item); //O(n)

            if (items.Count == 0)
            {
                int priority = indexes[item].priority; //O(1)
                contents.Remove(priority); //O(log n)
            }
            
            indexes.Remove(item); //O(1)
        }

        public bool Contains(T item)
        {
            return indexes.ContainsKey(item); //O(1)
        }

        public IEnumerator<KeyValuePair<int, LinkedList<T>>> GetEnumerator()
        {
            return contents.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

}
