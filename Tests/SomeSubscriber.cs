namespace Tests
{
    class SomeSubscriber : ISomeSubscriber
    {
        public bool triggered = false;
        public void OnTrigger() => triggered = true;
    }
}