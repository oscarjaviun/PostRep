using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace TextGenerationUtils
{
    /// <summary>
    /// Clase que permite la generacion de textos para el post de 1KB minimo.
    /// </summary>
    public class TextGenerator
    {
        public static string GetPostText(int maxSize)
        {
            char[] chars = "abcdefghijklmnñopqrstuvwxyzABCDEFGHIJKLMNÑOPQRSTUVWXYZ1234567890#$*+- ".ToCharArray();

            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[maxSize];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                switch ((chars[b % (chars.Length)]))
                {
                    case '#': result.Append(". " + Environment.NewLine); break;
                    case '$': result.Append(". "); break;
                    case '*': result.Append(", "); break;
                    case '+':
                    case '-': result.Append(" "); break;
                    default: result.Append(chars[b % (chars.Length)]); break;
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Genera el texto en paralelo.
        /// </summary>
        /// <returns>Candena generada de mas de 1KB</returns>
        public static string GetPostTextParallel()
        {
            StringBuilder result = new StringBuilder(1024);
            Task<string>[] taskArray = { Task<string>.Factory.StartNew(() => GetPostText(512)),
                                     Task<string>.Factory.StartNew(() => GetPostText(512)) };

            for (int i = 0; i < taskArray.Length; i++)
            {
                result.Append(taskArray[i].Result);
            }
            return result.ToString();
        }
    }
}
