using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace ConsoleQuizServer //Breyta þessu i nafnid a projecti
{
    class Program
    {
        private Socket connection;
        private int port = 8190;
        private int counter = 0;

        public struct Spurning
        {
            public string spurning, valmoguleikar, rettsvar;

            public Spurning(string sprng, string val, string rett)
            {
                spurning = sprng;
                valmoguleikar = val;
                rettsvar = rett;
            }
            public override string ToString()
            {
                return spurning + ";" + valmoguleikar + ";" + rettsvar;
            }
        }

        public Spurning[] spurningar = new Spurning[10];

        static void Main(string[] args)
        {
            new Program().Run();
        }


        void Run()
        {
            string line = "";
            int indx = 0;

            using (StreamReader sr = new StreamReader("spurningar.txt"))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                    string[] split = line.Split(';');
                    spurningar[indx] = new Spurning(split[0], split[1], split[2]);
                    indx++;
                }
            }
            new Thread(RunServer).Start();
        }

        public void RunServer()
        {
            Thread readThread;
            bool done = false;

            TcpListener listener;
            try
            {
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                //Console.WriteLine(spurningar[0]);
                Console.WriteLine("Waiting for connection...");

                while (!done)
                {
                    connection = listener.AcceptSocket();
                    counter++;
                    Console.WriteLine("Starting a new client, numbered " + counter);
                    readThread = new Thread(GetMesseges);
                    readThread.Start();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Port " + port + " may be busy. Try another.");
            }
        }

        public void GetMesseges()
        {
            Socket socket = connection;
            int count = counter;
            NetworkStream socketStream = null;
            BinaryWriter writer = null;
            BinaryReader reader = null;

            try
            {
                int rett = 0;
                int rangt = 0;
                int ind = 0;
                socketStream = new NetworkStream(socket);
                reader = new BinaryReader(socketStream);
                writer = new BinaryWriter(socketStream);
                //writer.Write("Connection successful. \n");
                string message = null;
                //Console.WriteLine(spurningar[0].spurning);
                for (int i = 0; i < spurningar.Length; i++)
                {
                     writer.Write(spurningar[i].spurning + spurningar[i].valmoguleikar);
                     message = reader.ReadString();
                     if (message == spurningar[i].rettsvar)
                     {
                         rett++;
                     }
                     else
                     {
                         rangt++;
                     }
                     Console.WriteLine(rett + " | " + rangt);
                }
                Console.ReadLine();
                /*while (ind < 2)
                {
                    writer.Write(spurningar[ind].spurning + spurningar[ind].valmoguleikar);
                    message = reader.ReadString();
                    if (message == spurningar[ind].rettsvar)
                    {
                        rett++;
                    }
                    else
                    {
                        rangt++;
                    }
                    
                    ind++;
                }*/

            }
            catch (Exception error)
            {
                Console.WriteLine(error.ToString());
            }
            finally
            {
                reader.Close();
                writer.Close();
                socketStream.Close();
                socket.Close();
            }
        }
    }
}
