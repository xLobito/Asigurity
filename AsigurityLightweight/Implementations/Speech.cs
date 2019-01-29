using System;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Speech;
using Android.Util;
using AsigurityLightweight.Interfaces;

namespace AsigurityLightweight.Implementations
{
    public class Speech : Java.Lang.Object, ISpeech, IRecognitionListener
    {
        public static AutoResetEvent AutoResetEvent = new AutoResetEvent(false);
        public static string SpeechText = string.Empty;
        private SpeechRecognizer SpeechRecognizer = null;
        private Intent SpeechIntent = null;
        //private const int SpeechIntentRequest = 10;

        public async Task<string> SpeechToText()
        {
            try
            {
                SpeechRecognizer = SpeechRecognizer.CreateSpeechRecognizer(Application.Context);
                SpeechRecognizer.SetRecognitionListener(this);
                SpeechIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
                SpeechIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
                SpeechIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 1500);
                SpeechIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 1500);
                SpeechIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 15000);
                SpeechRecognizer.StartListening(SpeechIntent);
                SpeechText = string.Empty;
                AutoResetEvent.Reset();
                await Task.Run(() => { AutoResetEvent.WaitOne(new TimeSpan(0, 2, 0)); });
                return SpeechText;
            }
            catch(Exception e)
            {
                Log.Debug("Asigurity Speech", "Exception: " + e.Message);
                return string.Empty;
            }
        }

        public void OnBeginningOfSpeech()
        {
            Log.Debug("Asigurity Speech", "OnBeginningOfSpeech");
        }

        public void OnBufferReceived(byte[] buffer)
        {
            Log.Debug("Asigurity Speech", "OnBufferReceived");
        }

        public void OnEndOfSpeech()
        {
            Log.Debug("Asigurity Speech", "OnEndOfSpeech");
        }

        public void OnError([GeneratedEnum] SpeechRecognizerError error)
        {
            Log.Debug("Asigurity Speech", "OnError: " + error.ToString());
        }

        public void OnEvent(int eventType, Bundle @params)
        {
            Log.Debug("Asigurity Speech", "OnEvent");
        }

        public void OnPartialResults(Bundle partialResults)
        {
            Log.Debug("Asigurity Speech", "OnPartialResults");
        }

        public void OnReadyForSpeech(Bundle @params)
        {
            Log.Debug("Asigurity Speech", "OnReadyForSpeech");
        }

        public void OnResults(Bundle results)
        {
            var Matches = results.GetStringArrayList(SpeechRecognizer.ResultsRecognition);

            if(Matches.Count != 0)
            {
                var TextRead = Matches[0];
                if (TextRead.Length > 500)
                    TextRead = TextRead.Substring(0, 500);
                SpeechText = TextRead;
            }
            AutoResetEvent.Set();
        }

        public void OnRmsChanged(float rmsdB)
        {
            Log.Debug("Asigurity Speech", "OnRmsChanged");
        }
    }
}