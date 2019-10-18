using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSRCE_Client
{
    class Program
    {
        static void Main(string[] args)
        {

            string ip = "127.0.0.1";
            int port = 1337;

            // While it is able to make this console invisable, for now we shouldn't.
            Console.WriteLine("[!] This is a program that can potentially cause harm to your device, depending on the vendor.\n[!] Only install this from trusted sources!\n[!] Description: This program can execute remote code, which can be harmful.");
            Console.WriteLine("[!] Enter 'y' to accept.");
            bool answer = Console.ReadLine().ToLower() == "y";
            while (answer)
            {
                NetworkStream stream = null;
                try
                {
                    stream = Connect(ip, port);
                }
                catch { }

                if (stream != null)
                {
                    Console.WriteLine("[!] Connected to master server, running script...");

                    string code = Recieve(stream);

                    CSharpCodeProvider provider = new CSharpCodeProvider();
                    CompilerParameters parameters = new CompilerParameters();

                    if (code.Contains("Form"))
                    {
                        parameters.ReferencedAssemblies.Add("System.Windows.Forms.dll");
                        parameters.ReferencedAssemblies.Add("System.dll");
                    }
                    if (code.Contains("Drawing"))
                        parameters.ReferencedAssemblies.Add("System.Drawing.dll");

                    parameters.GenerateInMemory = true;
                    parameters.GenerateExecutable = true;

                    CompilerResults results = provider.CompileAssemblyFromSource(parameters, code);

                    if (results.Errors.HasErrors)
                    {
                        StringBuilder sb = new StringBuilder();

                        foreach (CompilerError error in results.Errors)
                        {
                            sb.AppendLine(string.Format("Error ({0}): {1} , Line: {2}", error.ErrorNumber, error.ErrorText, error.Line));
                        }

                        Console.WriteLine(sb.ToString());

                        Console.Read();
                        answer = false;
                    }
                    else
                    {
                        Assembly assembly = results.CompiledAssembly;
                        Type program = assembly.GetType("Program.Program");
                        MethodInfo main = program.GetMethod("Main");

                        main.Invoke(null, null);

                        answer = false;
                    }
                }
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Basic connect method
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns>NetworkStream</returns>

        public static NetworkStream Connect(string ip, int port)
        {
            TcpClient client = new TcpClient(ip, port);

            NetworkStream stream = client.GetStream();

            return stream;
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
                byte[] bytes = Encoding.UTF8.GetBytes(data + "\n");

                stream.Write(bytes, 0, bytes.Length);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Basic receive data :)
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>String</returns>
        
        public static string Recieve(NetworkStream stream)
        {
            try
            {
                byte[] bytes = new byte[1024];

                stream.Read(bytes, 0, bytes.Length);

                string data = Encoding.UTF8.GetString(bytes);

                stream.Flush();

                return data;
            }
            catch (Exception)
            {
                return "No Code";
            }

        }
    }
}
