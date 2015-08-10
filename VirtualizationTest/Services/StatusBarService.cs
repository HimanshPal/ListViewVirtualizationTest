using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.ViewManagement;

namespace VirtualizationTest.Services
{
    public static class StatusBarService
    {
        public static StatusBar StatusBar => StatusBar.GetForCurrentView();

        public static IAsyncAction ShowProgressBar(string status = "updating list...")
        {
            StatusBar.BackgroundOpacity = 1;
            StatusBar.ProgressIndicator.Text = status;
            return StatusBar.ProgressIndicator.ShowAsync();
        }

        public static IAsyncAction HideProgressBar()
        {
            StatusBar.BackgroundOpacity = 0;
            return StatusBar.ProgressIndicator.HideAsync();
        }
    }
}
