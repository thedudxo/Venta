using NUnit.Framework;
using DudCo.Events;

namespace Tests
{
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public partial class EventSenderTests
    {
        EventSender<ISomeSubscriber> _event;
        SomeSubscriber subscriber;

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

        public EventSenderTests()
        {
            _event = new EventSender<ISomeSubscriber>();
            subscriber = new SomeSubscriber();

            _event.Subscribe(subscriber);
        }

        void SendEvent() => _event.Send((ISomeSubscriber sub) => sub.OnTrigger());

        [Test]
        public void SubscriberGetsNotified()
        {
            SendEvent();

            Assert.True(subscriber.triggered);
        }

        [Test]
        public void Unsubscribe_Doesnt_GetNotified()
        {
            _event.Unsubscribe(subscriber);

            SendEvent();

            Assert.False(subscriber.triggered);
        }

        [Test]
        public void AllreadySubscribed_Throws_InvaildOpException() 
        {
            Assert.Throws<System.InvalidOperationException>(
                () => _event.Subscribe(subscriber));
        }

        [Test]
        public void UnsubscribingNotSubscribed_Throws_InvaildOpException() 
        {
            _event.Unsubscribe(subscriber);

            Assert.Throws<System.InvalidOperationException>(
                () => _event.Unsubscribe(subscriber));
        }

        [Test]
        public void Unsubscribe_DuringEvent()
        {
            UnSubscribeDuringEvent sub = new UnSubscribeDuringEvent(_event);
            _event.Subscribe(sub);

            SendEvent();
            sub.triggered = false;
            SendEvent();

            Assert.False(sub.triggered);
        }

        [Test]
        public void SubscribeDuringEvent_TriggersNewSub()
        {
            SomeSubscriber createdSub = new SomeSubscriber();
            SubscribeDuringEvent subCreator = new SubscribeDuringEvent(_event, createdSub);
            _event.Subscribe(subCreator);

            SendEvent();
            SendEvent();

            Assert.True(createdSub.triggered);
        }

        [Test]
        public void SubscribeDuringEvent_DoesntTriggerEarly()
        {
            SomeSubscriber createdSub = new SomeSubscriber();
            SubscribeDuringEvent subCreator = new SubscribeDuringEvent(_event, createdSub);
            _event.Subscribe(subCreator);

            SendEvent();

            Assert.False(createdSub.triggered);
        }

        [Test]
        public void Clear_SubscriberIsntTriggered()
        {
            _event.Clear();
            SendEvent();

            Assert.False(subscriber.triggered);
        }

        [Test]
        public void Clear_NewSubscriberDuringEvent_IsntTriggered()
        {
            SomeSubscriber createdSub = new SomeSubscriber();
            SubscribeDuringEvent subcreator = new SubscribeDuringEvent(_event, createdSub);
            _event.Subscribe(subcreator);

            SendEvent();
            _event.Clear();
            SendEvent();

            Assert.False(createdSub.triggered);
        }

        [Test]
        public void SendingEvent_FromSubscriber_ThrowsException()
        {
            SubscriberThatSendsEvent evilSub = new SubscriberThatSendsEvent(_event);

            _event.Subscribe(evilSub);

            Assert.Throws<System.InvalidOperationException>(
                () => SendEvent()
                );
        }
    }
}