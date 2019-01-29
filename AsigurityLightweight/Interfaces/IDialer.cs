using System.Threading.Tasks;

namespace AsigurityLightweight.Interfaces
{
    public interface IDialer
    {
        void CallFromContactName(string ContactName);
        string GetPhoneNumberFromContactName(string ContactName);
    }
}