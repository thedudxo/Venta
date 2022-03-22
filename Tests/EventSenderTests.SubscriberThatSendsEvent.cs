using DudCo.Events;

namespace Tests
{
    public partial class EventSenderTests
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
}