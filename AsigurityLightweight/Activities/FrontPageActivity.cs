using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using AsigurityLightweight.Activities;
using AsigurityLightweight.Exceptions;
using AsigurityLightweight.Implementations;
using AsigurityLightweight.Services;
using AsigurityLightweight.Utilities;
using FloatingSearchViews;
using Java.Net;
using Newtonsoft.Json;
using Xamarin.Essentials;
using TextToSpeech = AsigurityLightweight.Implementations.TextToSpeech;

namespace AsigurityLightweight
{
    [Activity(Label = "FrontPageActivity", ConfigurationChanges = ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class FrontPageActivity : Activity
    {
        KeyguardManager Manager;
        TextToSpeech Speech = new TextToSpeech();
        readonly Speech SpeechRecognizer = new Speech();
        const int ConfirmRequestId = 1;
        const int MicrophoneRequestCode = 10;
        const int ContactDialRequestCode = 11;
        private FloatingSearchView FloatingSearchView { get; set; }
        private FloatingActionButton Fab { get; set; }
        private ImageButton Imb { get; set; }
        internal static FrontPageActivity Instance { get; private set; }
        internal static bool IsCarModeActivated { get; set; } = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Manager = (KeyguardManager)GetSystemService(KeyguardService);
            Instance = this;
            SetContentView(Resource.Layout.FrontPage);
            /* Añadir contexto para búsqueda de aplicaciones */
            if(!IsCarModeActivated)
            {
                ShowAuthenticationScreen();
            }
            FloatingSearchView = FindViewById<FloatingSearchView>(Resource.Id.floating_search_view);
            Fab = FindViewById<FloatingActionButton>(Resource.Id.floatingActionButton1);
            Imb = FindViewById<ImageButton>(Resource.Id.imageButton1);
            Fab.Click += FabOnClick;
            Imb.Click += Imb_Click;
            Imb.Enabled = false;
            /** 
             * Esta sección debe añadirse luego. FloatingSearchView.ContextClick += FloatingSearchView_ContextClick;
             */
            FloatingSearchView.SearchAction += FloatingSearchView_SearchAction;
        }

        /* Android Lifecycle phase 2 */

        private async void FabOnClick(object sender, EventArgs e)
        {
            await ProcessCarModeOrInteractive();
        }

        private async Task ProcessCarModeOrInteractive()
        {
            string SpeechResult = string.Empty;

            if (CheckSelfPermission(Manifest.Permission.RecordAudio) == Permission.Granted)
            {
                Toast.MakeText(this, "El micrófono está activo", ToastLength.Short).Show();
            }
            else
            {
                RequestPermissions(new string[] { Manifest.Permission.RecordAudio }, MicrophoneRequestCode);
            }
            Speech = new TextToSpeech();
            Speech.Speak("¿Qué desea hacer?");
            await Task.Delay(1900);
            try
            {
                SpeechResult = await SpeechRecognizer.SpeechToText();
                if (char.IsLower(SpeechResult[0]))
                    SpeechResult = char.ToUpperInvariant(SpeechResult[0]) + SpeechResult.Substring(1);
                if (!SpeechResult.Contains(" "))
                    await RecognizeCommandInteractive(SpeechResult, Speech, SpeechRecognizer);
                else if (SpeechResult.Equals("Activar modo auto", StringComparison.OrdinalIgnoreCase))
                    SwitchCarMode();
                else if (SpeechResult.Length == 0)
                    throw new CommandNotRecognizedException("No se ha podido realizar ninguna acción");
                else
                {
                    /**
                     * await RecognizeCommand(SpeechResult, null, null);
                     */
                    await RecognizeCommand();
                }
            }
            catch (Exception)
            {
                Speech.Speak("No se ha podido realizar ninguna acción");
            }
        }

        private void SwitchCarMode()
        {
            AlertDialog.Builder CarMode = null;

            RunOnUiThread(() =>
            {
                CarMode = new AlertDialog.Builder(this);
                CarMode.SetTitle("Modo Conducción");
                CarMode.SetPositiveButton("Sí", (sender, args) =>
                {
                    Toast.MakeText(this, "Modo conducción activo", ToastLength.Short).Show();
                    IsCarModeActivated = true;
                    Imb.Enabled = true;
                });
                CarMode.SetNegativeButton("No", (sender, args) =>
                {
                    Toast.MakeText(this, "Modo conducción inactivo", ToastLength.Short).Show();
                }).Show();
            });
        }

        private async void Imb_Click(object sender, EventArgs e)
        {
            await ProcessCommand();   
        }

        private async Task ProcessCommand()
        {
            if (CheckSelfPermission(Manifest.Permission.RecordAudio) == Permission.Granted)
            {
                Toast.MakeText(this, "El micrófono está activo", ToastLength.Short).Show();
            }
            else
            {
                RequestPermissions(new string[] { Manifest.Permission.RecordAudio }, MicrophoneRequestCode);
            }
            try
            {
                /** 
                 * await RecognizeCommand(SpeechResult, Speech, SpeechRecognizer);
                 */
                await RecognizeCommand();
            }
            catch (Exception)
            {
                Speech.Speak("No se ha podido realizar ninguna acción");
            }
        }

        private async void FloatingSearchView_SearchAction(object sender, FloatingSearchView.SearchActionEventArgs e)
        {
            string SearchForQuery = e.CurrentQuery;
            string DefaultGoogleSearch = "http://www.google.com/#q=";
            Android.Net.Uri UriRequested = null;
            Intent SearchIntent = null;

            await Task.Run(() =>
            {
                if (!string.IsNullOrEmpty(SearchForQuery))
                {
                    try
                    {
                        SearchForQuery = URLEncoder.Encode(SearchForQuery, "UTF-8");
                        UriRequested = Android.Net.Uri.Parse(DefaultGoogleSearch + SearchForQuery);
                        SearchIntent = new Intent(Intent.ActionView, UriRequested);
                        SearchIntent.SetPackage("com.android.chrome");
                        StartActivity(SearchIntent);
                        ((FloatingSearchView)sender).SetSearchText("");
                    }
                    catch (Exception ex)
                    {
                        Log.Debug("Asigurity", "Ha ocurrido un error: " + ex.Message);
                    }
                }
            });
        }

        private async Task RecognizeCommandInteractive(string SpeechResult, TextToSpeech Speech, Speech SpeechRecognizer)
        {
            Dialer CallContact = null;
            OpenApplication RequestOpenApp = null;
            string NewSpeechResult = string.Empty;

            switch(SpeechResult)
            {
                case "Llamar":
                    {
                        Speech.Speak("¿A quién desea llamar?");
                        await Task.Delay(2500);
                        NewSpeechResult = await SpeechRecognizer.SpeechToText();
                        if (CheckSelfPermission(Manifest.Permission.ReadContacts) == Permission.Granted)
                        {
                            Toast.MakeText(this, "Leer contactos están activos", ToastLength.Short).Show();
                        }
                        else
                        {
                            RequestPermissions(new string[] { Manifest.Permission.ReadContacts, Manifest.Permission.CallPhone }, ContactDialRequestCode); /* Lock */
                        }
                        CallContact = new Dialer();
                        NewSpeechResult = char.ToUpperInvariant(NewSpeechResult[0]) + NewSpeechResult.Substring(1);
                        CallContact.CallFromContactName(NewSpeechResult);
                    }
                    break;
                case "Abrir":
                    {
                        Speech.Speak("¿Qué aplicación desea abrir?");
                        await Task.Delay(2700);
                        NewSpeechResult = await SpeechRecognizer.SpeechToText();
                        RequestOpenApp = new OpenApplication();
                        RequestOpenApp.OpenApplications(NewSpeechResult);
                    }
                    break;
            }
        }

        private async Task RecognizeCommand()
        {
            Dialer CallContact = null;
            OpenApplication RequestOpenApp = null;
            string Cmd = string.Empty;
            string Phrase = string.Empty;
            string[] CmdArray;

            try
            {
                Cmd = await SpeechRecognizer.SpeechToText();
                if (char.IsLower(Cmd[0]))
                    Cmd = char.ToUpperInvariant(Cmd[0]) + Cmd.Substring(1);
                CmdArray = Cmd.Split(' ');
                if (CmdArray.Length > 2)
                {
                    for (int i = 1; i < CmdArray.Length; i++)
                        Phrase = (i == 1) ? new StringBuilder(CmdArray[1]).ToString() : new StringBuilder(" " + CmdArray[i]).ToString();
                        /** 
                         * Phrase += (i == 1) ? CmdArray[1] : (" " + CmdArray[i]);
                         */
                }
                else
                    Phrase += CmdArray[0];
                if (char.IsLower(CmdArray[0].First()))
                    CmdArray[0] = char.ToUpperInvariant(CmdArray[0].First()) + CmdArray[0].Substring(1);
                switch(CmdArray[0])
                {
                    case "Abrir":
                        {
                            try
                            {
                                RequestOpenApp = new OpenApplication();
                                RequestOpenApp.OpenApplications(CmdArray[1]);
                            }
                            catch (Exception ex)
                            {
                                Log.Debug("Asigurity", ex.Message);
                                Speech.Speak("No se ha podido abrir la aplicación solicitada");
                            }
                        }
                        break;
                    case "Llamar":
                        {
                            try
                            {
                                CallContact = new Dialer();
                                CallContact.CallFromContactName(Phrase);
                            }
                            catch (Exception ex)
                            {
                                Log.Debug("Asigurity", ex.Message);
                                Speech.Speak("No se ha podido llamar al contacto solicitado");
                            }
                        }
                        break;
                    case "Obtener":
                        {
                            if(string.Equals(CmdArray[1], "Clima", StringComparison.OrdinalIgnoreCase))
                            {
                                string GeocodeAddress = string.Empty;
                                Weather Weather = null;
                                WeatherStats ws = null;
                                Intent WeatherActivityIntent = null;
                                GeolocationRequest Accuracy = null;
                                Location RetrievedLocation = null;
                                Placemark Position = null;
                                IEnumerable<Placemark> Positions = null;
                            
                                try
                                {
                                    Accuracy = new GeolocationRequest(GeolocationAccuracy.Default);
                                    RetrievedLocation = await Geolocation.GetLocationAsync(Accuracy);
                                    if(RetrievedLocation != null)
                                    {
                                        Positions = await Geocoding.GetPlacemarksAsync(RetrievedLocation.Latitude, RetrievedLocation.Longitude);
                                        Position = Positions?.FirstOrDefault();
                                        if(Position != null)
                                            GeocodeAddress = Position.Locality;
                                    }
                                }
                                catch(Exception ex)
                                {
                                    Log.Debug("Asigurity", ex.Message);
                                }
                                Weather = new Weather();
                                ws = await Weather.GetWeatherAndForecastAsync(GeocodeAddress);
                                WeatherActivityIntent = new Intent(this, typeof(WeatherActivity));
                                WeatherActivityIntent.PutExtra("WeatherStatsStr", JsonConvert.SerializeObject(ws));
                                StartActivity(WeatherActivityIntent);
                            }
                        }
                        break;
                    case "Ir":
                        {
                            string Place = string.Empty;
                            string LatitudeEnd = string.Empty;
                            string LongitudeEnd = string.Empty;
                            const string WazeDefaultAddress = "waze://?ll=";
                            GeolocationRequest Accuracy = null;
                            Location RetrievedLocation = null;
                            Placemark Position = null;
                            Intent StartNavigationIntent = null;
                            IEnumerable<Location> Locations = null;
                            IEnumerable<Placemark> Positions = null;

                            Speech.Speak("¿A qué lugar se dirige?");
                            await Task.Delay(4000);
                            Place = await SpeechRecognizer.SpeechToText();
                            try
                            {
                                /* Switch case */
                                if(string.Equals(Place, "Portal", StringComparison.OrdinalIgnoreCase))
                                {
                                    StartNavigationIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(WazeUri.BuildFavoriteWazeUri("Portal")));
                                    StartNavigationIntent.SetPackage("com.waze");
                                    StartService(new Intent(this, typeof(WazeTimeoutService)));
                                    StartActivity(StartNavigationIntent);
                                }
                                else if(string.Equals("Automovil", Normalization.RemoveAccents(Place), StringComparison.OrdinalIgnoreCase))
                                {
                                    StartNavigationIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(WazeUri.BuildFavoriteWazeUri("Automovil")));
                                    StartNavigationIntent.SetPackage("com.waze");
                                    StartService(new Intent(this, typeof(WazeTimeoutService)));
                                    StartActivity(StartNavigationIntent);
                                }
                                else if(string.Equals(Place, "Casa", StringComparison.OrdinalIgnoreCase))
                                {
                                    StartNavigationIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(WazeUri.BuildFavoriteWazeUri("Casa")));
                                    StartNavigationIntent.SetPackage("com.waze");
                                    StartService(new Intent(this, typeof(WazeTimeoutService)));
                                    StartActivity(StartNavigationIntent);
                                }
                                else
                                {
                                    AlertDialog.Builder ADBuilder = new AlertDialog.Builder(this);
                                    Locations = await Geocoding.GetLocationsAsync(Place);
                                    List<Location> LocationsList = Locations.ToList();
                                    if(Locations.Count() != 0)
                                    {
                                        Accuracy = new GeolocationRequest(GeolocationAccuracy.Best);
                                        RetrievedLocation = await Geolocation.GetLocationAsync(Accuracy);
                                        if(RetrievedLocation != null)
                                        {
                                            Positions = await Geocoding.GetPlacemarksAsync(RetrievedLocation.Latitude, RetrievedLocation.Longitude);
#pragma warning disable S1854 // Dead stores should be removed
                                            Position = Positions?.FirstOrDefault();
#pragma warning restore S1854 // Dead stores should be removed
                                        }
                                        string[] ArrayDistances = new string[Locations.Count()];
                                        for (int i = 0; i < Locations.Count(); i++)
                                        {
                                            ArrayDistances[i] = Location.CalculateDistance(RetrievedLocation.Latitude, RetrievedLocation.Longitude, LocationsList[i], DistanceUnits.Kilometers).ToString("0.##") + " Km";
                                        }
                                        ADBuilder.SetTitle("Distancias Resultantes");
                                        ADBuilder.SetItems(ArrayDistances, (sender, args) =>
                                        {
                                            /* Falta completar */
                                            LatitudeEnd = LocationsList[args.Which].Latitude.ToString().Replace(",", ".");
                                            LongitudeEnd = LocationsList[args.Which].Longitude.ToString().Replace(",", ".");
                                            StartNavigationIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(WazeDefaultAddress + LatitudeEnd + "," + LongitudeEnd + "&navigate=yes"));
                                            StartNavigationIntent.SetPackage("com.waze");
                                            StartService(new Intent(this, typeof(WazeTimeoutService)));
                                            StartActivity(StartNavigationIntent);
                                        }).Show();
                                    }
                                }
                            }
                            catch(Exception ex)
                            {
                                Log.Debug("Asigurity", ex.Message);
                            }
                        }
                        break;
                    case "Enviar":
                        {
                            /* Si no existe el contacto (String pattern matching. Usar Levenshtein Distance para evaluar similtud < 0.75 */
                            string ContactName = string.Empty;
                            string ContactNumber = string.Empty;
                            string MessageText = string.Empty;
                            Intent SendMessageIntent = null;

                            Speech.Speak("Nombre del contacto");
                            await Task.Delay(3000);
                            ContactName = await SpeechRecognizer.SpeechToText();
                            try
                            {
                                CallContact = new Dialer();
                                ContactNumber = CallContact.GetPhoneNumberFromContactName(ContactName);
                                SendMessageIntent = new Intent(Intent.ActionSend);
                                if(SendMessageIntent != null)
                                {
                                    SendMessageIntent.SetType("text/plain");
                                    SendMessageIntent.SetPackage("com.whatsapp");
                                    Speech.Speak("Deje su mensaje");
                                    await Task.Delay(3000);
                                    MessageText = await SpeechRecognizer.SpeechToText();
                                    SendMessageIntent.PutExtra(Intent.ExtraText, MessageText);
                                    SendMessageIntent.PutExtra("jid", ContactNumber + "@s.whatsapp.net");
                                    try
                                    {
                                        StartActivity(SendMessageIntent);
                                    }
                                    catch(Exception ex)
                                    {
                                        Log.Debug("Asigurity", ex.Message);
                                    }
                                }
                            }
                            catch(Exception ex)
                            {
                                Log.Debug("Asigurity", ex.Message);
                            }
                        }
                        break;
                }
            }
            catch(Exception ex)
            {
                Log.Debug("Asigurity", ex.Message);
                Speech.Speak("Ha ocurrido un error inesperado");
            } 
        }

        #region Unused
        /**
        private async void Fab2_Click(object sender, EventArgs e)
        {
            Speech SpeechRecognizer = new Speech();
            TextToSpeech Speech = new TextToSpeech();
            Android.Net.Uri UriRequested = null;
            Intent SearchIntent = null;
            string SpeechResult = string.Empty;
            string SearchQuery = string.Empty;

            Speech.Speak("¿Qué desea buscar?");
            await Task.Delay(100);
            SpeechResult = await SpeechRecognizer.SpeechToText();
            SearchQuery = URLEncoder.Encode(SpeechResult, "UTF-8");
            UriRequested = Android.Net.Uri.Parse("http://www.google.com/#q=" + SearchQuery);
            SearchIntent = new Intent(Intent.ActionView, UriRequested);
            StartActivity(SearchIntent);
        } */
        #endregion

        private void ShowAuthenticationScreen()
        {
            var Intent = Manager.CreateConfirmDeviceCredentialIntent("Para acceder. Confirme su identificación", null);

            if (Intent != null)
                StartActivityForResult(Intent, ConfirmRequestId);
        }

        public override void OnBackPressed()
        {
            Intent Intent = new Intent();
            Intent.SetAction(Intent.ActionMain);
            Intent.AddCategory(Intent.CategoryHome);
            StartActivity(Intent);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if(requestCode == ConfirmRequestId && resultCode == Result.Ok)
            {
                Toast.MakeText(this, "Funciona correctamente el PIN", ToastLength.Long);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            if((requestCode == MicrophoneRequestCode) && grantResults.Length == 1 && grantResults[0] == Permission.Granted)
            {
                Toast.MakeText(this, "Permisos de micrófono están activos", ToastLength.Short).Show();
            }
            else if((requestCode == ContactDialRequestCode) && (grantResults.Length == 1 && grantResults[0] == Permission.Granted))
            {
                Toast.MakeText(this, "Permisos para leer contactos y llamadas están activos", ToastLength.Short).Show();
            }
        }
    }
}