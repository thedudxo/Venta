using DudCo.Events;
using Tests;
using NUnit.Framework;
using System.Collections.Generic;

namespace Tests.EventSenders
{
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class EventSenderTests
    {
        EventSender<ISomeSubscriber> Event;
        SomeSubscriber subscriber;

        public EventSenderTests()
        {
            Event = new EventSender<ISomeSubscriber>();
            subscriber = new SomeSubscriber();

            Event.Subscribe(subscriber);
        }

        void SendEvent() => Event.Send((ISomeSubscriber sub) => sub.OnTrigger());
        void Send(EventSender<ISomeSubscriber> Event) => Event.Send((ISomeSubscriber sub) => sub.OnTrigger());

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
                Event.Unsubscribe(subscriber);

                SendEvent();

                Assert.False(subscriber.triggered);
            }

            [Test]
            public void AllreadySubscribed_Throws_ArgumentException()
            {
                Assert.Throws<System.ArgumentException>(
                    () => Event.Subscribe(subscriber));
            }

            [Test]
            public void UnsubscribingNotSubscribed_Throws_ArgumentException()
            {
                Event.Unsubscribe(subscriber);

                Assert.Throws<System.ArgumentException>(
                    () => Event.Unsubscribe(subscriber));
            }

            [Test]
            public void Unsubscribe_DuringEvent()
            {
                UnSubscribeDuringEvent sub = new UnSubscribeDuringEvent(Event);
                Event.Subscribe(sub);

                SendEvent();
                sub.triggered = false;
                SendEvent();

                Assert.False(sub.triggered);
            }

            [Test]
            public void SubscribeDuringEvent_TriggersNewSub()
            {
                SomeSubscriber createdSub = new SomeSubscriber();
                SubscribeDuringEvent subCreator = new SubscribeDuringEvent(Event, createdSub);
                Event.Subscribe(subCreator);

                SendEvent();
                SendEvent();

                Assert.True(createdSub.triggered);
            }

            [Test]
            public void SubscribeDuringEvent_DoesntTriggerEarly()
            {
                SomeSubscriber createdSub = new SomeSubscriber();
                SubscribeDuringEvent subCreator = new SubscribeDuringEvent(Event, createdSub);
                Event.Subscribe(subCreator);

                SendEvent();

                Assert.False(createdSub.triggered);
            }

            [Test]
            public void AllreadySubscribedDuringEvent_ThrowsArgumentException()
            {
                SomeSubscriber createdSub = new SomeSubscriber();
                SubscribeDuringEvent subCreator = new SubscribeDuringEvent(Event, createdSub);
                SubscribeDuringEvent subCreator2 = new SubscribeDuringEvent(Event, createdSub);
                Event.Subscribe(subCreator);
                Event.Subscribe(subCreator2);

                Assert.Throws<System.ArgumentException>(
                    () =>
                    SendEvent()
                );
            }

