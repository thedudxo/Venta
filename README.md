# Features
- Priorites
- Subscriptions during an event are queued untill it finishes.
- Optionally only notify subscribers once. After the initial event, any new subscribers are notified apon subscription.
- Optionally only notify subscribers with the highest priority.
- Create Priority Dictionaries that determine what order types should receive events in relative to eachother.

# Links
- Documentation: https://thedudxo.github.io/InterfaceEvents/index.html
- Nuget: https://www.nuget.org/packages/InterfaceEvents

# Getting Started

### Creating and subscribing to event
```
using DudCo.Events;

interface IOnEvent
{
    void OnEvent();
}

class EventReceiver : IOnEvent
{
    public void OnEvent()
    {
        //do event stuff
    }
}

class EventsExample
{
    public void Main()
    {
         EventSender<IOnEvent> someEvent = new EventSender<IOnEvent>();
         EventReceiver receiver = new EventReceiver()
         someEvent.Subscribe(reveiver);
         someEvent.Send(s => s.OnEvent());
    }
}
```

Items can only be subscribed to an event once, and you'll get a `System.ArgumentException` if you try to subscribe multiple times.

<hr>

### Subscribing with priority
`someEvent.Subscribe(something, 5)`

`someEvent.Subscribe(somethingElse, -2)`

<hr>

### Subscribing in bulk
`Event.Subscribe(a, b);`

when bulk subscribing, the priority must be specified first:

`Event.Subscribe(2, c, d, e);`

also works with unsubscribing:

`Event.Unsubscribe(a, b, c, d, e);`

<hr>

### TrySubscribe 
If the item you're trying to subscribe might allready be subscribed, you can use `TrySubscribe()` and `TryUnsubscribe()` methods. 
They will not throw exceptions, but instead return a sucsess/failure bool.

Dont use it everywhere, only if you need to, as it will hide bugs (by not throwing).

<hr>

### Sending event with parameters 
``` 
public interface IOnCollision
{
    void OnCollision(CollisionInfo collision);
}

private void SendCollisionEvent(CollisionInfo info)
{
    collisionEvent.Send(s => s.OnCollision(info));
}
```

<hr>


## Optional Features

### EventBuilder
The event builder can be used to create EventSenders with advanced features.

This will create the default EventSender, same as using `new EventSender()`:

```
EventSender<IOnEvent> myEvent = new EventBuilder<IOnEvent>()
            .Build();
```

<hr>

### Type Priority Dictionaries
Type priority dictionaries let you specify what priority all instances of a type will be given when subscribing via ``eventSender.SubscribeByRegisteredType()``

An item in the dictionary subscribed with ``eventSender.Subscribe()`` will throw an exception.

```   
public class MyPriorityDictionary : PriorityDictionary
    {
        public MyPriorityDictionary()
        {
            Add<ClassA>(0);
            Add<ClassB>(After<ClassA>());
            Add<ClassC>(Before<ClassB>());
        }
    }
```

Create by either passing in the constructor, or using an EventBuilder.

```
EventSender<IOnEvent> myEvent = new EventSender<IOnEvent>(new MyPriorityDictionary());
```


```
EventSender<IOnEvent> myEvent = new EventBuilder<IOnEvent>()
            .WithPriorityDictionary(new MyPriorityDictionary())
            .Build();
```


<hr>

### Only notify highest priority

This option will only send events to the subscriber with the highest priortiy.
all subscribers which share the highest priority are notified.

```
EventSender<IOnEvent> myEvent = new EventBuilder<IOnEvent>()
            .SendOnlyHighestPriority()
            .Build();
```

<hr>

### Only notify once

This option will only alow an event to be sent once. after which:
- current subscribers are unsubscribed
- new subscribers are not subscribed, but instead receive the event immediately

this means that the EventSender holds no references to the subscribers after the event has been sent, so they may be garbage collected.

this option is usefull for delaying the creation of dependant objects untill thier dependancies have been created.

trying to send the event multiple times will throw a `System.InvalidOperationException`.

```
EventSender<IOnEvent> myEvent = new EventBuilder<IOnEvent>()
            .SendOnlyOnce()
            .Build();
```
