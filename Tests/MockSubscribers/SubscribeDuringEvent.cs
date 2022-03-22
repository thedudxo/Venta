using DudCo.Events;

namespace Tests
{
    public partial class EventSenderTests
    {
        class SubscribeDuringEvent : ISomeSubscriber
        {
            EventSender<ISomeSubscriber> sender;
            ISomeSubscriber newSub;
            public bool hasSubbed = false;

            public SubscribeDuringEvent(EventSender<ISomeSubscriber> sender, ISomeSubscriber newSub)
            {
                this.sender = sender;
                this.newSub = newSub;
            }

            public bool triggered = false;

            public void OnTrigger()
            {
                triggered = true;
                if (hasSubbed == false)
                {
                    sender.Subscribe(newSub);
                    hasSubbed = true;
                }
            }
        }
    }
}