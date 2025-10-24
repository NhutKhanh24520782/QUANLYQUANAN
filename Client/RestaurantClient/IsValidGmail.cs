using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace RestaurantClient
{
    public static class IsValidGmail
    {
        private const string GmailPattern = @"^[\w-\.]+@((gmail\.com)|(googlemail\.com))$";

        public static bool GmailCheck(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            string lowerCaseEmail = email.ToLowerInvariant();

            try
            {
                return Regex.IsMatch(lowerCaseEmail, GmailPattern);
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
    }
}

