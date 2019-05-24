using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SimulatorImproved;

namespace TCP_PLC
{
	public class Sender
	{		
        readonly Simulator process;

        public Sender(Simulator Process)
		{
            this.process = Process;
		}

		public void Send()
		{
			try
			{
				//Se creaza un TCP client cu adresa de IP si portul 
				TcpClient client = new TcpClient("127.0.0.1", 30000);

				while (true)
				{
					NetworkStream nwStream = client.GetStream();
					byte[] bytesToSend = new byte[2];
                    bytesToSend[0] = process.Get_State();
                    
					Console.WriteLine(string.Format("Sending bytes[0]= {0} and bytes[1] = {1}: ", bytesToSend[0].ToString(), bytesToSend[1].ToString()));
					nwStream.Write(bytesToSend, 0, bytesToSend.Length);
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
