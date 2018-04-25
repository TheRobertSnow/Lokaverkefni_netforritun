using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.IO;

namespace ConsoleQuizClient //Breyta í project nafnið 
{
    class Program
    {
        private NetworkStream output;
        private BinaryWriter writer;
        private BinaryReader reader;
        private string message = "";
        private string lina = "";
        static int port = 8190;

        static void Main(string[] args)
        {
            new Program().Run();
        }
        void Run()
        {
            new Thread(Connect).Start();
        }

        void Connect()
        {
            TcpClient client = null;
            try
            {
                Console.WriteLine("Attempting connection...");
                client = new TcpClient();
                client.Connect("localhost", port);
                output = client.GetStream();
                writer = new BinaryWriter(output);
                reader = new BinaryReader(output);
                do
                {
                    try
                    {
                        
                        //kóðinn kemur hérna
                        int indx = 0;
                        while(indx < 10)
                        {
                            message = reader.ReadString();
                            Console.WriteLine(message);
                            lina = Console.ReadLine();
                            writer.Write(lina);
                            lina = null;
                            indx++;                                                      
                        }
                        
                    }
                    catch (Exception error)
                    {
                        Console.WriteLine(error.ToString());
                        Environment.Exit(Environment.ExitCode);
                    }
                } while (true);//á eftir að velja exit code

            }//end try
            catch (Exception error)
            {
                Console.WriteLine(error.ToString());
                Environment.Exit(Environment.ExitCode);
            }// end catch
            finally
            {
                reader.Close();
                writer.Close();
                output.Close();
                client.Close();
            }
            Console.ReadLine();
        }
    }
}
