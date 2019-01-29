using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Android.Content.PM;
using AsigurityLightweight.Implementations;

namespace AsigurityLightweight
{
    [Activity(Label = "@string/app_name", Icon = "@drawable/logo", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true, ConfigurationChanges = ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {
        #region ProgressBar
        Startup InitializeEnvironment;
        #endregion
        #region PINAuthentication
        private KeyguardManager Manager;
        #endregion
        #region MainActivityInstanceRegion
        internal static MainActivity Instance { get; private set; }
        #endregion

        #region ActivityOverridenMethods
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            Instance = this;
            Manager = (KeyguardManager)GetSystemService(KeyguardService);
            InitializeEnvironment = new Startup();
            InitializeEnvironment.Loading(FindViewById<ProgressBar>(Resource.Id.progressBar1));
        }
        #endregion

        #region No utilizado
        /*
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View) sender;
            Snackbar.Make(view, "Micrófono activo", Snackbar.LengthLong).SetAction("Action", (View.IOnClickListener)null).Show();
        } */
        #endregion
    }
}

