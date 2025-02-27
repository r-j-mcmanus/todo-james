
/*
    JWT = hhhhhh.bbbbbbb.sssssss
*/

using System.Buffers.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore.Storage.Json;
using System.Text;


class Header {
    public string alg = "HS256";
    public string typ = "JWT";
}

class Body {
    public string? iss;
    public string? sub;
    public string? aud;
    public long?  exp;
    public long? nbf;
    public long? iat;
    public string? jti;
};

public static class JsonWebToken {

    public static string makeToken()
    {

        string? salt = Environment.GetEnvironmentVariable("JWT_SALT");

        if (salt == null) {
            throw new Exception("JWT_SALT env var is not set!");
        }

        var header = new Header();
        var body = new Body();

        string headerString  = JsonSerializer.Serialize(header);
        string bodyString  = JsonSerializer.Serialize(body);

        string headerString64 = Base64UrlEncode(headerString);
        string bodyString64 = Base64UrlEncode(bodyString);

        string token = $"{headerString64}.{bodyString64}";

        byte[] hash = Hasher.SaltAndHash(token, Encoding.UTF8.GetBytes(salt));
        string hashString = Base64UrlEncode(hash);

        string secureToken = $"{token}.{hashString}";

        return secureToken;
    }

    public static bool validateToken(string input){
        // xxx.yyy.sss
        string? salt = Environment.GetEnvironmentVariable("JWT_SALT");

        if (salt == null) {
            throw new Exception("Cannot load env var JWT_SALT");
        }

        int lastDotIndex = input.LastIndexOf('.');

        if(lastDotIndex == -1){ // might need to check if lastDotIndex is len
            return false;
        }

        string inputSig = input.Substring(lastDotIndex+1);
        string inputToken = input.Substring(0, lastDotIndex);

        byte[] tokenSig = Hasher.SaltAndHash(inputToken, salt);
        string tokenSigStr = Base64UrlEncode(tokenSig);

        if(inputSig == tokenSigStr){
            return true;
        }
        
        return false;
    }

    private static string Base64UrlEncode(string input) {
        string output = Convert.ToBase64String(Encoding.UTF8.GetBytes(input))
                                .Replace("+", "-")
                                .Replace("/", "_")
                                .TrimEnd('=');
        return output;
    }

    private static string Base64UrlEncode(byte[] input) {
        string output = Convert.ToBase64String(input)
                                .Replace("+", "-")
                                .Replace("/", "_")
                                .TrimEnd('=');
        return output;
    }

}