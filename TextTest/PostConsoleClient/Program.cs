using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextGenerationUtils;
using System.Threading;
using System.Net.Http;
using Newtonsoft.Json;
using System.Configuration;
using System.Diagnostics;


namespace PostConsoleClient
{
    class Program
    {
        /// <summary>
        /// Programa agente que genera post.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            
            Console.WriteLine("Cliente de Consola para Post");
            Console.WriteLine("Iniciar (s/n)?");
            string s = Console.ReadKey().KeyChar.ToString().ToLower();
            if (s.Equals("s"))
            {
                Console.WriteLine("Presione ESC para finalizar..");
                do
                {
                    while (!Console.KeyAvailable)
                    {
                        GeneratePosts();
                        break;
                    }
                } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
            }
            else 
            {
                Console.WriteLine();
                Console.WriteLine("Oprima una tecla para continuar...");
                Console.Read();
            }
        }

        public static void GeneratePosts()
        {
            var stopwatch = new Stopwatch();
            int postToDo = Convert.ToInt32(ConfigurationManager.AppSettings["PostQuantity"].ToString());
            int i = 0;
            string postText;
            stopwatch.Start();
            while (i<postToDo)
            {
                try
                {
                    i++;
                    if (i % 5 == 0) Thread.Sleep(2);
                    postText = TextGenerator.GetPostText(1024);
                    AddPost(postText);
                    Console.WriteLine("Cadena: " + i.ToString() + " Size: " + postText.Length.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            stopwatch.Stop();
            Console.WriteLine("Tiempo total: " + stopwatch.ElapsedMilliseconds.ToString());
            Console.WriteLine();
            Console.WriteLine("Generacion finalizada...");
            GC.Collect();
        }
        /// <summary>
        /// Metodo que se encargaa de enviar un post.
        /// </summary>
        /// <param name="posttext"></param>
        public static async void  AddPost(string posttext)
        {
            string result = string.Empty;
            try
            {
                string URI = ConfigurationManager.AppSettings["EndPoint"];
                if (!posttext.Equals(string.Empty))
                {
                    PostText post = new PostText() { Content = posttext };

                    using (var client = new HttpClient())
                    {
                        var serializedPost = JsonConvert.SerializeObject(post);
                        var content = new StringContent(serializedPost, Encoding.UTF8, "application/json");
                        var tempResult = await client.PostAsync(URI, content);
                    }
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
        }
    }
}
