using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using AsigurityLightweight.Utilities;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace AsigurityLightweight.Activities
{
    [Activity(Label = "WeatherActivity")]
    public class WeatherActivity : AppCompatActivity
    {
        private string CityName;
        TextView SelectCity { get; set; }
        TextView CityField { get; set; }
        TextView DetailsField { get; set; }
        TextView CurrentTemperatureField { get; set; }
        TextView HumidityField { get; set; }
        TextView WindField { get; set; }
        ImageView WeatherIcon { get; set; }
        TextView UpdatedField { get; set; }
        WeatherStats WeatherStats { get; set; }

        public WeatherActivity()
        {
            CityName = string.Empty;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Timer updateTimer = new Timer(1000);
            Timer activityLimit = new Timer(4000);

            base.OnCreate(savedInstanceState);
            WeatherStats = JsonConvert.DeserializeObject<WeatherStats>(Intent.GetStringExtra("WeatherStatsStr"));
            CityName = WeatherStats.City.ToUpperInvariant() + ", CL";
            SupportActionBar.Hide();
            SetContentView(Resource.Layout.WeatherPage);
            SelectCity = FindViewById<TextView>(Resource.Id.selectCity);
            CityField = FindViewById<TextView>(Resource.Id.cityField);
            UpdatedField = FindViewById<TextView>(Resource.Id.updatedField);
            DetailsField = FindViewById<TextView>(Resource.Id.detailsField);
            CurrentTemperatureField = FindViewById<TextView>(Resource.Id.currentTemperatureField);
            HumidityField = FindViewById<TextView>(Resource.Id.humidityField);
            WindField = FindViewById<TextView>(Resource.Id.windField);
            WeatherIcon = FindViewById<ImageView>(Resource.Id.weatherIcon);
            SelectCity.Text = char.ToUpperInvariant(WeatherStats.City[0]) + WeatherStats.City.Substring(1);
            CityField.Text = CityName;
            updateTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            updateTimer.Interval = 1000;
            updateTimer.Enabled = true;
            activityLimit.Elapsed += ActivityLimit_Elapsed;
            activityLimit.Enabled = true;
            DetailsField.Text = WeatherStats.WeatherCondition[0].DescriptionForCondition.ToUpperInvariant();
            CurrentTemperatureField.Text = WeatherStats.Temperature[0];
            HumidityField.Text = WeatherStats.Humidity[0];
            WindField.Text = WeatherStats.WindSpeed[0];
            SetWeatherIcon(WeatherStats, WeatherIcon);
        }

        private void ActivityLimit_Elapsed(object sender, ElapsedEventArgs e)
        {
            Finish();
            ((Timer)sender).Stop();
        }

        private async void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            await Task.Run(() =>
            {
                RunOnUiThread(() =>
                {
                    UpdatedField.Text = DateTime.Now.ToString();
                });
            });
        }

        private static void SetWeatherIcon(WeatherStats WeatherStats, ImageView WeatherIcon)
        {
            switch (WeatherStats.WeatherCondition[0].Icon)
            {
                case "01d":
                case "01n":
                    WeatherIcon.SetImageResource(Resource.Drawable.clearsky);
                    break;
                case "02d":
                case "02n":
                    WeatherIcon.SetImageResource(Resource.Drawable.fewclouds);
                    break;
                case "03d":
                case "03n":
                    WeatherIcon.SetImageResource(Resource.Drawable.scatteredclouds);
                    break;
                case "04d":
                case "04n":
                    WeatherIcon.SetImageResource(Resource.Drawable.brokenclouds);
                    break;
                case "09d":
                case "09n":
                    WeatherIcon.SetImageResource(Resource.Drawable.showerrain);
                    break;
                case "10d":
                case "10n":
                    WeatherIcon.SetImageResource(Resource.Drawable.rain);
                    break;
                case "11d":
                case "11n":
                    WeatherIcon.SetImageResource(Resource.Drawable.thunderstorm);
                    break;
                case "13d":
                case "13n":
                    WeatherIcon.SetImageResource(Resource.Drawable.snow);
                    break;
                case "50d":
                case "50n":
                    WeatherIcon.SetImageResource(Resource.Drawable.mist);
                    break;
            }
        }
    }
}