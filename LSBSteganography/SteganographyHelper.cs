using LSBSteganography.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSBSteganography
{
    public static class SteganographyHelper
    {
        static byte[] masks = new byte[] { (byte)128, (byte)64, (byte)32, (byte)16, (byte)8, (byte)4, (byte)2, (byte)1 };

        public static Result HideSecretBitmap(string carrierName, string secretName, string resultName, int numberOfBits)
        {
            byte[] carrierBytes = Utils.GetBytesFromFile(carrierName);
            byte[] secretBytes = Utils.GetBytesFromFile(secretName);

            int carrierNumberOfBytes = carrierBytes.Count();
            int secretNumberOfBytes = secretBytes.Count();

            if (carrierNumberOfBytes != secretNumberOfBytes)
                return Result.Fail("The carrier and the secret must be the same size!");

            int mask = (int)(256 - Math.Pow(2, numberOfBits));
            byte[] resultBytes = new byte[carrierNumberOfBytes];

            resultBytes = new byte[secretNumberOfBytes];

            for (int i = 0; i < secretNumberOfBytes; i++)
            {
                if (i < Utils.BitmapHeaderLength) resultBytes[i] = carrierBytes[i];
                else resultBytes[i] = (byte)((carrierBytes[i] & mask) | (secretBytes[i] >> (8 - numberOfBits)));
            }

            File.WriteAllBytes(resultName, resultBytes);

            return Result.Ok();

        }

        public static Result RecoverSecretBitmap(string source, string result, int numberOfBits)
        {
            byte[] sourceBytes = Utils.GetBytesFromFile(source);
            int sourceNumberOfBytes = sourceBytes.Count();
            byte[] resultBytes = new byte[sourceNumberOfBytes];

            for (int i = 0; i < sourceNumberOfBytes; i++)
            {
                if (i < Utils.BitmapHeaderLength)
                    resultBytes[i] = sourceBytes[i];
                else
                    resultBytes[i] = (byte)(sourceBytes[i] << (8 - numberOfBits));
            }

            File.WriteAllBytes(result, resultBytes);

            return Result.Ok();
        }
        

        public static Result HideSecretMessage(string message, string password, string carrierName, string resultName)
        {
            byte[] carrierBytes = Utils.GetBytesFromFile(carrierName);
            byte[] secretBytes = Utils.GetBytesFomString(message);

            int offset = Utils.BitmapHeaderLength;
            int secretNumberOfBytes = secretBytes.Length;
            int carrierNumberOfBytes = carrierBytes.Count();

            if (carrierNumberOfBytes - Utils.BitmapHeaderLength - 32 < secretNumberOfBytes * 8)
            {
                return Result.Fail("The message is too long!");
            }

            uint[] numberSequence = Utils.RandomNumberSequence(password, secretNumberOfBytes * 8, Utils.BitmapHeaderLength + 32 + 1, carrierNumberOfBytes);

            BitArray secretBits = new BitArray(new int[] { secretNumberOfBytes });

            for (int i = 0; i < secretBits.Count; i++)
            {
                if (!secretBits[i])
                    carrierBytes[offset + i] = (byte)(carrierBytes[offset + i] & 254);
                else
                    carrierBytes[offset + i] = (byte)(carrierBytes[offset + i] | 1);
            }

            for (int i = 0; i < secretNumberOfBytes; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((masks[j] & secretBytes[i]) == (byte)0)
                        carrierBytes[numberSequence[i * 8 + j]] = (byte)(carrierBytes[numberSequence[i * 8 + j]] & 254);
                    else
                        carrierBytes[numberSequence[i * 8 + j]] = (byte)(carrierBytes[numberSequence[i * 8 + j]] | 1);
                }
            }

            File.WriteAllBytes(resultName, carrierBytes);

            return Result.Ok();
        }

        public static Result<string> RecoverSecretMessage(string password, string sourceName)
        {
            byte[] sourceBytes = Utils.GetBytesFromFile(sourceName);

            BitArray hiddenMessageBits = new BitArray(new int[] { 0 });

            int offset = Utils.BitmapHeaderLength;

            for (int i = 0; i < 32; i++)
            {
                if ((byte)(sourceBytes[offset + i] & (byte)1) != (byte)0)
                    hiddenMessageBits[i] = true;
            }

            int hiddenMessageLength = Utils.GetIntFromBitArray(hiddenMessageBits);

            uint[] numberSequence = Utils.RandomNumberSequence(password, hiddenMessageLength * 8, Utils.BitmapHeaderLength + 32 + 1, sourceBytes.Length);

            byte[] hiddenMessageBytes = new byte[hiddenMessageLength];

            for (int i = 0; i < hiddenMessageLength; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((sourceBytes[numberSequence[i * 8 + j]] & (byte)1) != (byte)0)
                    {
                        hiddenMessageBytes[i] = (byte)(hiddenMessageBytes[i] | masks[j]);
                    }
                }
            }

            return new Result<string>() { Success = true, Value = Utils.GetStringFromBytes(hiddenMessageBytes) };
        }
    }
}
