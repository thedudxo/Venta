namespace DudCo.Events
{
    class DefualtSubscribeStratergy<T> : ISubscribeStratergy<T>
    {

        readonly SubscriptionQueue<T> subscriptionQueue;

        public DefualtSubscribeStratergy(SubscriptionQueue<T> subscriptionQueue)
        {
            this.subscriptionQueue = subscriptionQueue;
        }

        public void Subscribe(T subscriber, int priority)
        {
            subscriptionQueue.Subscribe(subscriber, priority);
        }
    }
}