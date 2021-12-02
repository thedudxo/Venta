using NUnit.Framework;
using DudCo.Events;

namespace Tests
{
    interface ISomeSubscriber { void OnTrigger(); }
    class SomeSubscriber : ISomeSubscriber
    {
        public bool triggered = false;
        public void OnTrigger() => triggered = true;
    }

    class UnSubscribeDuringEvent : ISomeSubscriber
    {
        EventSender<ISomeSubscriber> sender;

        public UnSubscribeDuringEvent(EventSender<ISomeSubscriber> sender)
        {
            this.sender = sender;
        }

        public bool triggered = false;

        public void OnTrigger()
        {
            triggered = true;
            sender.Unsubscribe(this);
        }
    }

    class SubscribeDuringEvent : ISomeSubscriber
    {
        EventSender<ISomeSubscriber> sender;
        ISomeSubscriber newSub;
        public bool hasSubbed = false;

        public SubscribeDuringEvent(EventSender<ISomeSubscriber> sender, ISomeSubscriber newSub)
        {
            this.sender = sender;
            this.newSub = newSub;
        }

        public bool triggered = false;

        public void OnTrigger()
        {
            triggered = true;
            if (hasSubbed == false)
            {
                sender.Subscribe(newSub);
                hasSubbed = true;
            }
        }
    }

    public class EventSenderTests
    {
        EventSender<ISomeSubscriber> sender;
        SomeSubscriber receiver;

        [SetUp]
        public void Setup()
        {
            sender = new EventSender<ISomeSubscriber>();
            receiver = new SomeSubscriber();

            sender.Subscribe(receiver);
        }

        void SendEvent()
        {
            sender.SendEvent((ISomeSubscriber sub) => sub.OnTrigger());
        }

        [Test]
        public void SubscriberGetsNotified()
        {
            SendEvent();

            Assert.True(receiver.triggered);
        }

        [Test]
        public void Unsubscribe_Doesnt_GetNotified()
        {
            sender.Unsubscribe(receiver);

            SendEvent();

            Assert.False(receiver.triggered);
        }

        [Test]
        public void AllreadySubscribed_Throws_InvaildOpException() 
        {
            Assert.Throws<System.InvalidOperationException>(
                () => sender.Subscribe(receiver));
        }

        [Test]
        public void UnsubscribingNotSubscribed_Throws_InvaildOpException() 
        {
            sender.Unsubscribe(receiver);

            Assert.Throws<System.InvalidOperationException>(
                () => sender.Unsubscribe(receiver));
        }

        [Test]
        public void Unsubscribe_DuringEvent()
        {
            UnSubscribeDuringEvent sub = new UnSubscribeDuringEvent(sender);
            sender.Subscribe(sub);

            SendEvent();
            sub.triggered = false;
            SendEvent();

            Assert.False(sub.triggered);
        }

        [Test]
        public void SubscribeDuringEvent_TriggersNewSub()
        {
            SomeSubscriber sub2 = new SomeSubscriber();
            SubscribeDuringEvent sub = new SubscribeDuringEvent(sender, sub2);
            sender.Subscribe(sub);

            SendEvent();
            SendEvent();

            Assert.True(sub2.triggered);
        }

        [Test]
        public void SubscribeDuringEvent_DoesntTriggerEarly()
        {
            SomeSubscriber sub2 = new SomeSubscriber();
            SubscribeDuringEvent sub = new SubscribeDuringEvent(sender, sub2);
            sender.Subscribe(sub);

            SendEvent();

            Assert.False(sub2.triggered);
        }
    }
}