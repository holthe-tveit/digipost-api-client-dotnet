using System;
using System.Security.Cryptography;

namespace Digipost.Api.Client.Common.Utilities
{
    public class UUIDInterop
    {
        public static string NameUUIDFromBytes(String input)
        {
            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
            hash[6] &= 0x0f;
            hash[6] |= 0x30;
            hash[8] &= 0x3f;
            hash[8] |= 0x80;
            string hex = BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
            return hex.Insert(8, "-").Insert(13, "-").Insert(18, "-").Insert(23, "-");
        }
    }
}
