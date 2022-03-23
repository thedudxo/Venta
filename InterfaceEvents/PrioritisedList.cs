using System.Collections;
using System.Collections.Generic;

namespace DudCo.Events
{
    internal class PrioritisedList<T> : IEnumerable<KeyValuePair<int, LinkedList<T>>>
    {
        //similar concept to the contents and index sections of a book
        //this setup allows both Add() and Remove() to be O(log n) Operations
        readonly SortedDictionary<int, LinkedList<T>> ItemsByPriority;
        readonly Dictionary<T, (LinkedListNode<T> node, int priority)> PriorityByItem;

        internal PrioritisedList()
        {
            ItemsByPriority = new SortedDictionary<int, LinkedList<T>>();
            PriorityByItem = new Dictionary<T, (LinkedListNode<T>, int)>();
        }

        public void Add(T item, int priority)
        { 
            //invert so the dictionary is sorted the right way around
            priority *= -1;
            LinkedListNode<T> node;

            bool priorityAllreadyExists = ItemsByPriority.ContainsKey(priority);
            if (priorityAllreadyExists)
            {
                LinkedList<T> items = ItemsByPriority[priority];
                node = items.AddLast(item);
            }
            else
                node = CreateAndAddToNewItemsList(item, priority);

            var index = (node, priority);
            PriorityByItem[item] = index;
        }

        private LinkedListNode<T> CreateAndAddToNewItemsList(T item, int prio)
        {
            LinkedList<T> items = new LinkedList<T>();
            LinkedListNode<T> node;

            node = items.AddLast(item); 
            ItemsByPriority.Add(prio, items); 

            return node;
        }

        public void Remove(T item)
        {
            LinkedListNode<T> node = PriorityByItem[item].node;
            LinkedList<T> items = node.List; 

            items.Remove(item);

            if (items.Count == 0)
            {
                int priority = PriorityByItem[item].priority;
                ItemsByPriority.Remove(priority);
            }
            
            PriorityByItem.Remove(item);
        }

        public bool Contains(T item)
        {
            return PriorityByItem.ContainsKey(item);
        }

        public IEnumerator<KeyValuePair<int, LinkedList<T>>> GetEnumerator()
        {
            return ItemsByPriority.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

}
