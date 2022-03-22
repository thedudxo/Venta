using NUnit.Framework;
using DudCo.Events;

namespace Tests
{
    public partial class EventSenderTests
    {
        EventSender<ISomeSubscriber> sender;
        SomeSubscriber receiver;
        TestHelper helper;

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

        [SetUp]
        public void Setup()
        {
            sender = new EventSender<ISomeSubscriber>();
            receiver = new SomeSubscriber();
            helper = new TestHelper(sender);

            sender.Subscribe(receiver);
        }

        void SendEvent() => helper.SendEvent();

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
            SomeSubscriber createdSub = new SomeSubscriber();
            SubscribeDuringEvent subCreator = new SubscribeDuringEvent(sender, createdSub);
            sender.Subscribe(subCreator);

            SendEvent();
            SendEvent();

            Assert.True(createdSub.triggered);
        }

        [Test]
        public void SubscribeDuringEvent_DoesntTriggerEarly()
        {
            SomeSubscriber createdSub = new SomeSubscriber();
            SubscribeDuringEvent subCreator = new SubscribeDuringEvent(sender, createdSub);
            sender.Subscribe(subCreator);

            SendEvent();

            Assert.False(createdSub.triggered);
        }

        [Test]
        public void Clear_SubscriberIsntTriggered()
        {
            sender.Clear();
            SendEvent();

            Assert.False(receiver.triggered);
        }

        [Test]
        public void Clear_NewSubscriberDuringEvent_IsntTriggered()
        {
            SomeSubscriber createdSub = new SomeSubscriber();
            SubscribeDuringEvent subcreator = new SubscribeDuringEvent(sender, createdSub);
            sender.Subscribe(subcreator);

            SendEvent();
            sender.Clear();
            SendEvent();

            Assert.False(createdSub.triggered);
        }

        [Test]
        public void SendingEvent_FromSubscriber_ThrowsException()
        {
            SubscriberThatSendsEvent evilSub = new SubscriberThatSendsEvent(sender);

            sender.Subscribe(evilSub);

            Assert.Throws<System.InvalidOperationException>(
                () => SendEvent()
                );
        }
    }
}