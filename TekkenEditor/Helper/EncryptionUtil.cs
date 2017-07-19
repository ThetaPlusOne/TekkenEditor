using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TekkenEditor
{
    class EncryptionUtil
    {
        private static UInt32 Chksum(byte[] data)
        {
            UInt32 sum = 0;
            for (int i = 0; i < (data.Length - SaveConstant.PADDING_SIZE - SaveConstant.SALT_SIZE - SaveConstant.CHKSUM_SIZE); i++)
            {
                sum += (UInt32)(data[i + SaveConstant.SALT_SIZE + SaveConstant.CHKSUM_SIZE] * (i + 1));
            }
            return sum;
        }


        private static UInt32[] BuildTable(UInt32 salt, int length)
        {
            UInt32[] table = new UInt32[length];

            for (int i = 0; i < length; i++)
            {
                UInt32 tmp = (salt >> 13) & 0xFFFFFFFF;
                salt ^= tmp;

                tmp = (salt << 17) & 0xFFFFFFFF;
                salt ^= tmp;
                tmp = salt >> 15 & 0xFFFFFFFF;
                salt ^= tmp;
                table[i] = (UInt32)(salt % length);

            }
            return table;
        }


        public static byte[] Decrypt(byte[] cipher)
        {
            byte[] key = Encoding.ASCII.GetBytes(SaveConstant.KEY_STRING);

            MemoryStream memoryStream;
            CryptoStream cryptoStream;
            Rijndael rijndael = Rijndael.Create();
            rijndael.Mode = CipherMode.ECB;
            rijndael.Key = key;
            rijndael.Padding = PaddingMode.Zeros;


            memoryStream = new MemoryStream();
            cryptoStream = new CryptoStream(memoryStream, rijndael.CreateDecryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(cipher, 0, cipher.Length);
            cryptoStream.FlushFinalBlock();
            cryptoStream.Close();

            byte[] data = memoryStream.ToArray();

            UInt32 salt = BitConverter.ToUInt32(data, 0);

            int msgLen = data.Length - SaveConstant.SALT_SIZE - SaveConstant.PADDING_SIZE;
            UInt32[] table = BuildTable(salt, msgLen);

            for (int i = msgLen - 1; i >= 0; i--)
            {

                byte tmp = data[i + SaveConstant.SALT_SIZE];
                data[i + SaveConstant.SALT_SIZE] = data[table[i] + SaveConstant.SALT_SIZE];
                data[table[i] + SaveConstant.SALT_SIZE] = tmp;
            }

            for (int i = 0; i < msgLen; i++)
            {

                data[i + SaveConstant.SALT_SIZE] = (byte)((data[i + SaveConstant.SALT_SIZE] - (table[i] & 0xFF00 >> 8)) & 0xFF);

            }
            return data;
        }


        public static byte[] Encrypt(byte[] scrData)
        {
            byte[] data = new byte[scrData.Length]; 
            Array.Copy(scrData, data, scrData.Length);
            int msgLen = data.Length - SaveConstant.SALT_SIZE - SaveConstant.PADDING_SIZE;


            UInt32 sum = Chksum(data);
            data[4] = (byte)(sum & 0xFF);
            data[5] = (byte)(sum >> 8 & 0xFF);
            data[6] = (byte)(sum >> 16 & 0xFF);
            data[7] = (byte)(sum >> 24 & 0xFF);

            UInt32 salt = BitConverter.ToUInt32(data, 0);
            UInt32[] table = BuildTable(salt, msgLen);

            for (int i = 0; i < msgLen; i++)
            {

                data[i + SaveConstant.SALT_SIZE] = (byte)((data[i + SaveConstant.SALT_SIZE] + (table[i] & 0xFF00 >> 8)) & 0xFF);

            }
            for (int i = 0; i < msgLen; i++)
            {
                byte tmp = data[i + SaveConstant.SALT_SIZE];
                data[i + SaveConstant.SALT_SIZE] = data[table[i] + SaveConstant.SALT_SIZE];
                data[table[i] + SaveConstant.SALT_SIZE] = tmp;
            }

            byte[] key = Encoding.ASCII.GetBytes(SaveConstant.KEY_STRING);
            MemoryStream memoryStream;
            CryptoStream cryptoStream;
            Rijndael rijndael = Rijndael.Create();
            rijndael.Mode = CipherMode.ECB;
            rijndael.Key = key;
            rijndael.Padding = PaddingMode.Zeros;


            memoryStream = new MemoryStream();
            cryptoStream = new CryptoStream(memoryStream, rijndael.CreateEncryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(data, 0, data.Length);
            cryptoStream.FlushFinalBlock();
            cryptoStream.Close();

            byte[] cipher = memoryStream.ToArray();




            return cipher;

        }

    }

}