            [Test]
            public void AllreadyUnSubscribedDuringEvent_ThrowsArgumentException()
            {
                var unSub = new UnSubscribeTwiceDuringEvent(Event);

                Event.Subscribe(unSub);

                Assert.Throws<System.ArgumentException>(
                    () =>
                    SendEvent()
                );
            }
        }

        class Clear : EventSenderTests
        {

            [Test]
            public void Clear_SubscriberIsntTriggered()
            {
                Event.Clear();
                SendEvent();

                Assert.False(subscriber.triggered);
            }

            [Test]
            public void Clear_NewSubscriberDuringEvent_IsntTriggered()
            {
                SomeSubscriber createdSub = new SomeSubscriber();
                SubscribeDuringEvent subcreator = new SubscribeDuringEvent(Event, createdSub);
                Event.Subscribe(subcreator);

                SendEvent();
                Event.Clear();
                SendEvent();

                Assert.False(createdSub.triggered);
            }

        }

        class Priority : EventSenderTests
        {
            public Priority()
            {
                OrderedSubscriber.Clear();
            }

            [Test]
            public void Subscribe_WithPriority()
            {
                SomeSubscriber subscriber = new SomeSubscriber();
                Event.Subscribe(subscriber, 0);

                SendEvent();

                Assert.True(subscriber.triggered);
            }

            [Test]
            public void HigherPriority_CalledFirst()
            {
                OrderedSubscriber
                    sub = new OrderedSubscriber(),
                    prioritySub = new OrderedSubscriber();

                Event.Subscribe(sub, 0);
                Event.Subscribe(prioritySub, 1);

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

                Event.Subscribe(highSub1, 1);
                Event.Subscribe(highSub2, 1);
                Event.Subscribe(lowSub, 0);

                SendEvent();

                Assert.AreEqual(2, lowSub.triggeredAt);
            }

            [Test]
            public void NegativePriority_IsLowerThan_PositivePriority()
            {
                OrderedSubscriber
                    highSub = new OrderedSubscriber(),
                    lowSub = new OrderedSubscriber();

                Event.Subscribe(lowSub, -1);
                Event.Subscribe(highSub, 1);

                SendEvent();

                Assert.AreEqual(1, lowSub.triggeredAt);
            }
        }

        class RecursiveSend : EventSenderTests
        {
            [Test]
            public void SendingEvent_FromSubscriber_ThrowsException()
            {
                SubscriberThatSendsEvent evilSub = new SubscriberThatSendsEvent(Event);

                Event.Subscribe(evilSub);

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

            public PriorityDictionaryTests()
            {
                OrderedSubscriber.Clear();
            }

            class TestPriorities : PriorityDictionary
            {
                public TestPriorities()
                {
                    Add<OrderedSubscriberA>(1);
                    Add<OrderedSubscriberB>(2);
                    Add<OrderedSubscriberC>(3);
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
            public void SubscribeItemInPriorityDictionary_ThrowsException()
            {
                Event = CreateEvent();

                OrderedSubscriberA subA = new();

                Assert.Throws<System.ArgumentException>
                (() =>
                    Event.Subscribe(subA)
                );

            }

            [Test]
            public void SubscribeByRegisteredType_UsesPriorityFromDictionary()
            {
                Event = CreateEvent();

                OrderedSubscriberA subA = new();
                OrderedSubscriberB subB = new();

                Event.SubscribeByRegisteredType(subA);
                Event.SubscribeByRegisteredType(subB);

                SendEvent();

                Assert.AreEqual(1, subA.triggeredAt);

            }

            [Test]
            public void SubscribeByRegisteredType_ThrowsException_IfNotInPriorityDictionary()
            {
                Event = CreateEvent();

                Assert.Throws<System.ArgumentException>
                (() =>
                    Event.SubscribeByRegisteredType(subscriber)
                );
            }

            [Test]
            public void SubscribeByRegisteredType_ThrowsException_IfAllreadySubscribed()
            {
                Event = CreateEvent();

                OrderedSubscriberA subA = new();
                Event.SubscribeByRegisteredType(subA);

                Assert.Throws<System.ArgumentException>
                (() =>
                    Event.SubscribeByRegisteredType(subA)
                );
            }
        }

        class OnlyHighestPriority : EventSenderTests
        {
            [Test]
            public void LowerPriority_NotCalled()
            {
                var OnlyHighestPriorityEvent = new EventSender<ISomeSubscriber>();
                OnlyHighestPriorityEvent.SendMethod = SendMethod.OnlyHighestPriority;

                var lowPriority = new SomeSubscriber();
                var highPriority = new SomeSubscriber();

                OnlyHighestPriorityEvent.Subscribe(lowPriority, 0);
                OnlyHighestPriorityEvent.Subscribe(highPriority, 1);

                Send(OnlyHighestPriorityEvent);

                Assert.False(lowPriority.triggered);
            }

            [Test]
            public void HigherPriority_Called()
            {
                var OnlyHighestPriorityEvent = new EventSender<ISomeSubscriber>();
                OnlyHighestPriorityEvent.SendMethod = SendMethod.OnlyHighestPriority;

                var lowPriority = new SomeSubscriber();
                var highPriority = new SomeSubscriber();

                OnlyHighestPriorityEvent.Subscribe(lowPriority, 0);
                OnlyHighestPriorityEvent.Subscribe(highPriority, 1);

                Send(OnlyHighestPriorityEvent);

                Assert.True(highPriority.triggered);
            }

            [Test]
            public void AllSubscribers_InHighestPriorityBraket_Called()
            {
                var OnlyHighestPriorityEvent = new EventSender<ISomeSubscriber>();
                OnlyHighestPriorityEvent.SendMethod = SendMethod.OnlyHighestPriority;

                var lowPriority = new SomeSubscriber();
                var highPriority = new SomeSubscriber();
                var highPriority2 = new SomeSubscriber();

                OnlyHighestPriorityEvent.Subscribe(lowPriority, 0);
                OnlyHighestPriorityEvent.Subscribe(highPriority, 1);
                OnlyHighestPriorityEvent.Subscribe(highPriority2, 1);

                Send(OnlyHighestPriorityEvent);

                Assume.That(lowPriority.triggered == false);
                Assert.True(highPriority.triggered && highPriority2.triggered);
            }
        }
    }
}