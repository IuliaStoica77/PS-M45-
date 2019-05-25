using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCP_PLC
{
    public class Listener
    {
        private TcpListener tcpListener;
        private Int32 port;
        private IPAddress IPAddress;
        private Byte[] bytes;
        private TcpClient tcpClient;
        private NetworkStream networkStream;
        private ProcessLogic logic;

        public Listener(ProcessLogic logic)
        {
            this.logic = logic;
            this.port = 2000;
            this.IPAddress = IPAddress.Parse("127.0.0.1");
            this.tcpListener = new TcpListener(IPAddress, port);
            this.bytes = new byte[2];
        }

        public void Listen()
        {
            try
            {

                tcpListener.Start();

                while (true)
                {
                    Console.WriteLine("Waiting for a connection... ");

                    tcpClient = tcpListener.AcceptTcpClient();

                    Console.WriteLine("Connected!");

                    networkStream = tcpClient.GetStream();

                    int i;

                    while ((i = networkStream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        if (bytes[0] != 0)
                            logic.Process_Request((Simulator.Command)bytes[0]);

                        logic.Filling_Speed(bytes[1]);
                        Console.WriteLine(string.Format("Received: {0}, {1}", bytes[0].ToString(), bytes[1].ToString()));
                    }
                    tcpClient.Close();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(string.Format("SocketException: {0}", e.ToString()));
            }
            finally
            {
                tcpListener.Stop();
            }

            Console.WriteLine("Hit enter to continue...");
        }
    }
}
