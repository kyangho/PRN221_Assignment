using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace SignalR.Helper
{
    public class HashPasswordHepler
    {
        public static string HashPassword(string password)
        {
            // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: Convert.FromBase64String("TvEqizdqKecFVmnZGi69Vg=="),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));


            return hashedPassword;
        }
    }
}
