using NUnit.Framework;
using DudCo.Events;

namespace Tests
{
    public class EventPriorityTests
    {
        class OrderedSubscriber : ISomeSubscriber
        {
            static int triggerCount = 0;
            public int triggeredAt = 0;
            public bool triggered = false;

            public void OnTrigger()
            {
                triggered = true;
                triggeredAt = triggerCount;
                triggerCount++;
            }
        }
        

        EventSender<ISomeSubscriber> sender;
        
        TestHelper helper;

        void SendEvent() => helper.SendEvent();

        [SetUp]
        public void SetUp()
        {
            sender = new EventSender<ISomeSubscriber>();
            
            helper = new TestHelper(sender);
        }

        [Test] 
        public void Subscribe_WithPriority()
        {
            SomeSubscriber subscriber = new SomeSubscriber();
            sender.Subscribe(subscriber, 0);

            SendEvent();

            Assert.True(subscriber.triggered);
        }

        [Test]
        public void HigherPriority_CalledFirst()
        {
            OrderedSubscriber
                sub = new OrderedSubscriber(),
                prioritySub = new OrderedSubscriber();

            sender.Subscribe(sub, 0);
            sender.Subscribe(prioritySub, 1);

            SendEvent();

            Assert.AreEqual(1, sub.triggeredAt);
            Assert.AreEqual(0, prioritySub.triggeredAt);
        }
    }
}