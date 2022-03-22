namespace Tests
{
    class OrderedSubscriber : ISomeSubscriber
    {
        string _name;

        public OrderedSubscriber(string name = "")
        {
            _name = name;
        }

        static int triggerCount = 0;
        public int triggeredAt = 0;
        public bool triggered = false;

        public void OnTrigger()
        {
            triggered = true;
            triggeredAt = triggerCount;
            triggerCount++;
        }

        public static void Clear() => triggerCount = 0;
    }
}