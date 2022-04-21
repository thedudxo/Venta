using System;
using System.Linq;

namespace DudCo.Events
{
    internal class DefaultSendStratergy<T> : ISendStratergy<T>
    {
        public void SendToSubscribers(Action<T> notify, PrioritisedList<T> subscribers)
        {
            foreach (var subscriber in subscribers.SelectMany(pair => pair.Value))
            {
                notify(subscriber);
            }
        }
    }
}