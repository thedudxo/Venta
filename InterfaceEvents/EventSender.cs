﻿using System;

namespace DudCo.Events
{
    /// <summary>
    /// Prioritised interface events.
    /// Uses subscription queues to avoid modification while sending out events.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventSender<T>
    {
        PrioritisedList<T> subscribers = new PrioritisedList<T>();
        readonly SubscriptionQueue<T> subscriptionQueue;
        readonly PriorityDictionary typePriorities;

        SendMethod _sendMethod;

        /// <summary>
        /// what <see cref="Events.SendMethod"/> to use. <see cref="Events.SendMethod.All"/> by default.
        /// </summary>
        public SendMethod SendMethod
        {
            get => _sendMethod;
            set
            {
                _sendMethod = value;

                switch (_sendMethod)
                {
                    case SendMethod.All:
                        sendStratergy = new SendToAllSubscribers<T>();
                        break;

                    case SendMethod.OnlyHighestPriority:
                        sendStratergy = new SendToHighestPriorityBracket<T>();
                        break;

                    default: throw new System.NotImplementedException();
                }
            }
        } 

        ISendStratergy<T> sendStratergy;

        bool sending = false;

        /// <summary>
        /// Create an EventSender with an empty <see cref="PriorityDictionary"/>.
        /// </summary>
        public EventSender() : this(new EmptyPriorityDictionary()) { }

        /// <summary>
        /// Create an EventSender with a <see cref="PriorityDictionary"/>.
        /// </summary>
        /// <param name="typePriorities"></param>
        public EventSender(PriorityDictionary typePriorities)
        {
            this.typePriorities = typePriorities;
            subscriptionQueue = new SubscriptionQueue<T>(subscribers);
            SendMethod = SendMethod.All;
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

            sending = true;

            subscriptionQueue.DelaySubscriptions();

            sendStratergy.SendToSubscribers(notify, subscribers);

            subscriptionQueue.UnDelaySubscriptions();
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

            subscriptionQueue.Subscribe(subscriber, priority);
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

            subscriptionQueue.Subscribe(subscriber, typePriorities[subType]);
        }

        /// <summary>
        /// Unsubscribe an item from the event.
        /// </summary>
        /// <param name="subscriber">The item to Unsubscribe.</param>
        public void Unsubscribe(T subscriber) => subscriptionQueue.Unsubscribe(subscriber);
        
        /// <summary>
        /// Unsubscribe everything.
        /// </summary>
        public void Clear() => subscribers = new PrioritisedList<T>();
    }
}