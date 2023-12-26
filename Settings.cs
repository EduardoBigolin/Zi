using System.Security.Cryptography;
using System.Text;

namespace Zi
{
    public class Settings
    {
            public static string Secret()
            {
                string secretString = "sua_string_secreta_aqui";

                byte[] secretBytes = Encoding.UTF8.GetBytes(secretString);

                byte[] hmacKey = DeriveHmacKey(secretBytes, 32);

                string base64Key = Convert.ToBase64String(hmacKey);

                return base64Key;
            }

            static byte[] DeriveHmacKey(byte[] secret, int keySizeInBytes)
            {
                using (var hmac = new HMACSHA256(secret))
                {
                    byte[] key = hmac.ComputeHash(secret);

                    Array.Resize(ref key, keySizeInBytes);

                    return key;
                }
            }
    }
}
