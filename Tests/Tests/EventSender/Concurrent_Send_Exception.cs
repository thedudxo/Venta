using DudCo.Events;
using NUnit.Framework;

namespace Tests.Tests.EventSender;

partial class Concurrent_Send_Exception : EventSenderTests
{

    [Test]
    public void SendingEvent_FromSubscriberOfThatEvent_ThrowsException()
    {
        EventSender<ISomeSubscriber> eventSender = new();
        SubscriberThatSendsEvent evilSub = new(eventSender);

        eventSender.Subscribe(evilSub);

        Assert.Throws<ConcurrentSendException>(() =>
            eventSender.Send(s => s.OnTrigger()));
    }
}