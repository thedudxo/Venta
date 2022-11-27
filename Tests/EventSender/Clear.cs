using NUnit.Framework;
using Tests.EventSender;

namespace Tests.EventSenders;

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
        SomeSubscriber createdSub = new();
        SubscribeDuringEvent subcreator = new(Event, createdSub);
        Event.Subscribe(subcreator);

        SendEvent();
        Event.Clear();
        SendEvent();

        Assert.False(createdSub.triggered);
    }

}
