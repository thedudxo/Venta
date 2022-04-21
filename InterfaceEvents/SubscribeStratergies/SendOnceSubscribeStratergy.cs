namespace DudCo.Events
{
    class SendOnceSubscribeStratergy<T> : ISubscribeStratergy<T>
    {
        readonly DefualtSubscribeStratergy<T> defualtSubscribeStratergy;
        readonly OnlyOnceSendStratergy<T> sendStratergy;
        public SendOnceSubscribeStratergy(DefualtSubscribeStratergy<T> defualtSubscribeStratergy, OnlyOnceSendStratergy<T> sendStratergy)
        {
            this.defualtSubscribeStratergy = defualtSubscribeStratergy;
            this.sendStratergy = sendStratergy;
        }

        public void Subscribe(T subscriber, int priority)
        {
            if (sendStratergy.Sent)
            {
                sendStratergy.NotifyAction(subscriber);
                return;
            }

            defualtSubscribeStratergy.Subscribe(subscriber, priority);
        }
    }
}