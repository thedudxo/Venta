using NUnit.Framework;

namespace Tests.EventSenders;

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

        void act() => Event.Subscribe(subA);

        Assert.Throws<System.ArgumentException>(act);
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

        void act() => Event.SubscribeByRegisteredType(subscriber);

        Assert.Throws<System.ArgumentException>(act);
    }

    [Test]
    public void SubscribeByRegisteredType_ThrowsException_IfAllreadySubscribed()
    {
        Event = CreateEvent();
        OrderedSubscriberA subA = new();
        Event.SubscribeByRegisteredType(subA);

        void act() => Event.SubscribeByRegisteredType(subA);

        Assert.Throws<System.ArgumentException>(act);
    }
}
