using DudCo.Events;
using NUnit.Framework;
using System.Collections.Generic;

namespace Tests
{
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public partial class EventSenderTests
    {
        EventSender<ISomeSubscriber> _event;
        SomeSubscriber subscriber;

        public EventSenderTests()
        {
            _event = new EventSender<ISomeSubscriber>();
            subscriber = new SomeSubscriber();

            _event.Subscribe(subscriber);
        }

        void SendEvent() => _event.Send((ISomeSubscriber sub) => sub.OnTrigger());

        class Subscribe : EventSenderTests
        {

            [Test]
            public void SubscriberGetsNotified()
            {
                SendEvent();

                Assert.True(subscriber.triggered);
            }

            [Test]
            public void Unsubscribe_Doesnt_GetNotified()
            {
                _event.Unsubscribe(subscriber);

                SendEvent();

                Assert.False(subscriber.triggered);
            }

            [Test]
            public void AllreadySubscribed_Throws_InvaildOpException()
            {
                Assert.Throws<System.InvalidOperationException>(
                    () => _event.Subscribe(subscriber));
            }

            [Test]
            public void UnsubscribingNotSubscribed_Throws_InvaildOpException()
            {
                _event.Unsubscribe(subscriber);

                Assert.Throws<System.InvalidOperationException>(
                    () => _event.Unsubscribe(subscriber));
            }

            [Test]
            public void Unsubscribe_DuringEvent()
            {
                UnSubscribeDuringEvent sub = new UnSubscribeDuringEvent(_event);
                _event.Subscribe(sub);

                SendEvent();
                sub.triggered = false;
                SendEvent();

                Assert.False(sub.triggered);
            }

            [Test]
            public void SubscribeDuringEvent_TriggersNewSub()
            {
                SomeSubscriber createdSub = new SomeSubscriber();
                SubscribeDuringEvent subCreator = new SubscribeDuringEvent(_event, createdSub);
                _event.Subscribe(subCreator);

                SendEvent();
                SendEvent();

                Assert.True(createdSub.triggered);
            }

            [Test]
            public void SubscribeDuringEvent_DoesntTriggerEarly()
            {
                SomeSubscriber createdSub = new SomeSubscriber();
                SubscribeDuringEvent subCreator = new SubscribeDuringEvent(_event, createdSub);
                _event.Subscribe(subCreator);

                SendEvent();

                Assert.False(createdSub.triggered);
            }
        }

        class Clear : EventSenderTests
        {

            [Test]
            public void Clear_SubscriberIsntTriggered()
            {
                _event.Clear();
                SendEvent();

                Assert.False(subscriber.triggered);
            }

            [Test]
            public void Clear_NewSubscriberDuringEvent_IsntTriggered()
            {
                SomeSubscriber createdSub = new SomeSubscriber();
                SubscribeDuringEvent subcreator = new SubscribeDuringEvent(_event, createdSub);
                _event.Subscribe(subcreator);

                SendEvent();
                _event.Clear();
                SendEvent();

                Assert.False(createdSub.triggered);
            }

        }

        partial class Priority : EventSenderTests
        {
            public Priority()
            {
                OrderedSubscriber.Clear();
            }

            [Test]
            public void Subscribe_WithPriority()
            {
                SomeSubscriber subscriber = new SomeSubscriber();
                _event.Subscribe(subscriber, 0);

                SendEvent();

                Assert.True(subscriber.triggered);
            }

            [Test]
            public void HigherPriority_CalledFirst()
            {
                OrderedSubscriber
                    sub = new OrderedSubscriber(),
                    prioritySub = new OrderedSubscriber();

                _event.Subscribe(sub, 0);
                _event.Subscribe(prioritySub, 1);

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

                _event.Subscribe(highSub1, 1);
                _event.Subscribe(highSub2, 1);
                _event.Subscribe(lowSub, 0);

                SendEvent();

                Assert.AreEqual(2, lowSub.triggeredAt);
            }

            [Test]
            public void NegativePriority_IsLowerThan_PositivePriority()
            {
                OrderedSubscriber
                    highSub = new OrderedSubscriber(),
                    lowSub = new OrderedSubscriber();

                _event.Subscribe(lowSub, -1);
                _event.Subscribe(highSub, 1);

                SendEvent();

                Assert.AreEqual(1, lowSub.triggeredAt);
            }
        }

        class RecursiveSend : EventSenderTests
        {
            [Test]
            public void SendingEvent_FromSubscriber_ThrowsException()
            {
                SubscriberThatSendsEvent evilSub = new SubscriberThatSendsEvent(_event);

                _event.Subscribe(evilSub);

                Assert.Throws<System.InvalidOperationException>(
                    () => SendEvent()
                    );
            }
        }

        class PriorityDictionaryTests : EventSenderTests
        {
            class OrderedSubscriberA : OrderedSubscriber { }
            class OrderedSubscriberB : OrderedSubscriber { }
            class OrderedSubscriberC : OrderedSubscriber { }

            static readonly Dictionary<char, int> priorityByChar = new Dictionary<char, int>
            {
                {'a', 0},
                {'b', 1},
                {'c', 2},
            };

            class TestPriorities : PriorityDictionary
            {
                public TestPriorities()
                {
                    Add<OrderedSubscriberA>(priorityByChar['a']);
                    Add<OrderedSubscriberB>(priorityByChar['b']);
                    Add<OrderedSubscriberC>(priorityByChar['c']);
                }
            }

            static readonly TestPriorities priorities = new();
            EventSender<ISomeSubscriber> CreateEvent() => new(priorities);


            [Test]
            public void CreateEventSender_WithPriorityDictionary()
            {
                Assert.DoesNotThrow(() => CreateEvent());
            }

            [Test]
            public void SubscribeItemInPriorityDictionary_UsesThatPriority()
            {
                _event = CreateEvent();

                OrderedSubscriberA subA = new();
                OrderedSubscriberB subB = new();

                _event.Subscribe(subA);
                _event.Subscribe(subB);

                Assert.AreEqual(priorityByChar['b'], subB.triggeredAt);
            }
        }
    }
}