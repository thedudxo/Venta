using System.Collections.Generic;
using System;

namespace DudCo.Events
{
    public abstract class PriorityDictionary
    {
        readonly Dictionary<Type, int> priorities = new Dictionary<Type, int>();

        public void Add(Type type, int priorty) => priorities.Add(type, priorty);
        public void Add<T>(int priority) => priorities.Add(typeof(T), priority);
        public bool ContainsKey(Type t) => priorities.ContainsKey(t);
        public int PriorityOf<T>() => priorities[typeof(T)];
        public int PriorityOf(Type type) => priorities[type];
        public int this[Type t] => priorities[t];
        public int After<T>() => PriorityOf<T>() - 1;
        public int Before<T>() => PriorityOf<T>() + 1;
    }
}