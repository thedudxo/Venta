using System.Collections;
using System.Collections.Generic;

namespace DudCo.Events
{
    internal class PrioritisedList<T> : IEnumerable<KeyValuePair<int, LinkedList<T>>>
    {
        //similar concept to the contents and index sections of a book
        SortedDictionary<int, LinkedList<T>> contents;
        Dictionary<T, (LinkedListNode<T> node, int priority)> indexes;

        internal PrioritisedList()
        {
            contents = new SortedDictionary<int, LinkedList<T>>();
            indexes = new Dictionary<T, (LinkedListNode<T>, int)>();
        }

        public void Add(T item, int priority) //O(log n)
        { 
            //invert so the dictionary is sorted the right way around
            priority *= -1;
            LinkedListNode<T> node = null;

            bool priorityAllreadyExists = contents.ContainsKey(priority); //O(log n)
            if (priorityAllreadyExists)
            {
                var items = contents[priority]; //O(log n)
                node = items.AddLast(item); //O(1)
            }
            else
                node = CreateAndAddToNewItemsList(item, priority); //O(log n)

            var index = (node, priority);
            indexes[item] = index; //O(1)
        }

        private LinkedListNode<T> CreateAndAddToNewItemsList(T item, int prio) //O(log n)
        {
            LinkedList<T> items = new LinkedList<T>();
            LinkedListNode<T> node;

            node = items.AddLast(item); //O(1)
            contents.Add(prio, items); //O(log n)

            return node;
        }

        public void Remove(T item) //O(log n)
        {
            LinkedListNode<T> node = indexes[item].node; //O(1)
            LinkedList<T> items = node.List; 

            items.Remove(item); //O(1)

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
