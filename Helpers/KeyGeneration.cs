using System.Security.Cryptography;

namespace AS_ASSN2_Rachel.Helpers
{
    public class KeyGeneration
    {
        public static (string Key, string IV) GenerateKeyAndIV()
        {
            using (var aesAlg = Aes.Create())
            {
                aesAlg.GenerateKey();
                aesAlg.GenerateIV();

                string key = Convert.ToBase64String(aesAlg.Key);
                string iv = Convert.ToBase64String(aesAlg.IV);

                return (key, iv);
            }
        }
    }
}
