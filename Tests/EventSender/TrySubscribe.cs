using NUnit.Framework;
using Tests.EventSender;

namespace Tests.EventSenders;

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
