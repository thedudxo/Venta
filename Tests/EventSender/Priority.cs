using NUnit.Framework;
using Tests.EventSender;

namespace Tests.EventSenders;

class Priority : EventSenderTests
{
    public Priority()
    {
        OrderedSubscriber.Clear();
    }

    [Test]
    public void Subscribe_WithPriority_Triggered()
    {
        SomeSubscriber subscriber = new();
        Event.Subscribe(subscriber, 0);

        SendEvent();

        Assert.True(subscriber.triggered);
    }

    [Test]
    public void Subscribe_With_Priority_Overloaded_Triggered()
    {
        SomeSubscriber subscriber = new();
        Event.Subscribe(0, subscriber);

        SendEvent();

        Assert.True(subscriber.triggered);
    }

    [Test]
    public void MultipleSubscribers_HigherPriority_TriggeredFirst()
    {
        OrderedSubscriber
            sub = new(),
            prioritySub = new();

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
            highSub1 = new(),
            highSub2 = new(),
            lowSub = new();

        Event.Subscribe(highSub1, 1);
        Event.Subscribe(highSub2, 1);
        Event.Subscribe(lowSub, 0);

        SendEvent();

        Assert.AreEqual(2, lowSub.triggeredAt);
    }

    [Test]
    public void NegativePriority_IsTriggeredAfter_PositivePriority()
    {
        OrderedSubscriber
            highSub = new(),
            lowSub = new();

        Event.Subscribe(lowSub, -1);
        Event.Subscribe(highSub, 1);

        SendEvent();

        Assert.AreEqual(1, lowSub.triggeredAt);
    }
}
