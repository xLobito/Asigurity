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
    public class WazeUri
    {
#pragma warning disable S1075 // URIs should not be hardcoded
        private static readonly string WazeDefaultUri = "https://waze.com/";
#pragma warning restore S1075 // URIs should not be hardcoded
        private static readonly string WazeDefaultFavorite = "ul?favorite=";
        private static readonly string WazeDefaultNavigate = "&navigate=yes";

        public static string BuildFavoriteWazeUri(string FavoritePlace)
        {
            return new StringBuilder(WazeDefaultUri + WazeDefaultFavorite + FavoritePlace + WazeDefaultNavigate).ToString();
        }

        protected WazeUri() { }
    }
}