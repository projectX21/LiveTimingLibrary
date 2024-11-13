using System;
using System.Security.Cryptography;
using System.Text;

public class SessionIdGenerator
{
    public static string Generate(TestableGameData gameData)
    {
        string value = $"{gameData.GameName}_{NormalizeTrackName(gameData.TrackName)}";
        return value.ToLower().Replace(" ", "_");

        /*
        SimHub.Logging.Current.Info($"Session ID: {value}");

        // byte array representation of that string
        byte[] encodedValue = new UTF8Encoding().GetBytes(value);

        // need MD5 to calculate the hash
        byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedValue);

        SimHub.Logging.Current.Info($"Calculated hash value for SessionID ({value}): {BitConverter.ToString(hash).ToLower().Replace("-", "")}");
        */

    }

    private static string NormalizeTrackName(string trackName)
    {
        // ACC has a weird bug, where the track name is constantly changing from 'Circuit Paul Ricard' to 'Paul Ricard' and vice versa
        if (trackName.Contains("Paul Ricard"))
        {
            return "Paul Ricard";
        }

        return trackName;
    }
}