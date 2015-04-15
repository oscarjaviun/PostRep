using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextUtils
{
    /// <summary>
    /// Clase que ejecuta reglas de procesamiento del texto de los post.
    /// </summary>
    public class TextProcesor 
    {
        /// <summary>
        /// Recibe cadena y ejecuta todas las estadisticas en secuencia.
        /// </summary>
        public static string ProcessText(string s)
        {
            string result;
            StringBuilder sb = new StringBuilder();

            sb.Append("--Post Start--" + DateTime.Now.ToString("HH:mm:ss.ffff"));
            sb.Append(Environment.NewLine);
            sb.Append(s);
            sb.Append(Environment.NewLine);
            sb.Append("--Statistics--");
            sb.Append(Environment.NewLine);
            sb.Append("Total Palabras n: ");
            sb.Append(WordnCount(s));
            sb.Append(Environment.NewLine);
            sb.Append("Total Frases mas 15 palabras: ");
            sb.Append(Phrase15Count(s));
            sb.Append(Environment.NewLine);
            sb.Append("Total parrafos: ");
            sb.Append(ParagraphCountU(s));
            sb.Append(Environment.NewLine);
            sb.Append("Total alfanumericos menos n: ");
            sb.Append(AlphaCharsCount(s));
            sb.Append(Environment.NewLine);
            sb.Append("--Post End--");

            result = sb.ToString();
            return result;
        }

        /// <summary>
        /// Recibe cadena y ejecuta las estadisticas en paralelo.
        /// </summary>
        public static string ProcessTextParallel(string s)
        {
            string result;
            StringBuilder sb = new StringBuilder();
            Task<int>[] taskArray = { Task<int>.Factory.StartNew(() => WordnCount(s)),
                                        Task<int>.Factory.StartNew(() => Phrase15Count(s)), 
                                        Task<int>.Factory.StartNew(() => ParagraphCountU(s)), 
                                        Task<int>.Factory.StartNew(() => AlphaCharsCount(s)) };

            sb.Append("--Post Start--" + DateTime.Now.ToString("HH:mm:ss.ffff"));
            sb.Append(Environment.NewLine);
            sb.Append(s);
            sb.Append(Environment.NewLine);
            sb.Append("--Statistics--");
            sb.Append(Environment.NewLine);
            sb.Append("Total Palabras n: ");
            sb.Append(taskArray[0].Result);
            sb.Append(Environment.NewLine);
            sb.Append("Total Frases mas 15 palabras: ");
            sb.Append(taskArray[1].Result);
            sb.Append(Environment.NewLine);
            sb.Append("Total parrafos: ");
            sb.Append(taskArray[2].Result);
            sb.Append(Environment.NewLine);
            sb.Append("Total alfanumericos menos n: ");
            sb.Append(taskArray[3].Result);
            sb.Append(Environment.NewLine);
            sb.Append("--Post End--");

            result = sb.ToString();
            return result;
        }

        /// <summary>
        /// Recibe cadena y genera estadisticas basadas en una sola iteracion.
        /// </summary>
        public static string ProcesTextSingleRun(string s)
        {
            var statistics = AllCount(s);
            string result;
            StringBuilder sb = new StringBuilder();

            sb.Append("--Post Start--" + DateTime.Now.ToString("HH:mm:ss.ffff"));
            sb.Append(Environment.NewLine);
            sb.Append(s);
            sb.Append(Environment.NewLine);
            sb.Append("--Statistics--");
            foreach (var stat in statistics)
            {
                sb.Append(Environment.NewLine);
                sb.Append(stat);
            }
            sb.Append(Environment.NewLine);
            sb.Append("--Post End--");
            result = sb.ToString();
            return result;
        }

        /// <summary>
        /// Metodo que procesa la cadena en una sola iteracion
        /// </summary>
        /// <param name="s">cadena de entrada</param>
        /// <returns>lista de estadisticas</returns>
        public static List<string> AllCount(string s)
        {
            var results = new List<string>();
            int alphaCount = 0, wordNCount = 0, phraseCount = 0, indexStart = 0, indexEnd = 0;
            for (int i = 0; i < s.Length; i++)
            {
                //cuenta los alfanumericos que no son n
                if (!char.IsLetterOrDigit(s[i]) || (s[i] == 'n' || s[i] == 'N')) alphaCount++;
                //cuenta las palabras que terminan en n
                if (char.IsWhiteSpace(s[i]) == true && i >= 1)
                {
                    if ((i > 2) && (s[i - 1] == 'n' || s[i - 1] == 'N' || (char.IsPunctuation(s[i - 1]) && (s[i - 2] == 'n' || s[i - 2] == 'N'))))
                    {
                        wordNCount++;
                    }
                }
                if (i == (s.Length - 1) && (s[i] == 'n' || s[i] == 'N'))
                {
                    wordNCount++;
                }
                //cuenta las frases con mas de 15
                if (s[i] == ' ' && i >= 1)
                {
                    if (s[i - 1] == '.')
                    {
                        indexEnd = i - 1;
                        phraseCount = WordCount(s.Substring(indexStart, indexEnd - indexStart)) > 15 ? phraseCount + 1 : phraseCount;
                        indexStart = indexEnd;
                    }
                }
                if (i == (s.Length - 1) && (s[i] == '.' && char.IsLetterOrDigit(s[i - 1])))
                {
                    phraseCount = WordCount(s.Substring(indexStart, s.Length - 1 - indexStart)) > 15 ? phraseCount + 1 : phraseCount;
                }

            }

            results.Add("Total Palabras n: " + wordNCount.ToString());
            results.Add("Total Frases mas 15 palabras: " + phraseCount.ToString());
            results.Add("Total parrafos: " + ParagraphCountU(s));
            results.Add("Total alfanumericos menos n: " + (s.Length - alphaCount).ToString());
            return results;
        }
        /// <summary>
        /// Recibe cadena cuenta palabras. 
        /// </summary>
        public static int WordCount(string s)
        {
            int wordCount = 0;
            for (int i = 1; i < s.Length; i++)
            {
                if (char.IsWhiteSpace(s[i - 1]) == true)
                {
                    if (char.IsLetterOrDigit(s[i]) == true ||
                        char.IsPunctuation(s[i]))
                    {
                        wordCount++;
                    }
                }

            }
            if (s.Length > 2)
            {
                wordCount++;
            }
            return wordCount;
        }

        /// <summary>
        /// Recibe cadena cuenta palabras que finalizan con n/N. 
        /// </summary>
        public static int WordnCount(string s)
        {
            int wordNCount = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (char.IsWhiteSpace(s[i]) == true && i >= 1)
                {
                    if (s[i - 1] == 'n' || s[i - 1] == 'N' || (char.IsPunctuation(s[i - 1]) && (s[i - 2] == 'n' || s[i - 2] == 'N')))
                    {
                        wordNCount++;
                    }
                }
                if (i == (s.Length - 1) && (s[i] == 'n' || s[i] == 'N'))
                {
                    wordNCount++;
                }
            }

            return wordNCount;

        }

        /// <summary>
        /// Recibe cadena cuenta frases con mas de 15 palabras
        /// </summary>
        public static int Phrase15Count(string s)
        {
            int phraseCount = 0, indexStart = 0, indexEnd = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == ' ' && i >= 1)
                {
                    if (s[i - 1] == '.')
                    {
                        indexEnd = i - 1;
                        phraseCount = WordCount(s.Substring(indexStart, indexEnd - indexStart)) > 15 ? phraseCount + 1 : phraseCount;
                        indexStart = indexEnd;
                    }
                }
                if (i == (s.Length - 1) && (s[i] == '.' && char.IsLetterOrDigit(s[i - 1])))
                {
                    phraseCount = WordCount(s.Substring(indexStart, s.Length - 1 - indexStart)) > 15 ? phraseCount + 1 : phraseCount;
                }

            }
            return phraseCount;
        }

        /// <summary>
        /// Recibe cadena cuenta parrafos
        /// </summary>
        public unsafe static int ParagraphCountU(string s)
        {
            int inputLength = s.Length;
            List<int> indexes = new List<int>();
            string target = Environment.NewLine;

            int len = inputLength;
            fixed (char* i = s, r = target)
            {
                while (--len > -1)
                {
                    if (i[len] == r[0] && i[len + 1] == r[1])
                    {
                        indexes.Add(len--);
                    }
                }
            }

            var idx = indexes.ToArray();
            return indexes.Count + 1;

        }

        /// <summary>
        /// Recibe cadena cuenta caracteres alfanumericos menos la n/N
        /// </summary>
        public static int AlphaCharsCount(string s)
        {
            int alphaCount = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (!char.IsLetterOrDigit(s[i]) || (s[i] == 'n' || s[i] == 'N')) alphaCount++;
            }
            return s.Length - alphaCount;
        }
    }
}
