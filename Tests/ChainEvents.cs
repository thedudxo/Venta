using DudCo.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    class A { }
    class B
    {
        public B(A a)
        {
        }
    }

    interface IOnCreateA { void OnCreateA(A a); }
    interface IOnCreateB { void OnCreateB(B b); }

    class AFactory
    {
        ChainEventOwner events;

        public AFactory(ChainEventOwner events)
        {
            this.events = events;
        }
        public void Create()
        {
            A a = new A();
            events.OnCreateA.Send(s => s.OnCreateA(a));
        }
    }

    class BFactory
    {
        ChainEventOwner events;

        public BFactory(ChainEventOwner events)
        {
            this.events = events;
        }

        public void Create(A a)
        {
            B b = new(a);
            events.OnCreateB.Send(s => s.OnCreateB(b));
        }
    }


    class ChainEventOwner
    {
        public EventSender<IOnCreateA> OnCreateA = new();
        public EventSender<IOnCreateB> OnCreateB = new();       
        public AFactory aFactory;
        public BFactory bFactory;

        public ChainEventOwner()
        {
            aFactory = new(this);
            bFactory = new(this);

            aFactory.Create();
        }


    }
    //Venter
    class ChainEventConsumer : IOnCreateA, IOnCreateB  
    {
        ChainEventOwner events = new();

        ChainEventConsumer()
        {
            events.OnCreateA.Subscribe(this);
            events.OnCreateB.Subscribe(this);
        }

        public void OnCreateA(A a)
        {
            events.bFactory.Create(a);
        }

        public void OnCreateB(B b)
        {
            //do stuff with b
        }
    }
}
