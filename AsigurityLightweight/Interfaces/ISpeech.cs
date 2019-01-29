using System.Threading.Tasks;

namespace AsigurityLightweight.Interfaces
{
    public interface ISpeech
    {
        Task<string> SpeechToText();
    }
}