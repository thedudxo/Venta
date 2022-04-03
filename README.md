Prioritised Interface Events. 

## Features
- Items with a higher priority are called first. negative priorities are fine.
- optionally only notify subscribers with the highest priority.
- Subscribing/Unsubscribing during an event will take effect afterwards.
- Create Priority Dictionaries that determine what order types should receive events in relative to eachother.

## Why interfaces?

### Pros
- No unesseceary Sender/Args parameters (though you can add them if needed)
- more explicit declerations:
  - It's immedately clear when a class receives events
  - Guarenteed to use the same method names
- personally prefer ".Subscribe(x)" to "+="
- The above features I've been able to add

### Cons
- Not core to c#
- Sending an event has slighty weird syntax:
    - ```myEvent.Send((IOnMyEvent subscriber) => subscriber.OnMyEvent());```
    - This is to allow any method signature to be used for receving the event.


## Examples

### Creating and subscribing to event
```
using DudCo.Events;

public class SomeEventSender
{
  public EventSender<INotifyOnSomeEvent> someEvent = new EventSender<INotifyOnSomeEvent>();
}

public class SomeEventReceiver : INotifyOnSomeEvent
{
  public SomeEventReveiver(EventSender someEvent)
  {
    someEvent.Subscribe(this)
  }
}
```

### Subscribing with priority
```
someEvent.Subscribe(something, 5)
someEvent.Subscribe(somethingElse, -2)
```

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

```
public EventSender<ISetup> myEvent = new EventSender<ISetup>(new MyPriorityDictionary());
```

### Sending event
```
public interface ISetup
{
    void Setup();
}

public void SendEvents()
{ 
    setupEvent.Send((ISetup subscriber) => subscriber.Setup());
}
```

### Sending event with parameters 
``` 
public interface IOnCollision
{
    void OnCollision(CollisionInfo collision);
}

private void SendCollisionEvent(CollisionInfo info)
{
    void action(IOnCollision subscriber) => subscriber.OnCollision(info);
    collisionEvent.Send(action);
}
```

### Only notify highest priority
```
MyEvent.SendMethod = EventSendMethod.OnlyHighestPriority;
```
