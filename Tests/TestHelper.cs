using DudCo.Events;

namespace Tests
{
    public class TestHelper
    {
        EventSender<ISomeSubscriber> sender;

        internal TestHelper(EventSender<ISomeSubscriber> sender)
        {
            this.sender = sender;
        }

        public void SendEvent()
        {
            sender.SendEvent((ISomeSubscriber sub) => sub.OnTrigger());
        }
    }
}