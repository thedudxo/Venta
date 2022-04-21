using System.Collections.Generic;
using System;

namespace DudCo.Events
{
    class SubscriptionQueue<T>
    {
        readonly PrioritisedList<T> subscribers;
        readonly Queue<(T value, int priority)> toSubscribe = new Queue<(T, int)>();
        readonly Queue<T> toUnsubscribe = new Queue<T>();

        Action<T, int> subscribeAction;
        Action<T> unsubscribeAction;

        public SubscriptionQueue(PrioritisedList<T> subscribers)
        {
            this.subscribers = subscribers;

            subscribeAction = AddNow;
            unsubscribeAction = RemoveNow;
        }


        public void ExecuteDelayedSubscriptionRequests()
        {
            UnDelaySubscriptions();

            foreach ((T value, int priority) in toSubscribe)
                Subscribe(value, priority);

            foreach (T sub in toUnsubscribe)
                Unsubscribe(sub);

            toSubscribe.Clear();
            toUnsubscribe.Clear();
        }

        void UnDelaySubscriptions()
        {
            subscribeAction = AddNow;
            unsubscribeAction = RemoveNow;
        }

        public void DelaySubscriptions()
        {
            subscribeAction = AddAfter;
            unsubscribeAction = RemoveAfter;
        }

        public void Subscribe(T subscriber, int priority)
        {
            if (subscribers.Contains(subscriber))
                throw new ArgumentException($"Cannot subscribe '{subscriber}'. was allready subscribed.");

            subscribeAction(subscriber, priority);
        }
        public void Unsubscribe(T subscriber)
        {
            if (subscribers.Contains(subscriber) == false)
                throw new ArgumentException($"Cannot unsubscribe '{subscriber}'. was not subscribed.");

            unsubscribeAction(subscriber);
        }

        void AddNow(T subscriber, int priority)
        {
            subscribers.Add(subscriber, priority);
        }

        void RemoveNow(T subscriber)
        {
            subscribers.Remove(subscriber);
        }

        void AddAfter(T subscriber, int priority)
        {
            toSubscribe.Enqueue((subscriber, priority));
        }

        void RemoveAfter(T subscriber)
        {
            toUnsubscribe.Enqueue(subscriber);
        }
    }
}