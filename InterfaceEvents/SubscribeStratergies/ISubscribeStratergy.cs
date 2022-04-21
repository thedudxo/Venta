namespace DudCo.Events
{
    internal interface ISubscribeStratergy<T>
    {
        void Subscribe(T subscriber, int priority);
    }
}