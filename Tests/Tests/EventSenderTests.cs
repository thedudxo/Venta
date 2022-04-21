using DudCo.Events;
using NUnit.Framework;

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
        static void Send(EventSender<ISomeSubscriber> Event) => Event.Send((ISomeSubscriber sub) => sub.OnTrigger());

        class PriorityDictionaryHelper : EventSenderTests
        {
            protected class OrderedSubscriberA : OrderedSubscriber { }
            protected class OrderedSubscriberB : OrderedSubscriber { }
            protected class OrderedSubscriberC : OrderedSubscriber { }

            public PriorityDictionaryHelper()
            {
                OrderedSubscriber.Clear();
            }

            protected class TestPriorities : PriorityDictionary
            {
                public TestPriorities()
                {
                    Add<OrderedSubscriberA>(1);
                    Add<OrderedSubscriberB>(2);
                    Add<OrderedSubscriberC>(3);
                }
            }

            static readonly TestPriorities priorities = new();
            protected EventSender<ISomeSubscriber> CreateEvent() => new(priorities);
        }

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

        class PriorityDictionaryTests : PriorityDictionaryHelper
        {

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
            static EventSender<ISomeSubscriber> BuildEventSender()
            {
                return new EventBuilder<ISomeSubscriber>()
                    .SendOnlyHighestPriority()
                    .Build();
            }

            [Test]
            public void LowerPriority_NotCalled()
            {
                var OnlyHighestPriorityEvent = BuildEventSender();

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
                var OnlyHighestPriorityEvent = BuildEventSender();

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
                var OnlyHighestPriorityEvent = BuildEventSender();

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

        class SubscribeBulk : EventSenderTests
        {
            [Test]
            public void BulkSubscription_EveryoneReceivesEvents()
            {
                SomeSubscriber
                    a = new SomeSubscriber(),
                    b = new SomeSubscriber();

                Event.Subscribe(a, b);

                SendEvent();

                Assert.True(a.triggered);
                Assert.True(b.triggered);
            }

            [Test]
            public void BulkSubscription_ItemsDefaultTo0Priority()
            {
                OrderedSubscriber
                    a = new OrderedSubscriber(),
                    b = new OrderedSubscriber(),
                    c = new OrderedSubscriber();


                Event.Subscribe(c, 1);
                Event.Subscribe(a, b);

                SendEvent();

                Assert.Greater(a.triggeredAt, c.triggeredAt);
                Assert.Greater(b.triggeredAt, c.triggeredAt);
            }

            [Test]
            public void BulkSubscription_GivenPriority_Correct()
            {
                OrderedSubscriber
                    a = new OrderedSubscriber(),
                    b = new OrderedSubscriber(),
                    c = new OrderedSubscriber();


                Event.Subscribe(c, 1);
                Event.Subscribe(2, a, b);

                SendEvent();

                Assert.Less(a.triggeredAt, c.triggeredAt);
                Assert.Less(b.triggeredAt, c.triggeredAt);
            }

            class SubscribeBulk_PriorityDictionary : PriorityDictionaryHelper
            {

                [Test]
                public void SubscribeBulk_ItemInPriorityDictionary_ThrowsException()
                {
                    Event = CreateEvent();
                    OrderedSubscriberA a = new OrderedSubscriberA();
                    OrderedSubscriberB b = new OrderedSubscriberB();
                    OrderedSubscriberC c = new OrderedSubscriberC();

                    Assert.Throws<System.ArgumentException>(() =>
                        Event.Subscribe(a, b, c)
                    );
                }

                [Test]
                public void SubscribeBulk_ByRegistered_ItemInPriorityDictionary()
                {
                    Event = CreateEvent();
                    OrderedSubscriberA a = new OrderedSubscriberA();
                    OrderedSubscriberB b = new OrderedSubscriberB();
                    OrderedSubscriberC c = new OrderedSubscriberC();

                    Event.SubscribeByRegisteredType(a, b, c);

                    SendEvent();

                    Assert.AreEqual(2, a.triggeredAt);
                    Assert.AreEqual(1, b.triggeredAt);
                    Assert.AreEqual(0, c.triggeredAt);
                }
            }

            [Test]
            public void UnsubscribeBulk_NoEventsReceived()
            {
                SomeSubscriber
                   a = new SomeSubscriber(),
                   b = new SomeSubscriber();

                Event.Subscribe(a, b);

                Event.Unsubscribe(a, b);
                SendEvent();

                Assert.False(a.triggered);
                Assert.False(b.triggered);
            }

            [Test]
            public void UnsubscribeBulk_NotSubscribed_throwsException()
            {
                SomeSubscriber
                   a = new SomeSubscriber(),
                   b = new SomeSubscriber();

                Assert.Throws<System.ArgumentException>(() =>
                    Event.Unsubscribe(a, b)
                );
            }
        }

        class SendOnlyOnce : PriorityDictionaryHelper
        {

            static EventSender<ISomeSubscriber> BuildEventSender()
            {
                return new EventBuilder<ISomeSubscriber>()
                    .SendOnlyOnce()
                    .Build();
            }

            [Test]
            public void SendTwice_ThrowsException()
            {
                Event = BuildEventSender();

                SendEvent();
                Assert.Throws<System.InvalidOperationException>(() => SendEvent());
            }

            [Test]
            public void SendOnce_ReceivesEvent()
            {
                Event = BuildEventSender();

                Event.Subscribe(subscriber);
                SendEvent();

                Assert.True(subscriber.triggered);
            }

            [Test]
            public void SendTwice_DoesNotReceiveEvent()
            {
                Event = BuildEventSender();
                Event.Subscribe(subscriber);

                SendEvent();
                Assume.That(subscriber.triggered);

                subscriber.triggered = false;

                try
                {
                    SendEvent();
                }
                catch (System.InvalidOperationException)
                {

                }

                Assert.False(subscriber.triggered);
            }

            [Test]
            public void SubscribedItems_AreUnsubscribed_AfterSend()
            {
                Event = BuildEventSender();
                Event.Subscribe(subscriber);

                SendEvent();

                Assert.Throws<System.ArgumentException>(() => Event.Unsubscribe(subscriber));
            }

            [Test]
            public void UnsubscribedItems_DoesNotReceiveEvent()
            {
                Event = BuildEventSender();
                Event.Subscribe(subscriber);
                Event.Unsubscribe(subscriber);

                SendEvent();

                Assert.False(subscriber.triggered);
            }


            [Test]
            public void SubscribeLate_ReceivesEventImmediately()
            {
                Event = BuildEventSender();
                SendEvent();

                Event.Subscribe(subscriber);

                Assert.True(subscriber.triggered);
            }


            [Test]
            public void SubscribeByRegisteredType_Late_ReceivesEventImmediately()
            {
                Event = new EventBuilder<ISomeSubscriber>()
                    .SendOnlyOnce()
                    .WithPriorityDictionary(new TestPriorities())
                    .Build();
                SendEvent();

                var sub = new OrderedSubscriberB();

                Event.SubscribeByRegisteredType(sub);

                Assert.True(sub.triggered);
            }

            [Test]
            public void SubscribeBulk_Late_ReceivesEventImmediately()
            {
                Event = BuildEventSender();
                SendEvent();

                var subB = new SomeSubscriber();
                Event.Subscribe(subscriber, subB);

                Assert.True(subscriber.triggered);
                Assert.True(subB.triggered);
            }
        }

        class TrySubscribe : EventSenderTests
        {
            [Test]
            public void TrySubscribe_AllreadySubscribed_DoesNotThrowException()
            {
                void testDelegate() => Event.TrySubscribe(subscriber);

                Assert.DoesNotThrow(testDelegate);
            }

            [Test]
            public void TrySubscribe_AllreadySubscribed_ReturnsFalse()
            {
                bool result = Event.TrySubscribe(subscriber);

                Assert.False(result);
            }

            [Test]
            public void TrySubscribe_AllreadySubscribed_NotSubscribedAgain()
            {
                Event.TrySubscribe(subscriber);
                SendEvent();

                Assert.AreEqual(1, subscriber.triggerCount);
            }

            [Test]
            public void TrySubscribe_NotSubscribed_DoesNotThrowException()
            {
                var sub = new SomeSubscriber();
                void testDelegate() => Event.TrySubscribe(sub);

                Assert.DoesNotThrow(testDelegate);
            }

            [Test]
            public void TrySubscribe_NotSubscribed_ReturnsTrue()
            {
                var sub = new SomeSubscriber();
                bool result = Event.TrySubscribe(sub);

                Assert.True(result);
            }

            [Test]
            public void TrySubscribe_NotSubscribed_IsSubscribed()
            {
                var sub = new SomeSubscriber();
                Event.TrySubscribe(sub);
                SendEvent();

                Assert.True(sub.triggered);
            }
        }

        class TryUnsubscribe : EventSenderTests
        {
            [Test]
            public void TryUnsubscribe_AllreadySubscribed_DoesNotThrowException()
            {
                void testDelegate() => Event.TryUnsubscribe(subscriber);

                Assert.DoesNotThrow(testDelegate);
            }

            [Test]
            public void TryUnsubscribe_AllreadySubscribed_ReturnsTrue()
            {
                bool result = Event.TryUnsubscribe(subscriber);

                Assert.True(result);
            }

            [Test]
            public void TryUnsubscribe_NotSubscribed_DoesNotThrowException()
            {
                var sub = new SomeSubscriber();
                void testDelegate() => Event.TryUnsubscribe(sub);

                Assert.DoesNotThrow(testDelegate);
            }

            [Test]
            public void TryUnsubscribe_NotSubscribed_ReturnsFalse()
            {
                var sub = new SomeSubscriber();
                bool result = Event.TryUnsubscribe(sub);

                Assert.False(result);
            }

            [Test]
            public void TryUnsubscribe_Subscribed_IsUnsubscribed()
            {
                Event.TryUnsubscribe(subscriber);
                SendEvent();

                Assert.False(subscriber.triggered);
            }
        }
    }
}