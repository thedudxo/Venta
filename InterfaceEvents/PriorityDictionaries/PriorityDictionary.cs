using System.Collections.Generic;
using System;

namespace DudCo.Events
{
    /// <summary>
    /// A dictionary of types and the priorities they should have, once subscribed to an EventSender.
    /// </summary>
    public abstract class PriorityDictionary
    {
        readonly Dictionary<Type, int> priorities = new Dictionary<Type, int>();

        /// <summary>
        /// Add a type to the dictionary.
        /// </summary>
        /// <param name="type">The type to add.</param>
        /// <param name="priorty">The priority the type should have.</param>
        public void Add(Type type, int priorty) => priorities.Add(type, priorty);

        /// <summary>
        /// Add a type to the dictionary.
        /// </summary>
        /// <typeparam name="T">The type to add.</typeparam>
        /// <param name="priority">The priority the type should have.</param>
        public void Add<T>(int priority) => priorities.Add(typeof(T), priority);

        /// <summary>
        /// Check if the type has an entry in the dictionary.
        /// </summary>
        /// <param name="t">The type to look for</param>
        /// <returns>True if the type is in the dictionary.</returns>
        public bool ContainsKey(Type t) => priorities.ContainsKey(t);

        /// <summary>
        /// Get the priority of a type in the dictionary.
        /// </summary>
        /// <typeparam name="T">The type to get the priority of</typeparam>
        /// <returns>Priority of the given type.</returns>
        public int PriorityOf<T>() => priorities[typeof(T)];

        /// <summary>
        /// Get the priority of a type in the dictionary.
        /// </summary>
        /// <param name="type">The type to get the priority of</param>
        /// <returns>Priority of the given type.</returns>
        public int PriorityOf(Type type) => priorities[type];

        /// <summary>
        /// Get the priority of a type in the dictionary.
        /// </summary>
        /// <param name="t">The type to get the priority of</param>
        /// <returns>Priority of the given type.</returns>
        public int this[Type t] => priorities[t];

        /// <summary>
        /// Get a priority which is lower than a type already in this dictionary.
        /// </summary>
        /// <typeparam name="T">The type to get a lower priority than</typeparam>
        /// <returns>A lower priority than the given item.</returns>
        public int After<T>() => PriorityOf<T>() - 1;

        /// <summary>
        /// Get a priority which is higher than a type already in this dictionary.
        /// </summary>
        /// <typeparam name="T">The type to get a higher priority than</typeparam>
        /// <returns>A higher priority than the given item.</returns>
        public int Before<T>() => PriorityOf<T>() + 1;

    }
}