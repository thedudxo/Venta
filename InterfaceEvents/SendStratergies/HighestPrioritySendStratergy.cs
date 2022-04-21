using System;

namespace DudCo.Events
{
    internal class HighestPrioritySendStratergy<T> : ISendStratergy<T>
    {
        public void SendToSubscribers(Action<T> notify, PrioritisedList<T> subscribers)
        {
            foreach(T subscriber in subscribers.GetHighestPriorityItems())
                notify(subscriber);
        }
    }
}