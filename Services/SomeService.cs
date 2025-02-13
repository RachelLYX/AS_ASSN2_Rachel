namespace AS_ASSN2_Rachel.Services
{
    public class SomeService
    {
        public string DecryptSensitiveData(string encryptedData)
        {
            return EncryptionHelper.Decrypt(encryptedData);
        }

        public string EncryptSensitiveData(string data)
        {
            return EncryptionHelper.Encrypt(data);
        }
    }
}
