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
        SubscriptionMethod subscribeAction;
        UnsubscriptionMethod unsubscribeAction;

        PriorityDictionary typePriorities;

        bool sending = false;

        public EventSender() : this(new EmptyPriorityDictionary()) { }

        public EventSender(PriorityDictionary typePriorities)
        {
            this.typePriorities = typePriorities;

            subscribeAction = AddNow;
            unsubscribeAction = RemoveNow;
        }

        public void Send(Action<T> notify)
        {
            if (sending) throw new InvalidOperationException("Recursive event sending is not supported");
            //this is beacuse un/subscribing during an event won't happen untill the recursion ends
            sending = true;

            DelaySubscriptions();

            foreach (KeyValuePair<int, LinkedList<T>> pair in subscribers)
                foreach (T subscriber in pair.Value)
                    notify(subscriber);

            UnDelaySubscriptions();

            ExecuteDelayedSubscriptionRequests();

            sending = false;
        }

        private void ExecuteDelayedSubscriptionRequests()
        {
            foreach ((T value, int priority) in toSubscribe)
                AddNow(value, priority);

            foreach (T sub in toUnsubscribe)
                RemoveNow(sub);

            toSubscribe.Clear();
            toUnsubscribe.Clear();
        }

        private void UnDelaySubscriptions()
        {
            subscribeAction = AddNow;
            unsubscribeAction = RemoveNow;
        }

        private void DelaySubscriptions()
        {
            subscribeAction = AddAfter;
            unsubscribeAction = RemoveAfter;
        }

        public void Subscribe(T subscriber, int priority = 0)
        {
            if (subscribers.Contains(subscriber))
                throw new InvalidOperationException($"Cannot subscribe '{subscriber}'. was allready subscribed.");
            else
            {
                Type subType = subscriber.GetType();

                if (typePriorities.ContainsKey(subType))
                    subscribeAction(subscriber, typePriorities[subType]);

                else
                    subscribeAction(subscriber, priority);
            }
        }

        public void SubscribeByRegisteredType(T subscriber)
        {
            Type subType = subscriber.GetType();

            if (typePriorities.ContainsKey(subType) == false) 
                throw new ArgumentException("Was not found in the priority dictionary", nameof(subscriber));
            
            subscribeAction(subscriber, typePriorities[subType]);
        }

        public void Unsubscribe(T subscriber)
        {
            if (subscribers.Contains(subscriber))
                unsubscribeAction(subscriber);
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

        public void Clear()
        {
            subscribers = new PrioritisedList<T>();
        }
    }
}