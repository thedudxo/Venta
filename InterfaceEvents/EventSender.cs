using System;

namespace DudCo.Events
{
    /// <summary>
    /// Prioritised Events.
    /// Uses subscription queues to avoid modification while sending out events.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventSender<T>
    {
        readonly PrioritisedList<T> subscribers;
        readonly SubscriptionQueue<T> subscriptionQueue;
        readonly PriorityDictionary typePriorities;
        readonly ISendStratergy<T> sendStratergy;
        readonly ISubscribeStratergy<T> subscribeStratergy;

        bool sending = false;

        /// <summary>
        /// Create an EventSender with an empty <see cref="PriorityDictionary"/>.
        /// </summary>
        public EventSender() 
            : this(new EmptyPriorityDictionary()) 
        {}

        /// <summary>
        /// Create an EventSender with a <see cref="PriorityDictionary"/>.
        /// </summary>
        /// <param name="typePriorities"></param>
        public EventSender(PriorityDictionary typePriorities)
            : this(typePriorities, null)
        {}

        internal EventSender(PriorityDictionary typePriorities, ISendStratergy<T> sendStratergy = null, ISubscribeStratergy<T> subscribeStratergy = null,  PrioritisedList<T> subscribers = null, SubscriptionQueue<T> subscriptionQueue = null)
        {
            this.typePriorities = typePriorities;
            this.subscribers = subscribers ?? new PrioritisedList<T>();
            this.subscriptionQueue = subscriptionQueue ?? new SubscriptionQueue<T>(this.subscribers);
            this.sendStratergy = sendStratergy ?? new DefaultSendStratergy<T>();
            this.subscribeStratergy = subscribeStratergy ?? new DefualtSubscribeStratergy<T>(this.subscriptionQueue);
        }

        /// <summary>
        /// Send the event to all subscribers.
        /// </summary>
        /// <param name="notify">Delegate used to send the event.</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void Send(Action<T> notify)
        {
            if (sending) throw new InvalidOperationException("Recursive event sending is not supported");
            //this is beacuse un/subscribing during an event won't happen untill the recursion ends
            //this will also catch async race conditions

            sending = true;

            subscriptionQueue.DelaySubscriptions();

            sendStratergy.SendToSubscribers(notify, subscribers);

            subscriptionQueue.ExecuteDelayedSubscriptionRequests();

            sending = false;
        }

        /// <summary>
        /// Subscribe an item to this event.
        /// </summary>
        /// <param name="subscriber">The item to subscribe.</param>
        /// <param name="priority">Priority the item should have. defaults to 0.</param>
        /// <exception cref="ArgumentException"></exception>
        public void Subscribe(T subscriber, int priority = 0)
        {
            if (typePriorities.ContainsKey(subscriber.GetType()))
                throw new ArgumentException($"Subscriber type had an entry in the priority dictionary. Use {nameof(SubscribeByRegisteredType)} instead.", nameof(subscriber));

            subscribeStratergy.Subscribe(subscriber, priority);
        }

        /// <summary>
        /// Subscribe multiple items to this event.
        /// </summary>
        /// <param name="subscribers">The items to subscribe</param>
        public void Subscribe(params T[] subscribers)
        {
            Subscribe(0, subscribers);
        }

        /// <summary>
        /// Subscribe multiple items to this event with priority.
        /// </summary>
        /// <param name="priority">The priority each item should have</param>
        /// <param name="subscribers">The items to subscribe</param>
        public void Subscribe(int priority, params T[] subscribers)
        {
            foreach (T subscriber in subscribers)
                Subscribe(subscriber, priority);
        }

        /// <summary>
        /// Subscribe an item only if it isn't allready subscribed.
        /// </summary>
        /// <param name="subscriber"></param>
        /// <param name="priority"></param>
        /// <returns>True if sucsessfull, false if allready subscribed.</returns>
        public bool TrySubscribe(T subscriber, int priority = 0)
        {
            if (subscribers.Contains(subscriber)) 
                return false;

            Subscribe(subscriber, priority);
            return true;
        }

        /// <summary>
        /// Subscribe an item in the <see cref="PriorityDictionary"/> to the event.
        /// </summary>
        /// <param name="subscriber">The item to subscribe.</param>
        /// <exception cref="ArgumentException"></exception>
        public void SubscribeByRegisteredType(T subscriber)
        {
            Type subType = subscriber.GetType();

            if (typePriorities.ContainsKey(subType) == false) 
                throw new ArgumentException("Was not found in the priority dictionary", nameof(subscriber));

            subscribeStratergy.Subscribe(subscriber, typePriorities[subType]);
        }

        /// <summary>
        /// Subscribe items in the <see cref="PriorityDictionary"/> to the event.
        /// </summary>
        /// <param name="subscribers">The items to subscribe.</param>
        public void SubscribeByRegisteredType(params T[] subscribers)
        {
            foreach (T subscriber in subscribers)
                SubscribeByRegisteredType(subscriber);
        }

        /// <summary>
        /// Unsubscribe an item from the event.
        /// </summary>
        /// <param name="subscriber">The item to Unsubscribe.</param>
        public void Unsubscribe(T subscriber) => subscriptionQueue.Unsubscribe(subscriber);

        /// <summary>
        /// Unsubscribe items from the event.
        /// </summary>
        /// <param name="subscribers"></param>
        public void Unsubscribe(params T[] subscribers)
        {
            foreach (T subscriber in subscribers)
                Unsubscribe(subscriber);
        }

        /// <summary>
        /// Unsubscribe only if the item is actually subscribed.
        /// </summary>
        /// <param name="subscriber"></param>
        /// <returns>True if sucsessfull, false if the item was not subscribed.</returns>
        public bool TryUnsubscribe(T subscriber)
        {
            if (subscribers.Contains(subscriber) == false)
                return false;

            Unsubscribe(subscriber);
            return true;
        }
        
        /// <summary>
        /// Unsubscribe everything.
        /// </summary>
        public void Clear() => subscribers.Clear();
    }
}