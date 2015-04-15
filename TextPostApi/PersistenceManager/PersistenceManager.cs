using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace PersistenceManager
{
    /// <summary>
    /// Clase para manejar la persistencia de los post.
    /// </summary>
    public class PersistenceMgr
    {
        private static PersistenceMgr instance;
        private int _maxQueueSize = 3000;
        private ConcurrentQueue<string> postQueue;
        private int fileCounter;
        private string path;
        private System.Timers.Timer timer;
        private static readonly Object thisLock = new Object();

        public static PersistenceMgr Instance
        {
            get
            {
                lock (thisLock)
                {
                    if (instance == null)
                    {
                        instance = new PersistenceMgr();
                    }
                    return instance;
                }
            }
        }

        private PersistenceMgr()
        {
            postQueue = new ConcurrentQueue<string>();
            fileCounter = 0;
            path = CreateIfMissing("C:\\PostServerData\\");
            timer = new System.Timers.Timer(_maxQueueSize);
            timer.Elapsed += timer_Elapsed;
            timer.Enabled = true;
        }

        /// <summary>
        /// Evento del temporizador de descarga de la cola.
        /// </summary>
        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            DeQueuePosts();
        }

        /// <summary>
        /// Metodo que se encarga de la persistencia, saca los datos de la cola.
        /// </summary>
        private void DeQueuePosts()
        {
            if (postQueue.IsEmpty) return;
            string post;
            fileCounter++;
            StringBuilder sb = new StringBuilder();
            sb.Append("CONTENIDO: " + postQueue.Count().ToString() + "POST");

            while (postQueue.TryDequeue(out post))
            {
                sb.Append(Environment.NewLine);
                sb.Append(post);
            }

            this.WriteFile(sb.ToString());
            GC.Collect();
        }

        public void ProcessPostData(string post)
        {
            if (postQueue.Count % 4 == 0) Thread.Sleep(1);
            postQueue.Enqueue(post);
        }

        /// <summary>
        /// Metodo utilitario para crear el directorio de los resultados.
        /// </summary>
        private string CreateIfMissing(string path)
        {
            bool folderExists = Directory.Exists(path);
            if (!folderExists)
                Directory.CreateDirectory(path);
            return path;
        }

        /// <summary>
        /// Metodo que permite persistir los post.
        /// </summary>
        /// <param name="q"></param>
        private void PersistPost(ConcurrentQueue<string> q)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("CONTENIDO: " + q.Count().ToString() + "POST");
            while (q.Count > 0)
            {
                sb.Append(Environment.NewLine);
                string s;
                q.TryDequeue(out s);
                sb.Append(s);
            }

            string fileContent = sb.ToString();
            this.WriteFile(fileContent);
        }

        /// <summary>
        /// Metodo que resuelve el nombre del archivo a crear.
        /// </summary>
        /// <returns></returns>
        private string GetFileName()
        {
            string filePath = path + DateTime.Now.ToString("dd-MM", CultureInfo.InvariantCulture) + "_" + fileCounter.ToString() + ".txt";
            return filePath;
        }

        /// <summary>
        /// Metodo que permite escribir un archivo en disco.
        /// </summary>
        /// <param name="fileContent"></param>
        private void WriteFile(string fileContent)
        {
            string filename = GetFileName();
            BinaryWriter bwFile = new BinaryWriter(File.Open(filename, FileMode.Create));
            byte[] content = GetBytes(fileContent);
            bwFile.Write(content, 0, content.Length);
            bwFile.Close();
        }

        private void WriteExFile(string fileContent)
        {
            string filename = path + DateTime.Now.ToString("dd-MM", CultureInfo.InvariantCulture) + "_EX.txt";
            BinaryWriter bwFile = new BinaryWriter(File.Open(filename, FileMode.OpenOrCreate));
            byte[] content = GetBytes(fileContent);
            bwFile.Write(content, 0, content.Length);
            bwFile.Close();
        }

        /// <summary>
        /// Metodo de ayuda para obtener los bytes de una cadena.
        /// </summary>
        private byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}
