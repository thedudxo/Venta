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
            sender.Send((ISomeSubscriber sub) => sub.OnTrigger());
        }
    }
}