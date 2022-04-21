namespace Tests
{
    class SomeSubscriber : ISomeSubscriber
    {
        public int triggerCount = 0;
        public bool triggered = false;
        public void OnTrigger()
        {
            triggered = true;
            triggerCount++;
        }
    }
}