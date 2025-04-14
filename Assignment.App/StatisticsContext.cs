namespace Assignment.LoaderConsole
{
    public class StatisticsContext
    {
        private volatile int dbCallsCount;

        internal void RegisterDbCall()
        {
            Interlocked.Increment(ref dbCallsCount);
        }

        public bool IsFromDatabase => dbCallsCount > 0;
    }
}
