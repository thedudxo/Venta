using DudCo.Events;
using NUnit.Framework;

namespace Tests.EventSenders;

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
