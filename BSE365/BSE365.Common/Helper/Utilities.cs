using BSE365.Common.Constants;
using System;
using System.Collections.Generic;
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

        public static string StandardizeUserId(int infoId)
        {
            return Convert.ToString(SystemAdmin.StartId + infoId)
                .PadLeft(SystemAdmin.UserIdLength, SystemAdmin.FillInChar);
        }

        public static List<string> GetRangeUserName(int infoId)
        {
            var result = new List<string>();
            var name = StandardizeUserId(infoId);
            for (char i = 'A'; i <= 'F'; i++)
            {
                result.Add(name + i);
            }

            return result;
        }

        public static string GetImageContentType(string extension)
        {
            var ext = extension.ToLower();

            if (ext == "png")
            {
                return "image/png";
            }

            if (ext == ".jpg" || ext == ".jpeg")
            {
                return "image/jpeg";
            }

            return "application/octet-stream";
        }
    }
}