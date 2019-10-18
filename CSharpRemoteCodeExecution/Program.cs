using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace CSharpRemoteCodeExecution
{
    class Program
    {
        static string filePath = "";

        static void Main(string[] args)
        {
            // Check for a file in args
            if (args.Length != 1)
                return;
            if (!File.Exists(args[0]))
                return;

            filePath = args[0];

            TcpListener listener = new TcpListener(IPAddress.Any, 1337);
            listener.Start();

            Console.Title = "CSRCE Server - - - Made for NON-Malicous use!";

            Console.WriteLine("[!] Waiting for a connection...");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                ThreadPool.QueueUserWorkItem(DispatchScript, client);
            }
        }

        static void DispatchScript(object client)
        {
            // Just send the script
            Console.WriteLine("[!] Client connected, dispatching script...");

            TcpClient cl = (TcpClient)client;

            SendData(cl.GetStream(), File.ReadAllText(filePath));

        }

        /// <summary>
        /// Basic send data method
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="data"></param>
        /// <returns>Boolean</returns>

        public static bool SendData(NetworkStream stream, string data)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(data);

                stream.Write(bytes, 0, bytes.Length);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
