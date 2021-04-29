using System;
using System.Collections.Generic;
using System.Text;

namespace ClientTaskManager
{
    static class Configuration
    {
       static public int MaxRequestPerInterval = 5;
       static public int MaxIntervalsPerMinute = 50;
       static public int MaxTasksPerRequest = 100000;

    }
}
