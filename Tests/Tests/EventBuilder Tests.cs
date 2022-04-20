using DudCo.Events;
using NUnit.Framework;

namespace Tests.EventBuilders
{
    internal class EventBuilder_Tests
    {
        EventBuilder<ISomeSubscriber> builder;
        EventSender<ISomeSubscriber> eventSender;

        [SetUp]
        public void SetUp()
        {
            builder = new EventBuilder<ISomeSubscriber>();
        }

        public void SendEvent() => eventSender.Send((ISomeSubscriber sub) => sub.OnTrigger());

        [Test] 
        public void BuildDefault_CreatesEmptyPriorityDictionary()
        {
            var sub = new SomeSubscriber();
            
            eventSender = builder.Build();

            Assert.DoesNotThrow(
                () => eventSender.Subscribe(sub, 1)
                );
        }

        [Test]
        public void BuildDefault_CreatesDefuatSendStratergy()
        {
            eventSender = builder.Build();

            Assert.DoesNotThrow( () => SendEvent() );
        }

        class OrderedSubscriberA : OrderedSubscriber { }
        class OrderedSubscriberB : OrderedSubscriber { }

        class TestPriorities : PriorityDictionary
        {
            public TestPriorities()
            {
                Add<OrderedSubscriberA>(1);
                Add<OrderedSubscriberB>(0);
            }
        }

        [Test]
        public void BuildWith_PriorityDictionary()
        {
            var subA = new OrderedSubscriberA();
            var subB = new OrderedSubscriberB();
            var priorities = new TestPriorities();

            builder.WithPriorityDictionary(priorities);
            eventSender = builder.Build();

            eventSender.SubscribeByRegisteredType(subA, subB);
            SendEvent();

            Assert.AreEqual(0, subA.triggeredAt);
            Assert.AreEqual(1, subB.triggeredAt);
        }

        [Test]
        public void BuildWith_SendToHighestPriority()
        {
            var subA = new SomeSubscriber();
            var subB = new SomeSubscriber();

            builder.WithSendToHighestPriority();
            eventSender = builder.Build();

            eventSender.Subscribe(subA, 0);
            eventSender.Subscribe(subB, 10);

            SendEvent();

            Assert.True(subB.triggered);
            Assert.False(subA.triggered);
        }
    }
}
