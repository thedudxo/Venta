using NUnit.Framework;
using Tests.EventSender;

namespace Tests.EventSenders;

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
