namespace DudCo.Events
{
    /// <summary>
    /// Builder for <see cref="EventSender{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventBuilder<T>
    {
        ISendStratergy<T> sendStratergy;
        PriorityDictionary priorityDictionary;

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
            sendStratergy = new SendToAllSubscribers<T>();
            priorityDictionary = new EmptyPriorityDictionary();
        }

        /// <summary>
        /// Get the final product
        /// </summary>
        /// <returns>new <see cref="EventSender{T}"/> with previously specified settings</returns>
        public EventSender<T> Build()
        {
            return new EventSender<T>(priorityDictionary, sendStratergy);
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
        public EventBuilder<T> WithSendToHighestPriority()
        {
            sendStratergy = new SendToHighestPriorityBracket<T>();
            return this;
        }

    }
}