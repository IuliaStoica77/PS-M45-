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
		public void Listen()
		{
			TcpListener server = null;
			try
			{				
				Int32 port = 2000;
				IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                server = new TcpListener(localAddr, port);
                Byte[] bytes = new Byte[2];

                server.Start();
		
				while (true)
				{                
                    Console.WriteLine("Waiting for a connection... ");

					TcpClient client = server.AcceptTcpClient();  
                    
                    Console.WriteLine("Connected!");

					NetworkStream stream = client.GetStream();

                    int i;

					while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
					{
                        //_process.UpdateState(bytes[0]);
                       
                        Console.WriteLine(string.Format("Received: {0}, {1}", bytes[0].ToString(), bytes[1].ToString()));
					}
					client.Close();
				}
			}
			catch (SocketException e)
			{              
                Console.WriteLine(string.Format("SocketException: {0}", e.ToString()));
			}
			finally
			{
				//se opreste serverul.
				server.Stop();
			}

            Console.WriteLine("Hit enter to continue...");            		
		}
	}	
}
