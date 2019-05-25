using System;
using System.Net.Sockets;
using System.Threading;

namespace TCP_PLC
{
    public class Sender
    {
        readonly Simulator.Simulator process;
        readonly TcpClient tcpClient;
        NetworkStream networkStream;

        public Sender(Simulator.Simulator Process)
        {
            this.process = Process;
            this.tcpClient = new TcpClient("127.0.0.1", 30000);            // initializare client TCP
            this.networkStream = tcpClient.GetStream();
        }

        public void Send()
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("--------------------------");
                    Console.WriteLine(string.Format("Sending byte = {0} ", process.Get_State()));
                    //Console.WriteLine(string.Format("Water level: " + process.Level));
                    //Console.WriteLine(string.Format("Fill Speed: " + process.FillSpeed));
                    Console.WriteLine("--------------------------");
                    networkStream.Write(new byte[] { process.Get_State(), 0x00 }, 0, 2);
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
    }
}
