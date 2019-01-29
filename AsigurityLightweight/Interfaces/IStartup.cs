using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace AsigurityLightweight.Interfaces
{
    public interface IStartup
    {
        void Loading(ProgressBar progressBar);
        void OnTimedEvent(object sender, ElapsedEventArgs e);
        void CheckProgress(int progress);
    }
}