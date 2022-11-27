using Tests.EventSender;

namespace Tests.EventSenders;

sealed class Subscribe : EventSenderTests
{

    [Test]
    public void Subscribed_item_is_notified()
    {
        SendEvent();

        Assert.True(subscriber.triggered);
    }

    [Test]
    public void Item_that_unsubscribed_isnt_notified()
    {
        Event.Unsubscribe(subscriber);

        SendEvent();

        Assert.False(subscriber.triggered);
    }

    [Test]
    public void Item_that_is_not_subscriberd_isnt_notified()
    {
        //this tests the mock more so than the library
        SomeSubscriber notSubscribed = new();

        SendEvent();

        Assert.False(notSubscribed.triggered);
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
    public void Unsubscribe_DuringEvent_NotTriggeredNextTime()
    {
        UnSubscribeDuringEvent sub = new(Event);
        Event.Subscribe(sub);

        SendEvent();
        sub.triggered = false;
        SendEvent();

        Assert.False(sub.triggered);
    }

    [Test]
    public void SubscribeDuringEvent_TriggersNewSubNextTime()
    {
        SomeSubscriber createdSub = new();
        SubscribeDuringEvent subCreator = new(Event, createdSub);
        Event.Subscribe(subCreator);

        SendEvent();
        SendEvent();

        Assert.True(createdSub.triggered);
    }

    [Test]
    public void SubscribeDuringEvent_DoesntTriggerThisTime()
    {
        SomeSubscriber createdSub = new();
        SubscribeDuringEvent subCreator = new(Event, createdSub);
        Event.Subscribe(subCreator);

        SendEvent();

        Assert.False(createdSub.triggered);
    }

    [Test]
    public void AllreadySubscribedDuringEvent_ThrowsArgumentException()
    {
        SomeSubscriber createdSub = new();
        SubscribeDuringEvent subCreator = new(Event, createdSub);
        SubscribeDuringEvent subCreator2 = new(Event, createdSub);
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
        UnSubscribeTwiceDuringEvent unSub = new(Event);

        Event.Subscribe(unSub);

        Assert.Throws<System.ArgumentException>(
            () =>
            SendEvent()
        );
    }
}
