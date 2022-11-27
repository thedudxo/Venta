namespace Tests.EventSender;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
abstract class EventSenderTests
{
    protected EventSender<ISomeSubscriber> Event;
    protected SomeSubscriber subscriber;

    public EventSenderTests()
    {
        Event = new EventSender<ISomeSubscriber>();
        subscriber = new SomeSubscriber();

        Event.Subscribe(subscriber);
    }

    protected void SendEvent() => Event.Send((sub) => sub.OnTrigger());
    protected static void Send(EventSender<ISomeSubscriber> Event) => Event.Send((sub) => sub.OnTrigger());

}