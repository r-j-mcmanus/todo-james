using System.Security.Cryptography;
using System.Text;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TodoApi.Test")]
public static class PasswordHasher
{
    private static readonly int saltByteLen = 32;


    public static string HashPassword(string password)
    {
        byte[] salt = new byte[saltByteLen];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        
        byte[] passwordHash = Hasher.SaltAndHash(password, salt);

        string passwordHashString = Convert.ToBase64String(passwordHash);
        string saltString = Convert.ToBase64String(salt);
        string hashedPasswordAndSalt = passwordHashString + saltString;

        return hashedPasswordAndSalt;
    }


    public static bool VerifyPassword(string providedPassword, string hashedPasswordWithSalt)
    {
        byte[] textBytes = Encoding.UTF8.GetBytes(hashedPasswordWithSalt);
        byte[] salt = new byte[saltByteLen];
        byte[] hashedPassword = new byte[hashedPasswordWithSalt.Length - saltByteLen];


        Buffer.BlockCopy(
            textBytes, // what we are copying
            textBytes.Length - saltByteLen, // which index we are copying it from
            salt, // what we copy it into
            0, // which index in saltAndTextBytes we want to copy into
            saltByteLen // how much we copy over
        );

        Buffer.BlockCopy(
            textBytes, // what we are copying
            0, // which index we are copying it from
            hashedPassword, // what we copy it into
            0, // which index in saltAndTextBytes we want to copy into
            textBytes.Length - saltByteLen // how much we copy over
        );

        byte[] hashedProvidedPassword = Hasher.SaltAndHash(providedPassword, salt);

        if (hashedPassword.Length != hashedProvidedPassword.Length)
        {
            return false;
        }
        for(int i = 0; i < hashedPassword.Length; i++)
        {
            if (hashedPassword[i] != hashedProvidedPassword[i])
            {
                return false;
            }
        }
        return true;
    }
}