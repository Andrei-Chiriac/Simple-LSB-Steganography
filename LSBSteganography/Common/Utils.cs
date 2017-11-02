using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSBSteganography.Common
{
    public static class Utils
    {
        public static int BitmapHeaderLength = 54;
       
        public static string GetBitmap()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            
            dlg.DefaultExt = ".bmp";
            dlg.Filter = "Bitmap (.bmp)|*.bmp";

            bool? result = dlg.ShowDialog();

            return result.Value == true ? dlg.FileName : string.Empty;
        }

        public static byte[] GetBytesFromFile(string file)
        {
            byte[] fileBytes = null;
            FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            long numFileBytes = new FileInfo(file).Length;
            fileBytes = br.ReadBytes((int)numFileBytes);
            fs.Close();

            return fileBytes;
        }

        public static byte[] GetBytesFomString(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string GetStringFromBytes(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public static int GetIntFromBitArray(BitArray bitArray)
        {
            int[] array = new int[1];
            bitArray.CopyTo(array, 0);
            return array[0];
        }

        //https://en.wikipedia.org/wiki/Jenkins_hash_function
        public static uint GetStableHash(string s)
        {
            uint hash = 0;

            foreach (byte b in Encoding.Unicode.GetBytes(s))
            {
                hash += b;
                hash += (hash << 10);
                hash ^= (hash >> 6);
            }

            hash += (hash << 3);
            hash ^= (hash >> 11);
            hash += (hash << 15);

            return hash;
        }


        public static uint[] RandomNumberSequence(string password, int length, int min, int max)
        {
            uint seed = GetStableHash(password);
            return PseudoRandomNumberGenerator(seed, length, min, max);
        }

        //https://en.wikipedia.org/wiki/Blum_Blum_Shub
        public static uint[] PseudoRandomNumberGenerator(uint seed, int count, int min, int max)
        {
            uint[] numbers = new uint[count];

            int i = 0;
            uint n = (seed == 0 || seed == 1) ? 12345 : seed;

            while (i < count)
            {
                n = (uint)(Math.Pow(n, 2) % max);

                if (!numbers.Contains(n) && n > min)
                {
                    numbers[i] = n;
                    i++;
                }
            }

            return numbers;
        }

        public static string Timestamp()
        {
            return DateTime.Now.ToString("yyyyMMddhhmmss");
        }
    }
}
