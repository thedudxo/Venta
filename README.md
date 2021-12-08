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
