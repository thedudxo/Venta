using System.Collections.Generic;
using System;

namespace Game.Events
{
    /// <summary>
    /// Interface events
    /// uses subscribe and unsubscribe queues to avoid collection modified errors while enumerating
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class EventSender<T>
    {
        List<T> subscribers = new List<T>();
        Queue<T> toSubscribe = new Queue<T>();
        Queue<T> toUnsubscribe = new Queue<T>();

        delegate void SubscriptionMethod(T subscriber);
        SubscriptionMethod subscribe;
        SubscriptionMethod unsubscribe;

        public EventSender()
        {
            subscribe = AddNow;
            unsubscribe = RemoveNow;
        }

        public void SendEvent(Action<T> notify)
        {
            subscribe = AddAfter;
            unsubscribe = RemoveAfter;

            foreach (T subscriber in subscribers)
                notify(subscriber);

            subscribe = AddNow;
            unsubscribe = RemoveNow;

            foreach(T sub in toSubscribe)
                AddNow(sub);

            foreach (T sub in toUnsubscribe)
                RemoveNow(sub);
        }

        public void Subscribe(T subscriber)
        {
            if (subscribers.Contains(subscriber))
                throw new InvalidOperationException($"Cannot subscribe '{subscriber}'. was allready subscribed.");
            else 
                subscribe(subscriber);
        }
        public void Unsubscribe(T subscriber)
        {
            if (subscribers.Contains(subscriber))
                unsubscribe(subscriber);
            else throw new InvalidOperationException($"Cannot unsubscribe '{subscriber}'. was not subscribed.");
        }

        void AddNow(T subscriber)
            => subscribers.Add(subscriber);

        void AddAfter(T subscriber)
            => toSubscribe.Enqueue(subscriber);

        void RemoveNow(T sub)
            => subscribers.Remove(sub);

        void RemoveAfter(T sub)
            => toUnsubscribe.Enqueue(sub);
    }
}