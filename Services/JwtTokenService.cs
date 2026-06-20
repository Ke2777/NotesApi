using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using NotesApi.Models;

namespace NotesApi.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string CreateToken(User user)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        DateTimeOffset expires = now.AddHours(8);

        Dictionary<string, object> header = new()
        {
            ["alg"] = "HS256",
            ["typ"] = "JWT"
        };

        Dictionary<string, object> payload = new()
        {
            ["sub"] = user.Id.ToString(),
            ["name"] = user.Username,
            ["email"] = user.Email,
            ["iat"] = now.ToUnixTimeSeconds(),
            ["exp"] = expires.ToUnixTimeSeconds()
        };

        string encodedHeader = Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(header));
        string encodedPayload = Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(payload));
        string unsignedToken = $"{encodedHeader}.{encodedPayload}";
        string signature = CreateSignature(unsignedToken);

        return $"{unsignedToken}.{signature}";
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        string[] parts = token.Split('.');

        if (parts.Length != 3)
        {
            return null;
        }

        string unsignedToken = $"{parts[0]}.{parts[1]}";
        string expectedSignature = CreateSignature(unsignedToken);

        if (!CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(expectedSignature),
            Encoding.UTF8.GetBytes(parts[2])))
        {
            return null;
        }

        byte[] payloadBytes;

        try
        {
            payloadBytes = Base64UrlDecode(parts[1]);
        }
        catch
        {
            return null;
        }

        using JsonDocument document = JsonDocument.Parse(payloadBytes);
        JsonElement root = document.RootElement;

        if (!root.TryGetProperty("sub", out JsonElement subject) ||
            !root.TryGetProperty("exp", out JsonElement expires))
        {
            return null;
        }

        long expiresUnix = expires.GetInt64();

        if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() >= expiresUnix)
        {
            return null;
        }

        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.NameIdentifier, subject.GetString() ?? string.Empty)
        };

        if (root.TryGetProperty("name", out JsonElement name))
        {
            claims.Add(new Claim(ClaimTypes.Name, name.GetString() ?? string.Empty));
        }

        if (root.TryGetProperty("email", out JsonElement email))
        {
            claims.Add(new Claim(ClaimTypes.Email, email.GetString() ?? string.Empty));
        }

        ClaimsIdentity identity = new(claims, "Bearer");
        return new ClaimsPrincipal(identity);
    }

    private string CreateSignature(string value)
    {
        byte[] key = Encoding.UTF8.GetBytes(GetSecretKey());
        byte[] bytes = Encoding.UTF8.GetBytes(value);

        using HMACSHA256 hmac = new(key);
        return Base64UrlEncode(hmac.ComputeHash(bytes));
    }

    private string GetSecretKey()
    {
        string? secret = _configuration["Jwt:Key"];

        if (string.IsNullOrWhiteSpace(secret) || secret.Length < 32)
        {
            throw new InvalidOperationException("Jwt:Key must be at least 32 characters long.");
        }

        return secret;
    }

    private static string Base64UrlEncode(byte[] bytes)
    {
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static byte[] Base64UrlDecode(string value)
    {
        string padded = value
            .Replace('-', '+')
            .Replace('_', '/');

        padded = padded.PadRight(padded.Length + ((4 - padded.Length % 4) % 4), '=');

        return Convert.FromBase64String(padded);
    }
}
