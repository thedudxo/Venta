using System.Collections.Generic;
using System;

namespace DudCo.Events
{
    /// <summary>
    /// Prioritised interface events
    /// uses subscription queues to avoid modification while sending out events
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventSender<T>
    {
        PrioritisedList<T> subscribers = new PrioritisedList<T>();
        Queue<(T value, int priority)> toSubscribe = new Queue<(T,int)>();
        Queue<T> toUnsubscribe = new Queue<T>();

        delegate void SubscriptionMethod(T subscriber, int priority);
        delegate void UnsubscriptionMethod(T subscriber);
        SubscriptionMethod subscribe;
        UnsubscriptionMethod unsubscribe;

        public EventSender()
        {
            subscribe = AddNow;
            unsubscribe = RemoveNow;
        }

        public void Send(Action<T> notify)
        {
            subscribe = AddAfter;
            unsubscribe = RemoveAfter;

            foreach (KeyValuePair<int,List<T>> pair in subscribers)
                foreach (T subscriber in pair.Value)
                    notify(subscriber);

            subscribe = AddNow;
            unsubscribe = RemoveNow;

            foreach((T value, int priority) subscriber in toSubscribe)
                AddNow(subscriber.value, subscriber.priority);

            foreach (T sub in toUnsubscribe)
                RemoveNow(sub);

            toSubscribe.Clear();
            toUnsubscribe.Clear();
        }

        public void Subscribe(T subscriber, int priority = 0)
        {
            if (subscribers.Contains(subscriber))
                throw new InvalidOperationException($"Cannot subscribe '{subscriber}'. was allready subscribed.");
            else 
                subscribe(subscriber, priority);
        }
        public void Unsubscribe(T subscriber)
        {
            if (subscribers.Contains(subscriber))
                unsubscribe(subscriber);
            else throw new InvalidOperationException($"Cannot unsubscribe '{subscriber}'. was not subscribed.");
        }

        void AddNow(T subscriber, int priority)
            => subscribers.Add(subscriber, priority);

        void AddAfter(T subscriber, int priority)
            => toSubscribe.Enqueue((subscriber, priority));

        void RemoveNow(T sub)
            => subscribers.Remove(sub);

        void RemoveAfter(T sub)
            => toUnsubscribe.Enqueue(sub);
    }
}