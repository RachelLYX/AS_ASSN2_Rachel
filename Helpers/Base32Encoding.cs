using System.Text;

namespace AS_ASSN2_Rachel.Helpers
{
    public static class Base32Encoding
    {
        private static readonly char[] Base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567".ToCharArray();

        public static string ToBase32(byte[] bytes)
        {
            StringBuilder builder = new StringBuilder();
            int byteIndex = 0;
            int bitIndex = 0;

            while (byteIndex < bytes.Length)
            {
                int value = (bytes[byteIndex] >> (8 - (bitIndex + 5))) & 0x1F;
                builder.Append(Base32Chars[value]);
                bitIndex = (bitIndex + 5) % 8;
                if (bitIndex == 0)
                {
                    byteIndex++;
                }
            }
            return builder.ToString();
        }
    }
}
