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
        public EventSender<IOnRunB> onRunB = new eventSender<IOnRunB>();

        public B(A a)
        {
        }

        public void Run(){
            onRunB.Send(s => s.onRunB());
        }
    }

    interface IOnRunB { void onRunB();}
    interface IOnCreateA { void OnCreateA(A a); }
    interface IOnCreateB { void OnCreateB(B b); }

    class AFactory
    {
        ChainEventOwner events;

        public AFactory(ChainEventOwner events)
        {
            this.events = events;
        }
        public A Create()
        {
            A a = new A();
            events.OnCreateA.Send(s => s.OnCreateA(a));
            return a;
        }
    }

    class BFactory
    {
        ChainEventOwner events;

        public BFactory(ChainEventOwner events)
        {
            this.events = events;
        }

        public B Create(A a)
        {
            B b = new(a);
            events.OnCreateB.Send(s => s.OnCreateB(b));
            return b;
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

            A a = aFactory.Create();
            events.bFactory.Create(a);
        }


    }
    //Venter
    class ChainEventConsumer : IOnCreateB, IOnRunB
    {
        ChainEventOwner events = new();

        ChainEventConsumer()
        {
            events.OnCreateB.Subscribe(this);
        }

        public void OnCreateB(B b)
        {
            b.onRunB.Subscribe(this);
        }

        public void OnRunB()
        {
            //do stuff with b
        }
    }
}
