using DudCo.Events;
using NUnit.Framework;
using Tests.EventSender;

namespace Tests.EventSenders;

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
