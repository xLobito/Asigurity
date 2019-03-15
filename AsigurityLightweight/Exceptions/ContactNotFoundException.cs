using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace AsigurityLightweight.Exceptions
{
    [Serializable]
    public class ContactNotFoundException : Exception
    {
        public ContactNotFoundException()
        {
        }

        public ContactNotFoundException(string message) : base(message)
        {
        }

        public ContactNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ContactNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}