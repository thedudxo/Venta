using DudCo.Events;

namespace Tests
{
    public partial class EventSenderTests
    {
        class UnSubscribeDuringEvent : ISomeSubscriber
        {
            EventSender<ISomeSubscriber> sender;

            public UnSubscribeDuringEvent(EventSender<ISomeSubscriber> sender)
            {
                this.sender = sender;
            }

            public bool triggered = false;

            public void OnTrigger()
            {
                triggered = true;
                sender.Unsubscribe(this);
            }
        }
    }
}