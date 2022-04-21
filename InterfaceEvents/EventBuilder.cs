namespace DudCo.Events
{
    /// <summary>
    /// Builder for <see cref="EventSender{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventBuilder<T>
    {
        ISendStratergy<T> sendStratergy;
        ISubscribeStratergy<T> subscribeStratergy;
        PriorityDictionary priorityDictionary;
        SubscriptionQueue<T> subscriptionQueue;
        PrioritisedList<T> subscribers;

        /// <summary>
        /// Create defualt builder
        /// </summary>
        public EventBuilder()
        {
            Clear();
        }

        /// <summary>
        /// Reset this builder to it's defualt state
        /// </summary>
        public void Clear()
        {
            sendStratergy = new DefaultSendStratergy<T>();
            priorityDictionary = new EmptyPriorityDictionary();
            subscribers = new PrioritisedList<T>();
            subscriptionQueue = new SubscriptionQueue<T>(subscribers);
        }

        /// <summary>
        /// Get the final product
        /// </summary>
        /// <returns>new <see cref="EventSender{T}"/> with previously specified settings</returns>
        public EventSender<T> Build()
        {
            return new EventSender<T>(priorityDictionary, sendStratergy, subscribeStratergy, subscribers, subscriptionQueue);
        }

        /// <summary>
        /// Create <see cref="EventSender{T}"/> with a <see cref="PriorityDictionary"/>
        /// </summary>
        /// <param name="priorityDictionary"></param>
        /// <returns></returns>
        public EventBuilder<T> WithPriorityDictionary(PriorityDictionary priorityDictionary)
        {
            this.priorityDictionary = priorityDictionary;
            return this;
        }

        /// <summary>
        /// Notify only the highest priority subscribers. Will notify multiple if they all have the highest priority.
        /// </summary>
        /// <returns></returns>
        public EventBuilder<T> SendOnlyHighestPriority()
        {
            sendStratergy = new HighestPrioritySendStratergy<T>();
            return this;
        }

        /// <summary>
        /// Allow the event to be sent only one time. 
        /// late subscriptions after the event has sent will "bounce back" and receive the event immediately.
        /// Usefull for delaying the creation of objects untill their dependancies have been created.
        /// </summary>
        /// <returns></returns>
        public EventBuilder<T> SendOnlyOnce()
        {
            var onlyOnceSendStratergy = new OnlyOnceSendStratergy<T>();

            var defualtSubscribe = new DefualtSubscribeStratergy<T>(subscriptionQueue);
            subscribeStratergy = new SendOnceSubscribeStratergy<T>(defualtSubscribe, onlyOnceSendStratergy);
            sendStratergy = onlyOnceSendStratergy;

            return this;
        }

    }
}