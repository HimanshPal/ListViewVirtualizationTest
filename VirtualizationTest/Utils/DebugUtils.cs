using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;

namespace VirtualizationTest.Utils
{
    public sealed class DebugUtils
    {
        public static void TrackAppMemoryUsage()
        {
            if (Debugger.IsAttached)
            {
                var dp = new DispatcherTimer();
                dp.Interval = TimeSpan.FromSeconds(1);
                dp.Tick += (s, ex) =>
                {
                    ulong AppMemoryUsageUlong = MemoryManager.AppMemoryUsage;
                    AppMemoryUsageUlong /= 1024 * 1024;
                    Debug.WriteLine("Memory log: " + AppMemoryUsageUlong.ToString());
                };
                dp.Start();
            }
        }
    }
}
