using DudCo.Events;
using Tests.EventSender;

namespace Tests.EventSenders;

class PriorityDictionaryHelper : EventSenderTests
{
    protected class OrderedSubscriberA : OrderedSubscriber { }
    protected class OrderedSubscriberB : OrderedSubscriber { }
    protected class OrderedSubscriberC : OrderedSubscriber { }

    public PriorityDictionaryHelper()
    {
        OrderedSubscriber.Clear();
    }

    protected class TestPriorities : PriorityDictionary
    {
        public TestPriorities()
        {
            Add<OrderedSubscriberA>(1);
            Add<OrderedSubscriberB>(2);
            Add<OrderedSubscriberC>(3);
        }
    }

    static readonly TestPriorities priorities = new();
    protected EventSender<ISomeSubscriber> CreateEvent() => new(priorities);
}
