using NUnit.Framework;
using DudCo.Events;

namespace Tests
{
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class EventPriorityTests
    {
        class OrderedSubscriber : ISomeSubscriber
        {
            string _name;

            public OrderedSubscriber(string name ="")
            {
                _name = name;
            }

            static int triggerCount = 0;
            public int triggeredAt = 0;
            public bool triggered = false;

            public void OnTrigger()
            {
                triggered = true;
                triggeredAt = triggerCount;
                triggerCount++;
            }

            public static void Clear() => triggerCount = 0;
        }
        

        EventSender<ISomeSubscriber> sender;
        

        void SendEvent() => sender.Send((ISomeSubscriber sub) => sub.OnTrigger());

        public EventPriorityTests()
        {
            sender = new EventSender<ISomeSubscriber>();

            OrderedSubscriber.Clear();
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

        [Test]
        public void AllItemsInOnePriorty_CalledBeforeNextPrioritiy()
        {
            OrderedSubscriber
                highSub1 = new OrderedSubscriber(),
                highSub2 = new OrderedSubscriber(),
                lowSub = new OrderedSubscriber();

            sender.Subscribe(highSub1, 1);
            sender.Subscribe(highSub2, 1);
            sender.Subscribe(lowSub, 0);

            SendEvent();

            Assert.AreEqual(2, lowSub.triggeredAt);
        }

        [Test]
        public void NegativePriority_IsLowerThan_PositivePriority()
        {
            OrderedSubscriber
                highSub = new OrderedSubscriber(),
                lowSub = new OrderedSubscriber();

            sender.Subscribe(lowSub, -1);
            sender.Subscribe(highSub, 1);

            SendEvent();

            Assert.AreEqual(1, lowSub.triggeredAt);
        }
    }
}