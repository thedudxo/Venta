using System.Collections.Generic;
using System;

namespace DudCo.Events
{
    class SubscriptionQueue<T>
    {
        PrioritisedList<T> subscribers;
        Queue<(T value, int priority)> toSubscribe = new Queue<(T, int)>();
        Queue<T> toUnsubscribe = new Queue<T>();

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
            foreach ((T value, int priority) in toSubscribe)
                AddNow(value, priority);

            foreach (T sub in toUnsubscribe)
                RemoveNow(sub);

            toSubscribe.Clear();
            toUnsubscribe.Clear();
        }

        public void UnDelaySubscriptions()
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
            // subscribe something thats allready qeued?
            if (subscribers.Contains(subscriber))
                throw new ArgumentException($"Cannot subscribe '{subscriber}'. was allready subscribed.");

            subscribeAction(subscriber, priority);
        }
        public void Unsubscribe(T subscriber) => unsubscribeAction(subscriber);

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