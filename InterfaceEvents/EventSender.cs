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
        readonly SubscriptionQueue<T> subscriptionQueue;
        readonly PriorityDictionary typePriorities;

        bool sending = false;

        public EventSender() : this(new EmptyPriorityDictionary()) { }
        public EventSender(PriorityDictionary typePriorities)
        {
            this.typePriorities = typePriorities;
            subscriptionQueue = new SubscriptionQueue<T>(subscribers);
        }

        public void Send(Action<T> notify)
        {
            if (sending) throw new InvalidOperationException("Recursive event sending is not supported");
            //this is beacuse un/subscribing during an event won't happen untill the recursion ends
            sending = true;

            subscriptionQueue.DelaySubscriptions();

            foreach (KeyValuePair<int, LinkedList<T>> pair in subscribers)
                foreach (T subscriber in pair.Value)
                    notify(subscriber);

            subscriptionQueue.UnDelaySubscriptions();

            subscriptionQueue.ExecuteDelayedSubscriptionRequests();

            sending = false;
        }

        public void Subscribe(T subscriber, int priority = 0)
        {
            if (typePriorities.ContainsKey(subscriber.GetType()))
                throw new ArgumentException($"Subscriber type had an entry in the priority dictionary. Use {nameof(SubscribeByRegisteredType)} instead.", nameof(subscriber));

            subscriptionQueue.Subscribe(subscriber, priority);
        }

        public void SubscribeByRegisteredType(T subscriber)
        {
            Type subType = subscriber.GetType();

            if (typePriorities.ContainsKey(subType) == false) 
                throw new ArgumentException("Was not found in the priority dictionary", nameof(subscriber));

            subscriptionQueue.Subscribe(subscriber, typePriorities[subType]);
        }

        public void Unsubscribe(T subscriber)
        {
            if (subscribers.Contains(subscriber))
                subscriptionQueue.Unsubscribe(subscriber);
            else throw new InvalidOperationException($"Cannot unsubscribe '{subscriber}'. was not subscribed.");
        }

        public void Clear()
        {
            subscribers = new PrioritisedList<T>();
        }
    }
}