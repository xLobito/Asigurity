using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using AsigurityLightweight.Interfaces;
using AsigurityLightweight.Utilities;

namespace AsigurityLightweight.Implementations
{
    public class OpenApplication : TextToSpeech, IOpenApplication
    {
        List<ApplicationInfo> ApplicationsList = null;
        public void OpenApplications(string ApplicationName)
        {
            Intent OpenApplicationIntent = null;

            if (ApplicationsList == null)
                RetrieveInstalledApplications();
            try
            {
                /* Aplicaciones estándar */
                switch (ApplicationName)
                {
                    case "Spotify":
                        OpenApplicationOnIntent("com.spotify.music");
                        break;
                    case "Facebook":
                        OpenApplicationOnIntent("com.facebook.lite", "com.facebook.katana");
                        break;
                    case "WhatsApp":
                        OpenApplicationOnIntent("com.whatsapp");
                        break;
                    case "Telegram":
                        OpenApplicationOnIntent("org.telegram.messenger");
                        break;
                    case "Instagram":
                        OpenApplicationOnIntent("com.instagram.android");
                        break;
                    default:
                        OpenSpecificApplicationOnIntent(OpenApplicationIntent, ApplicationName);
                        break;
                }
            }
            catch(Exception)
            {
                Speak("No se ha podido abrir ninguna aplicación. Por favor, reinténtelo");
            }
        }
        private void OpenApplicationOnIntent(string PackageName)
        {
            Intent OpenApplicationIntent = Application.Context.PackageManager.GetLaunchIntentForPackage(PackageName);

            if(OpenApplicationIntent == null)
            {
                FrontPageActivity.Instance.RunOnUiThread(() => Speak("Abriendo Play Store"));
                OpenApplicationIntent = new Intent(Intent.ActionView);
                OpenApplicationIntent.AddFlags(ActivityFlags.NewTask);
                OpenApplicationIntent.AddFlags(ActivityFlags.ClearTask);
                OpenApplicationIntent.SetData(Android.Net.Uri.Parse(GooglePlayStoreUri.BuildGooglePlayStoreRedirect(PackageName)));
                Application.Context.StartActivity(OpenApplicationIntent);
            }
            else
            {
                OpenApplicationIntent.AddFlags(ActivityFlags.NewTask);
                OpenApplicationIntent.AddFlags(ActivityFlags.ClearTask);
                Application.Context.StartActivity(OpenApplicationIntent);
            }
        }

        private void OpenApplicationOnIntent(string PackageNamePrimary, string PackageNameSecondary)
        {
            Intent OpenApplicationIntent = Application.Context.PackageManager.GetLaunchIntentForPackage(PackageNamePrimary);

            if(OpenApplicationIntent == null)
            {
                OpenApplicationIntent = Application.Context.PackageManager.GetLaunchIntentForPackage(PackageNameSecondary);
                if(OpenApplicationIntent == null)
                {
                    FrontPageActivity.Instance.RunOnUiThread(() => Speak("Abriendo Play Store"));
                    OpenApplicationIntent = new Intent(Intent.ActionView);
                    OpenApplicationIntent.AddFlags(ActivityFlags.NewTask);
                    OpenApplicationIntent.SetData(Android.Net.Uri.Parse(GooglePlayStoreUri.BuildGooglePlayStoreRedirect(PackageNameSecondary)));
                    Application.Context.StartActivity(OpenApplicationIntent);
                }
                else
                {
                    OpenApplicationIntent.SetAction(Intent.ActionMain);
                    OpenApplicationIntent.AddFlags(ActivityFlags.NewTask);
                    Application.Context.StartActivity(OpenApplicationIntent);
                }
            }
            else
            {
                OpenApplicationIntent.SetAction(Intent.ActionMain);
                OpenApplicationIntent.AddFlags(ActivityFlags.NewTask);
                Application.Context.StartActivity(OpenApplicationIntent);
            }
        }

        private void OpenSpecificApplicationOnIntent(Intent OpenApplicationIntent, string ApplicationName)
        {
            try
            {
                foreach (ApplicationInfo AppList in ApplicationsList)
                {
                    if (string.Equals(AppList.LoadLabel(Application.Context.PackageManager), ApplicationName, StringComparison.OrdinalIgnoreCase))
                    {
                        OpenApplicationIntent = Application.Context.PackageManager.GetLaunchIntentForPackage(AppList.PackageName);
                        OpenApplicationIntent.AddFlags(ActivityFlags.NewTask);
                        OpenApplicationIntent.AddFlags(ActivityFlags.ClearTask);
                        Application.Context.StartActivity(OpenApplicationIntent);
                    }
                }
                if (OpenApplicationIntent == null)
                {
                    FrontPageActivity.Instance.RunOnUiThread(() => Speak("Abriendo Play Store"));
                    OpenApplicationIntent = new Intent(Intent.ActionView);
                    OpenApplicationIntent.AddFlags(ActivityFlags.NewTask);
                    OpenApplicationIntent.AddFlags(ActivityFlags.ClearTask);
                    if (ApplicationName.Contains(" "))
                        ApplicationName = ApplicationName.Replace(" ", "%20");
                    OpenApplicationIntent.SetData(Android.Net.Uri.Parse(GooglePlayStoreUri.BuildGooglePlayStoreSearchRedirect(char.ToUpper(ApplicationName[0]) + ApplicationName.Substring(1))));
                    Application.Context.StartActivity(OpenApplicationIntent);
                }
            }
            catch(Exception)
            {
                Speak("Ha ocurrido un error al abrir la aplicación");
            }
        }

        private void RetrieveInstalledApplications()
        {
            PackageInfoFlags PkgInfoFlags = PackageInfoFlags.MetaData | PackageInfoFlags.SharedLibraryFiles | PackageInfoFlags.UninstalledPackages;
            List<ApplicationInfo> AllApplications = Application.Context.PackageManager.GetInstalledApplications(PkgInfoFlags).ToList();

            ApplicationsList = AllApplications.Where(AppInfo => IsSystemApplication(AppInfo.Flags, ApplicationInfoFlags.System) != 1).ToList();
        }

        private int IsSystemApplication(ApplicationInfoFlags Flags, ApplicationInfoFlags System)
        {
            int value1 = (int)Flags;
            int value2 = (int)System;

            return value1 & value2;
        }
    }
}