using System;
using System.Security.Cryptography;
using System.Text;

public class SessionIdGenerator
{
    public static string Generate(TestableStatusDataBase statusData)
    {
        string value = $"{statusData.GameName}_{statusData.TrackName}_{statusData.SessionName}";

        // byte array representation of that string
        byte[] encodedValue = new UTF8Encoding().GetBytes(value);

        // need MD5 to calculate the hash
        byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedValue);

        return BitConverter.ToString(hash).ToLower().Replace("-", "");
    }
}