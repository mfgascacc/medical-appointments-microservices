using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace People.Api.Security
{
    public class JwtTokenService
    {
        public string GenerateToken(string username, int expiresInMinutes)
        {
            var now = DateTimeOffset.UtcNow;
            var exp = now.AddMinutes(expiresInMinutes).ToUnixTimeSeconds();

            var header = new
            {
                alg = "HS256",
                typ = "JWT"
            };

            var payload = new
            {
                sub = username,
                iss = Issuer,
                aud = Audience,
                iat = now.ToUnixTimeSeconds(),
                exp = exp
            };

            var headerPart = Base64UrlEncode(JsonConvert.SerializeObject(header));
            var payloadPart = Base64UrlEncode(JsonConvert.SerializeObject(payload));
            var signature = ComputeSignature($"{headerPart}.{payloadPart}", Secret);

            return $"{headerPart}.{payloadPart}.{signature}";
        }

        public bool ValidateToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            var parts = token.Split('.');
            if (parts.Length != 3)
            {
                return false;
            }

            var expected = ComputeSignature($"{parts[0]}.{parts[1]}", Secret);
            if (!SecureEquals(parts[2], expected))
            {
                return false;
            }

            JwtPayload payload;
            try
            {
                var payloadJson = Base64UrlDecode(parts[1]);
                payload = JsonConvert.DeserializeObject<JwtPayload>(payloadJson);
            }
            catch
            {
                return false;
            }

            if (payload == null || payload.exp <= DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            {
                return false;
            }

            if (!string.Equals(payload.iss, Issuer, StringComparison.Ordinal) ||
                !string.Equals(payload.aud, Audience, StringComparison.Ordinal))
            {
                return false;
            }

            return true;
        }

        public bool ValidateCredentials(string username, string password)
        {
            return string.Equals(username, AuthUsername, StringComparison.Ordinal) &&
                   string.Equals(password, AuthPassword, StringComparison.Ordinal);
        }

        private static string ComputeSignature(string content, string secret)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(content));
                return Base64UrlEncode(hash);
            }
        }

        private static bool SecureEquals(string a, string b)
        {
            var left = Encoding.UTF8.GetBytes(a ?? string.Empty);
            var right = Encoding.UTF8.GetBytes(b ?? string.Empty);
            if (left.Length != right.Length)
            {
                return false;
            }

            var diff = 0;
            for (var i = 0; i < left.Length; i++)
            {
                diff |= left[i] ^ right[i];
            }

            return diff == 0;
        }

        private static string Base64UrlEncode(string value)
        {
            return Base64UrlEncode(Encoding.UTF8.GetBytes(value));
        }

        private static string Base64UrlEncode(byte[] bytes)
        {
            return Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        private static string Base64UrlDecode(string value)
        {
            var normalized = value.Replace('-', '+').Replace('_', '/');
            switch (normalized.Length % 4)
            {
                case 2: normalized += "=="; break;
                case 3: normalized += "="; break;
            }

            return Encoding.UTF8.GetString(Convert.FromBase64String(normalized));
        }

        private static string ReadSetting(string key, string fallback)
        {
            var value = ConfigurationManager.AppSettings[key];
            return string.IsNullOrWhiteSpace(value) ? fallback : value;
        }

        private static string Secret => ReadSetting("JwtSecret", "change-this-secret-in-config");
        private static string Issuer => ReadSetting("JwtIssuer", "medical-appointments");
        private static string Audience => ReadSetting("JwtAudience", "medical-appointments-clients");
        private static string AuthUsername => ReadSetting("JwtUsername", "admin");
        private static string AuthPassword => ReadSetting("JwtPassword", "admin123");

        private class JwtPayload
        {
            public string iss { get; set; }
            public string aud { get; set; }
            public long exp { get; set; }
        }
    }
}
