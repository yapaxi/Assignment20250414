using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
