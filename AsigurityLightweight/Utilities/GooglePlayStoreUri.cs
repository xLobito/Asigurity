using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace AsigurityLightweight.Utilities
{
    public class GooglePlayStoreUri
    {
#pragma warning disable S1075 // URIs should not be hardcoded
        private static readonly string GooglePlayStoreDefaultUri = "market://details?id=";
#pragma warning restore S1075 // URIs should not be hardcoded
#pragma warning disable S1075 // URIs should not be hardcoded
        private static readonly string GooglePlayStoreDefaultSearchUri = "market://search?q=";
#pragma warning restore S1075 // URIs should not be hardcoded

        public static string BuildGooglePlayStoreRedirect(string PackageName)
        {
            return new StringBuilder(GooglePlayStoreDefaultUri + PackageName).ToString();
        }

        public static string BuildGooglePlayStoreSearchRedirect(string ApplicationName)
        {
            return new StringBuilder(GooglePlayStoreDefaultSearchUri + ApplicationName).ToString();
        }

        protected GooglePlayStoreUri() { }
    }
}