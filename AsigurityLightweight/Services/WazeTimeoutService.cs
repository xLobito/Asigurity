using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using System.Timers;

namespace AsigurityLightweight.Services
{
    [Service(Enabled = true, Name = "com.asigurity.timeout")]
    public class WazeTimeoutService : Service
    {
        Timer WazeTimer = null;
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            EnableWazeTimeout();
            return StartCommandResult.NotSticky;
        }

        private void EnableWazeTimeout()
        {
            WazeTimer = new Timer(15);
            WazeTimer.Elapsed += WazeTimer_Elapsed;
            WazeTimer.Interval = 15000;
            WazeTimer.Enabled = true;
        }

        private void WazeTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ((Timer)sender).Stop();
            Intent RestoreAsigurityIntent = new Intent(this, typeof(MainActivity));
            RestoreAsigurityIntent.AddFlags(ActivityFlags.NewTask);
            StartActivity(RestoreAsigurityIntent);
            this.StopSelf();
        }
    }
}