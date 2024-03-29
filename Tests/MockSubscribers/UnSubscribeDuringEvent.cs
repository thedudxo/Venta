﻿using DudCo.Events;

namespace Tests
{
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

    class UnSubscribeTwiceDuringEvent : ISomeSubscriber
    {
        EventSender<ISomeSubscriber> sender;

        public UnSubscribeTwiceDuringEvent(EventSender<ISomeSubscriber> sender)
        {
            this.sender = sender;
        }

        public bool triggered = false;

        public void OnTrigger()
        {
            triggered = true;
            sender.Unsubscribe(this);
            sender.Unsubscribe(this);
        }
    }
}