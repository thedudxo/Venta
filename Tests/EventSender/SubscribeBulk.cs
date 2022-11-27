using NUnit.Framework;
using Tests.EventSender;

namespace Tests.EventSenders;

class SubscribeBulk : EventSenderTests
{
    [Test]
    public void BulkSubscription_EveryoneReceivesEvents()
    {
        SomeSubscriber
            a = new(),
            b = new();

        Event.Subscribe(a, b);

        SendEvent();

        Assert.True(a.triggered);
        Assert.True(b.triggered);
    }

    [Test]
    public void BulkSubscription_ItemsDefaultTo0Priority()
    {
        OrderedSubscriber
            a = new(),
            b = new(),
            c = new();


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
            a = new(),
            b = new(),
            c = new();


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
            OrderedSubscriberA a = new();
            OrderedSubscriberB b = new();
            OrderedSubscriberC c = new();

            Assert.Throws<System.ArgumentException>(() =>
                Event.Subscribe(a, b, c)
            );
        }

        [Test]
        public void SubscribeBulk_ByRegistered_ItemInPriorityDictionary()
        {
            Event = CreateEvent();
            OrderedSubscriberA a = new();
            OrderedSubscriberB b = new();
            OrderedSubscriberC c = new();

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
           a = new(),
           b = new();

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
           a = new(),
           b = new();

        Assert.Throws<System.ArgumentException>(() =>
            Event.Unsubscribe(a, b)
        );
    }
}
