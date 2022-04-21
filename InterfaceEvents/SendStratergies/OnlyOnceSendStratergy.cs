using System;

namespace DudCo.Events
{
    internal class OnlyOnceSendStratergy<T> : ISendStratergy<T>
    {
        readonly DefaultSendStratergy<T> sendToAllSubscribers = new DefaultSendStratergy<T>();
        internal bool Sent { get; private set; } = false;
        internal Action<T> NotifyAction { get; private set; }

        public void SendToSubscribers(Action<T> notify, PrioritisedList<T> subscribers)
        {
            if (Sent) throw new InvalidOperationException("SendOnlyOnce event has allready been sent");
            Sent = true;
            NotifyAction = notify;
            sendToAllSubscribers.SendToSubscribers(notify, subscribers);
            subscribers.Clear();
        }
    }
}