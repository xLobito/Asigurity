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
    public class CommandNotRecognizedException : Exception
    {
        public CommandNotRecognizedException()
        {
        }

        public CommandNotRecognizedException(string message) : base(message)
        {
        }

        public CommandNotRecognizedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CommandNotRecognizedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}