using DudCo.Events;
using NUnit.Framework;

namespace Tests.EventSender;

public class Concurrent_Send_Exception
{
    [Test]
    public void Is_thrown_when_a_subscriber_sends_an_event_it_is_subscribed_to()
    {
        EventSender<ISomeSubscriber> eventSender = new();
        SubscriberThatSendsEvent evilSub = new(eventSender);

        eventSender.Subscribe(evilSub);

        Assert.Throws<ConcurrentSendException>(() =>
            eventSender.Send(s => s.OnTrigger()));
    }
}