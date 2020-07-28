using System;
using System.Security.Cryptography;

namespace AI_lib
{
    internal class CryptoRandomizer : RandomNumberGenerator
    {
        private readonly RNGCryptoServiceProvider randomProvider = new RNGCryptoServiceProvider();
        public CryptoRandomizer()
        {

        }


        public int Next(int min, int max)
        {
            // So max is inclusive
            max++;
            uint scale = uint.MaxValue;
            while (scale == uint.MaxValue)
            {
                // Get four random bytes.
                byte[] four_bytes = new byte[4];
                randomProvider.GetBytes(four_bytes);

                // Convert that into an uint.
                scale = BitConverter.ToUInt32(four_bytes, 0);
            }

            // Add min to the scaled difference between max and min.
            return (int)(min + (max - min) *
                (scale / (double)uint.MaxValue));
        }

        public override void GetBytes(byte[] data)
        {
            randomProvider.GetBytes(data);
        }

        public override void GetNonZeroBytes(byte[] data)
        {
            randomProvider.GetNonZeroBytes(data);
        }
    }
}