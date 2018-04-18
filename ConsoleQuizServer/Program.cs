using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace ConsoleQuizServer
{
    class Program
    {
        private Socket connection;
        private int port = 8190;
        private int counter = 0;

        public struct Spurning
        {
            public string spurning, valmoguleikiA, valmoguleikiB, valmoguleikiC, rettsvar;

            public Spurning(string sprng, string valA, string valB, string valC, string rett)
            {
                spurning = sprng;
                valmoguleikiA = valA;
                valmoguleikiB = valB;
                valmoguleikiC = valC;
                rettsvar = rett;
            }
            public override string ToString()
            {
                return spurning + ";" + valmoguleikiA + ";" + valmoguleikiB + ";" + valmoguleikiC + ";" + rettsvar; 
            }
        }

        public Spurning[] spurningar = new Spurning[2];

        static void Main(string[] args)
        {
            new Program().Run();
        }


        void Run()
        {
            string line = "";
            int indx = 0;

            using(StreamReader sr = new StreamReader("spurningar.txt"))
            {
                while((line = sr.ReadLine()) != null)
                {
                    //Console.WriteLine(line);                   
                    string[] split = line.Split(';');
                    spurningar[indx] = new Spurning(split[0],split[1],split[2],split[3],split[4]);
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

                while(!done)
                {
                    connection = listener.AcceptSocket();
                    counter++;
                    Console.WriteLine("Strating a new client, numbered " + counter);
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
                socketStream = new NetworkStream(socket);
                reader = new BinaryReader(socketStream);
                writer = new BinaryWriter(socketStream);
                writer.Write("Connection successful. \n");
                string message = null;
                Console.WriteLine(spurningar[0].spurning);
                
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
