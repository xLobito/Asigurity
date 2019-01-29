using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AsigurityLightweight.Interfaces;

namespace AsigurityLightweight.Implementations
{
    public class Startup : IStartup
    {
        private Timer progressTimer;
        private int countSeconds;
        private ProgressBar _progressBar;
        private object lockThread = new object();

        public void Loading(ProgressBar progressBar)
        {
            progressBar.Max = 100;
            progressBar.Progress = 0;
            _progressBar = progressBar;
            progressTimer = new Timer();
            countSeconds = 100;
            progressTimer.Enabled = true;
            progressTimer.Interval = 1000;
            progressTimer.Elapsed += OnTimedEvent;
        }

        public void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            countSeconds -= 20;

            MainActivity.Instance.RunOnUiThread(() =>
            {
                _progressBar.IncrementProgressBy(20);
                if (countSeconds == 100 || countSeconds == 80)
                {
                    _progressBar.IndeterminateTintList = ColorStateList.ValueOf(Color.Black);
                }
                else if (countSeconds == 60 || countSeconds == 40)
                {
                    _progressBar.IndeterminateTintList = ColorStateList.ValueOf(Color.DarkGreen);
                }
                else
                {
                    _progressBar.IndeterminateTintList = ColorStateList.ValueOf(Color.DarkViolet);
                }
                CheckProgress(100 - countSeconds);
            });
        }
        public void CheckProgress(int progress)
        {
            lock (lockThread)
            {
                if (progress >= 100)
                {
                    progressTimer.Dispose();
                    MainActivity.Instance.StartActivity(new Intent(Application.Context, typeof(FrontPageActivity)));
                }
            }
        }
    }
}