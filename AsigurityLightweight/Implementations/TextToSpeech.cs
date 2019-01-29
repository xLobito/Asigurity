using Android.App;
using Android.Runtime;
using AsigurityLightweight.Interfaces;
using Java.Util;
using TTS = Android.Speech.Tts;

namespace AsigurityLightweight.Implementations
{
    public class TextToSpeech : Java.Lang.Object, ITextToSpeech, TTS.TextToSpeech.IOnInitListener
    {
        private TTS.TextToSpeech Speaker;
        private string Message;
        private readonly Locale esp = new Locale("es", "ES");

        public void Speak(string message)
        {
            Message = message;
            if (Speaker == null)
            {
                Speaker = new TTS.TextToSpeech(Application.Context, this);
            }
            else
                Speaker.Speak(Message, TTS.QueueMode.Flush, null, null);
        }

        public void OnInit([GeneratedEnum] TTS.OperationResult status)
        {
            if(status.Equals(TTS.OperationResult.Success))
            {
                Speaker.SetLanguage(esp);
                Speaker.Speak(Message, TTS.QueueMode.Flush, null, null);
            }
        }
    }
}