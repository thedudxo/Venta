using System;
using System.Collections.Generic;

namespace DudCo.Events
{
    public class InterfaceEvent<Interface> : EventSender<Interface>
    {
        readonly PriorityDictionary Priorities;

        public InterfaceEvent(PriorityDictionary priorities)
        {
            Priorities = priorities;
        }

        public void Subscribe(Interface sub)
        {
            Type subType = sub.GetType();

            if (Priorities.ContainsKey(subType))
                Subscribe(subType, sub);

            else
                Subscribe(sub, 0);
        }

        public void Subscribe<T>(Interface sub) => Subscribe(typeof(T), sub);

        public void Subscribe(Type type, Interface sub)
        {
            bool KeyNotFound = Priorities.ContainsKey(type) == false;
            if (KeyNotFound) throw new KeyNotFoundException($"{type} did not have an entry in the priority dictionary.");

            Subscribe(sub, Priorities[type]);
        }
    }
}