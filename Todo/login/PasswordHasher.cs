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

        byte[] hashedPasswordAndSalt = new byte[passwordHash.Length + saltByteLen];

        Buffer.BlockCopy(
            passwordHash, // what we are copying
            0, // which index we are copying it from
            hashedPasswordAndSalt, // what we copy it into
            0, // which index we want to copy into
            passwordHash.Length // how much we copy over
        );

        Buffer.BlockCopy(
            salt, // what we are copying
            0, // which index we are copying it from
            hashedPasswordAndSalt, // what we copy it into
            passwordHash.Length, // which index we want to copy into
            salt.Length // how much we copy over
        );

        string hashedPasswordAndSaltString = Convert.ToBase64String(hashedPasswordAndSalt);

        return hashedPasswordAndSaltString;
    }

    public static bool VerifyPassword(string providedPassword, string hashedPasswordWithSalt)
    {
        byte[] hashedPasswordWithSaltBytes = Convert.FromBase64String(hashedPasswordWithSalt);
        byte[] salt = new byte[saltByteLen];
        byte[] hashedPassword = new byte[hashedPasswordWithSaltBytes.Length - saltByteLen];

        //get the salt from hashedPasswordWithSalt 
        Buffer.BlockCopy(
            hashedPasswordWithSaltBytes, // what we are copying
            hashedPasswordWithSaltBytes.Length - saltByteLen, // which index we are copying it from
            salt, // what we copy it into
            0, // which index in saltAndTextBytes we want to copy into
            saltByteLen // how much we copy over
        );
        // get the hashed password from hashedPasswordWithSalt
        Buffer.BlockCopy(
            hashedPasswordWithSaltBytes, // what we are copying
            0, // which index we are copying it from
            hashedPassword, // what we copy it into
            0, // which index in saltAndTextBytes we want to copy into
            hashedPasswordWithSaltBytes.Length - saltByteLen // how much we copy over
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