using DudCo.Events;

namespace Tests
{
    class SubscriberThatSendsEvent : ISomeSubscriber
    {
        EventSender<ISomeSubscriber> eventSender;

        public SubscriberThatSendsEvent(EventSender<ISomeSubscriber> eventSender)
        {
            this.eventSender = eventSender;
        }

        public void OnTrigger()
        {
            eventSender.Send((ISomeSubscriber sub) => sub.OnTrigger());
        }
    }
}