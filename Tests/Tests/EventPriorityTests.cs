using NUnit.Framework;
using DudCo.Events;

namespace Tests
{
    public class EventPriorityTests
    {
        EventSender<ISomeSubscriber> sender;
        SomeSubscriber receiver;
        TestHelper helper;

        void SendEvent() => helper.SendEvent();

        [SetUp]
        public void SetUp()
        {
            sender = new EventSender<ISomeSubscriber>();
            receiver = new SomeSubscriber();
            helper = new TestHelper(sender);
        }

        [Test] 
        public void Subscribe_WithPriority()
        {
            sender.Subscribe(receiver, 0);

            SendEvent();

            Assert.True(receiver.triggered);
        }
    }
}