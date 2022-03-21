# Prioritised Interface Events. 
Items with a higher priority are called first. negative priorities are fine.
Subscribing/Unsubscribing during an event will take effect afterwards

### Why interfaces?
- No unesseceary Sender/Args parameters (though you can add them if needed)
- more explicit declerations:
  - It's immedately clear when a class uses events
  - Guarenteed to use the same method names
- personally prefer ".Subscribe(x)" to "+="

### Cons
- Not the standard way of doing it
- Sending an event has slighty weird syntax:
    - >exampleEvent.Send((ISubscriberInterface sub) => sub.OnEvent());
    - This is to allow any method signature to be used for receving the event.


## Examples

### Creating and subscribing to event
```
using DudCo.Events;

public class SomeEventSender
{
  public EventSender<INotifyOnAnyRespawn> someEvent = new EventSender<INotifyOnSomeEvent>();
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
public void SubscribeSomething()
{
  someEvent.Subscribe(something, 5)
}
```

my own project uses a dictionary to store what priorites things should have. Optional, but usefull to have.
```
public static readonly Dictionary<Type, int> SetupPriorities;

public static void SetupPriorityDictionary()
{
    Add<Namespace.ClassA>(0);
    Add<Namespace.ClassB>(After<Namespace.ClassA>());
    Add<Namespace.ClassC>(After<Namespace.ClassB>());
}

static void Add<T>(int priority) => SetupPriorities.Add(typeof(T), priority);
static int PriorityOf<T>() => SetupPriorities[typeof(T)];
static int After<T>() => PriorityOf<T>() - 1;
static int Before<T>() => PriorityOf<T>() + 1;
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
