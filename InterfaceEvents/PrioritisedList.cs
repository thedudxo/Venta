using System.Collections;
using System.Collections.Generic;

namespace DudCo.Events
{
    internal class PrioritisedList<T> : IEnumerable<KeyValuePair<int, LinkedList<T>>>
    {
        //similar concept to the contents and index sections of a book
        //this setup allows both Add() and Remove() to be O(log n) Operations
        SortedDictionary<int, LinkedList<T>> contents;
        Dictionary<T, (LinkedListNode<T> node, int priority)> indexes;

        internal PrioritisedList()
        {
            contents = new SortedDictionary<int, LinkedList<T>>();
            indexes = new Dictionary<T, (LinkedListNode<T>, int)>();
        }

        public void Add(T item, int priority)
        { 
            //invert so the dictionary is sorted the right way around
            priority *= -1;
            LinkedListNode<T> node = null;

            bool priorityAllreadyExists = contents.ContainsKey(priority);
            if (priorityAllreadyExists)
            {
                var items = contents[priority];
                node = items.AddLast(item);
            }
            else
                node = CreateAndAddToNewItemsList(item, priority);

            var index = (node, priority);
            indexes[item] = index;
        }

        private LinkedListNode<T> CreateAndAddToNewItemsList(T item, int prio)
        {
            LinkedList<T> items = new LinkedList<T>();
            LinkedListNode<T> node;

            node = items.AddLast(item); 
            contents.Add(prio, items); 

            return node;
        }

        public void Remove(T item)
        {
            LinkedListNode<T> node = indexes[item].node;
            LinkedList<T> items = node.List; 

            items.Remove(item);

            if (items.Count == 0)
            {
                int priority = indexes[item].priority;
                contents.Remove(priority);
            }
            
            indexes.Remove(item);
        }

        public bool Contains(T item)
        {
            return indexes.ContainsKey(item);
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
