using BSE365.Common.Constants;
using System;
using System.Security.Cryptography;

namespace BSE365.Common.Helper
{
    public static class Utilities
    {
        public static string GetHash(string input)
        {
            HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider();

            byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(input);

            byte[] byteHash = hashAlgorithm.ComputeHash(byteValue);

            return Convert.ToBase64String(byteHash);
        }

        public static string StandardizeUserId(object userName)
        {
            if (userName == null) return string.Empty;
            return Convert.ToString(userName).PadLeft(SystemAdmin.UserIdLength, SystemAdmin.FillInChar);
        }
    }
}
