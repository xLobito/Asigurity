using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Provider;
using Android.Util;
using Android.Widget;
using AsigurityLightweight.Exceptions;
using AsigurityLightweight.Interfaces;
using AsigurityLightweight.Utilities;

namespace AsigurityLightweight.Implementations
{
    public class Dialer : TextToSpeech, IDialer
    {
        bool IsContactFound = false;

        internal class Contacts
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string PhoneNumber { get; set; }
            public string FullName { get => $"{ FirstName }{ LastName }"; }
        }

        public void CallFromContactName(string ContactName)
        {
            Intent DialIntent;
            IEnumerable<Contacts> ContactsList;
            string NormalizedName = TrimOut(RemoveAccents(ContactName));
            try
            {
                ContactsList = RetrieveContactsList();
                foreach (var Contact in ContactsList)
                {
                    /**
                     * if (string.IsNullOrEmpty(Contact.LastName) && string.Equals(NormalizedName, RemoveAccents(Contact.FirstName), StringComparison.OrdinalIgnoreCase))
                     */
                    string ContactFirstNameNormalized = Contact.FirstName;
                    if (string.IsNullOrEmpty(Contact.LastName) && string.Equals(NormalizedName, ContactFirstNameNormalized, StringComparison.OrdinalIgnoreCase) || (LevenshteinDistance.GetLevenshteinPercentage(NormalizedName, ContactFirstNameNormalized) < 0.75))
                    {
                        IsContactFound = true;
                        DialIntent = new Intent(Intent.ActionCall);
                        DialIntent.SetData(Android.Net.Uri.Parse("tel: " + Contact.PhoneNumber));
                        Application.Context.StartActivity(DialIntent);
                    }
                    else
                    {
                        if (string.Equals(NormalizedName, RemoveAccents(Contact.FullName), StringComparison.OrdinalIgnoreCase))
                        {
                            IsContactFound = true;
                            DialIntent = new Intent(Intent.ActionCall);
                            DialIntent.SetData(Android.Net.Uri.Parse("tel: " + Contact.PhoneNumber));
                            Application.Context.StartActivity(DialIntent);
                        }
                    }
                }
                if (!IsContactFound)
                    throw new ContactNotFoundException("No se ha encontrado el contacto solicitado");
            }
            catch (Exception ex)
            {
                Speak("No se ha encontrado el contacto solicitado");
            }
        }

        public string GetPhoneNumberFromContactName(string ContactName)
        {
            string ContactPhoneNumber = string.Empty;
            string NormalizedName = RemoveAccents(ContactName);
            IEnumerable<Contacts> ContactsList = null;

            try
            {
                ContactsList = RetrieveContactsList();
                ContactPhoneNumber = CheckPhoneNumberExistance(NormalizedName, ContactsList);
            }
            catch (Exception ex)
            {
                Log.Debug("Asigurity", ex.Message);
            }
            return ContactPhoneNumber;
        }

        private string CheckPhoneNumberExistance(string NormalizedName, IEnumerable<Contacts> ContactsList)
        {
            string ContactPhoneNumber = string.Empty;
            string ContactFirstNameNormalized = string.Empty;

            foreach (var Contact in ContactsList)
            {
                /* Añadir Levenshtein */
                /** 
                 * if((string.IsNullOrEmpty(Contact.LastName) && string.Equals(NormalizedName, RemoveAccents(Contact.FirstName), StringComparison.OrdinalIgnoreCase)) || (LevenshteinDistance.GetLevenshteinPercentage(NormalizedName, RemoveAccents(Contact.FirstName)) < 0.75)) 
                 */
                ContactFirstNameNormalized = Contact.FirstName;
                if (string.IsNullOrEmpty(Contact.LastName) && string.Equals(NormalizedName, ContactFirstNameNormalized, StringComparison.OrdinalIgnoreCase) || (LevenshteinDistance.GetLevenshteinPercentage(NormalizedName, ContactFirstNameNormalized) < 0.75))
                {
                    ContactPhoneNumber = Contact.PhoneNumber;
                    if (ContactPhoneNumber.StartsWith("+"))
                    {
                        ContactPhoneNumber = ContactPhoneNumber[1] + ContactPhoneNumber.Substring(2);
                    }
                    else
                    {
                        if (ContactPhoneNumber.StartsWith("9"))
                        {
                            ContactPhoneNumber = "56" + ContactPhoneNumber;
                        }
                    }
                }
            }
            return ContactPhoneNumber;
        }

        private string TrimOut(string ContactName)
        {
            if (ContactName.StartsWith("a ", StringComparison.OrdinalIgnoreCase))
                return ContactName.Substring(2);
            else
                return ContactName;
        }

        private string RemoveAccents(string ContactName)
        {
            return string.Concat(ContactName.Normalize(NormalizationForm.FormD).Where(ch => CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)).Normalize(NormalizationForm.FormC);
        }

        private IEnumerable<Contacts> RetrieveContactsList()
        {
            List<Contacts> PhoneContacts = new List<Contacts>();
            Contacts Contact = null;
            string Name = string.Empty;
            string PhoneNumber = string.Empty;
            string[] SeparatedName;

            using (var PhoneCts = Application.Context.ContentResolver.Query(ContactsContract.CommonDataKinds.Phone.ContentUri, null, null, null, null))
            {
                if(PhoneCts != null)
                {
                    while(PhoneCts.MoveToNext())
                    {
                        try
                        {
                            Name = PhoneCts.GetString(PhoneCts.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.DisplayName));
                            PhoneNumber = PhoneCts.GetString(PhoneCts.GetColumnIndex(ContactsContract.CommonDataKinds.Phone.Number));
                            SeparatedName = Name.Split(' ');
                            if (SeparatedName.Length > 1)
                                Contact = new Contacts { FirstName = SeparatedName[0], LastName = SeparatedName[1], PhoneNumber = PhoneNumber };
                            else
                                Contact = new Contacts { FirstName = SeparatedName[0], LastName = string.Empty, PhoneNumber = PhoneNumber };
                            PhoneContacts.Add(Contact);
                        }
                        catch(Exception)
                        {
                            Speak("No se pueden obtener los datos de contacto del teléfono");
                        }
                    }
                }
            }
            return PhoneContacts;
        }
    }
}