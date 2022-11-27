using DudCo.Events;
using NUnit.Framework;
using System;

namespace Tests.EventSenders;

class Exception_during_event
{
    class TestException : Exception { }

    [Test]
    public void Resending_an_event_after_a_subscriber_threw_an_exception_does_not_cause_eventSender_to_throw_a_concurrent_send_exception() 
    {
        bool doThrow = true;
        EventSender<Action> eventSender = new();
        eventSender.Subscribe(() =>
        {
            if(doThrow)
                throw new TestException();
        });

        void Act()
        {
            try
            {
                eventSender.Send(s => s());
            }
            catch(TestException)
            {
                doThrow = false;
                eventSender.Send(s => s());
            }
        }

        Assert.DoesNotThrow(Act);
    }

    [Test]
    public void New_subscriber_to_an_event_within_a_caught_exception_block_receives_the_event_next_time_it_is_sent()
    {
        bool receivedEvent = false;
        bool doThrow = true;

        void Act()
        {
            try
            {
                causeTheProblem();
            }
            catch (TestException)
            {

            }

            void causeTheProblem()
            {
                EventSender<Action> eventSender = new();

                eventSender.Subscribe(() =>
                {
                    if (doThrow)
                        throw new TestException();
                });

                try
                {
                    eventSender.Send(s => s());
                }
                catch (TestException)
                {
                    doThrow = false;
                    eventSender.Subscribe(() => receivedEvent = true);
                    eventSender.Send(s => s());
                }
            }
        }

        Assume.That(Act, Throws.Nothing);
        Assert.That(receivedEvent, Is.True);
    }

    [Test]
    public void A_new_subscriber_during_event_that_later_throws_and_catches_an_exception_receives_the_event_the_next_time_it_is_sent()
    {
        bool receivedEvent = false;
        bool doSubscribeNew = true;
        bool doThrow = true;
        bool caughtException = false;

        void Act()
        {
            EventSender<Action> eventSender = new();

            eventSender.Subscribe(() =>
            {
                if (doSubscribeNew)
                    eventSender.Subscribe(() => receivedEvent = true);

                doSubscribeNew = false;

                if (doThrow)
                    throw new TestException();
            });

            try
            {
                eventSender.Send(s => s());
            }
            catch (TestException)
            {
                caughtException = true;
                doThrow = false;
                eventSender.Send(s => s());
            }
        }

        Assume.That(Act, Throws.Nothing, "All exceptions caught");
        Assume.That(caughtException, Is.True, "threw and caught testException");
        Assert.That(receivedEvent, Is.True, "new sub received event");
    }
}
