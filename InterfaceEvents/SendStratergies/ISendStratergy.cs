using System;

namespace DudCo.Events
{
    internal interface ISendStratergy<T>
    {
        void SendToSubscribers(Action<T> notify, PrioritisedList<T> subscribers);
    }
}